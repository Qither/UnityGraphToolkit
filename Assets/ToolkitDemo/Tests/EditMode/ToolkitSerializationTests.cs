using Newtonsoft.Json;
using NPBehave;
using NUnit.Framework;
using RedDotSystem.Runtime;
using System;
using System.Collections.Generic;
using ToolkitDemo.NPBehaveDemo;
using ToolkitDemo.RedDotDemo;

namespace ToolkitDemo.Tests
{
    public sealed class ToolkitSerializationTests
    {
        [Test]
        public void NPBehaveJson_RoundTripsAndRunsCustomAction()
        {
            NPBehaveData source = CreateBehaviorData();
            TestNPBehaveTypeProvider provider = new TestNPBehaveTypeProvider();
            NPBehaveJsonSerializer serializer = new NPBehaveJsonSerializer(provider);

            string json = serializer.Serialize(source, Formatting.Indented);
            NPBehaveData copy = serializer.Deserialize(json);
            DemoRuntimeTree tree = new DemoRuntimeTree("EditMode", copy, new Clock(() => 0.5f));
            tree.SetupRuntimeTree();
            tree.Start();

            Assert.That(tree.GetBlackboard().Get<int>("DemoRunCount"), Is.EqualTo(1));
            Assert.That(copy.allNode[0], Is.TypeOf<NPActionNodeData>());
            Assert.That(((NPActionNodeData)copy.allNode[0]).actionData,
                Is.TypeOf<DemoSetBlackboardAction>());
            tree.Stop();
        }

        [Test]
        public void NPBehaveJson_RejectsUnknownType()
        {
            NPBehaveJsonSerializer serializer = new NPBehaveJsonSerializer(new TestNPBehaveTypeProvider());
            string json = serializer.Serialize(CreateBehaviorData());
            string tampered = json.Replace(
                typeof(DemoSetBlackboardAction).FullName,
                typeof(Version).FullName);

            Assert.Throws<JsonSerializationException>(() => serializer.Deserialize(tampered));
        }

        [Test]
        public void RedDotJson_RoundTripsReferencesAndRejectsUnknownType()
        {
            RedDotData source = CreateRedDotData();
            RedDotJsonSerializer serializer = new RedDotJsonSerializer(new[]
            {
                typeof(DemoMultiRedDotData),
                typeof(DemoPassThroughRule)
            });

            string json = serializer.Serialize(source);
            RedDotConfigDocumentV1 document = serializer.DeserializeDocument(json);
            RedDotData inbox = document.root.NextData[0];
            RedDotData rewards = inbox.NextData[0];
            RedDotData multi = rewards.NextData[0];

            Assert.That(document.formatVersion, Is.EqualTo(1));
            Assert.That(json, Does.Contain("\"$ref\""));
            Assert.That(rewards.PreData, Is.SameAs(inbox));
            Assert.That(multi.PreData, Is.SameAs(rewards));

            string tampered = json.Replace(typeof(DemoMultiRedDotData).FullName, typeof(Version).FullName);
            Assert.Throws<JsonSerializationException>(() => serializer.Deserialize(tampered));
        }

        private static NPBehaveData CreateBehaviorData()
        {
            NPActionNodeData action = new NPActionNodeData
            {
                id = 0,
                nodeDes = "Test action",
                actionData = new DemoSetBlackboardAction()
            };
            NPSequenceNodeData sequence = new NPSequenceNodeData
            {
                id = 1,
                nodeDes = "Test sequence",
                linkedIds = new List<int> { 0 }
            };
            NPRootNodeData root = new NPRootNodeData
            {
                id = 2,
                nodeDes = "Test root",
                linkedIds = new List<int> { 1 }
            };
            root.blackboardValues.Add("DemoRunCount", new SharedInt());

            NPBehaveData data = new NPBehaveData { id = "EditModeBehavior" };
            data.allNode.Add(0, action);
            data.allNode.Add(1, sequence);
            data.allNode.Add(2, root);
            return data;
        }

        internal static RedDotData CreateRedDotData()
        {
            RedDotSingeData root = new RedDotSingeData("ROOT", null, 0,
                new List<RedDotData>(), null, null);
            RedDotSingeData inbox = new RedDotSingeData("ROOT_Inbox", root, 0,
                new List<RedDotData>(), null, null);
            LinkedList<RedDotRule> rules = new LinkedList<RedDotRule>();
            rules.AddLast(new DemoPassThroughRule());
            RedDotSingeData rewards = new RedDotSingeData("ROOT_Inbox_Rewards", inbox, 0,
                new List<RedDotData>(), null, rules);
            DemoMultiRedDotData multi = new DemoMultiRedDotData(rewards.NodeName, rewards, 1,
                new List<RedDotData>(), null);
            rewards.NextData.Add(multi);
            inbox.NextData.Add(rewards);
            root.NextData.Add(inbox);
            return root;
        }

        private sealed class TestNPBehaveTypeProvider : INPBehaveTypeProvider
        {
            public Type[] GetKnownTypes()
            {
                return new[]
                {
                    typeof(NPRootNodeData),
                    typeof(NPSequenceNodeData),
                    typeof(NPActionNodeData),
                    typeof(DemoSetBlackboardAction),
                    typeof(SharedInt)
                };
            }
        }
    }
}
