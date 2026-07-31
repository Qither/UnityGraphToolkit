using NPBehave;
using RedDotSystem.Runtime;
using System;
using System.Collections.Generic;
using ToolkitDemo.NPBehaveDemo;
using ToolkitDemo.RedDot.Generated;
using ToolkitDemo.RedDotDemo;
using UnityEngine;

namespace ToolkitDemo
{
    public sealed class ToolkitDemoController : MonoBehaviour, INPBehaveTypeProvider
    {
        public TextAsset behaviorTreeJson;

        public GameObject redDotIndicator;

        public int BehaviorRunCount { get; private set; }

        public int RedDotValue { get; private set; }

        public bool IsDemoSystemAvailable { get; private set; } = true;

        private RedDotService m_RedDotService;

        private DemoRuntimeTree m_RuntimeTree;

        private void Start()
        {
            this.InitializeRedDot();
            this.RunBehaviorOnce();
        }

        private void OnDestroy()
        {
            this.m_RuntimeTree?.Stop();
            this.m_RedDotService?.Dispose();
        }

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

        public void RunBehaviorOnce()
        {
            if (this.behaviorTreeJson == null)
            {
                Debug.LogWarning("Assign the exported behavior-tree JSON TextAsset.");
                return;
            }

            this.m_RuntimeTree?.Stop();
            NPBehaveJsonSerializer serializer = new NPBehaveJsonSerializer(this);
            NPBehaveData data = serializer.Deserialize(this.behaviorTreeJson.text);
            Clock clock = new Clock(() => 0.5f);
            this.m_RuntimeTree = new DemoRuntimeTree("ToolkitDemo", data, clock);
            this.m_RuntimeTree.SetupRuntimeTree();
            this.m_RuntimeTree.Start();
            this.BehaviorRunCount = this.m_RuntimeTree.GetBlackboard().Get<int>("DemoRunCount");
        }

        public void IncrementPrimary()
        {
            DemoRedDotState.PrimaryCount++;
            this.RefreshRedDot();
        }

        public void IncrementSecondary()
        {
            DemoRedDotState.SecondaryCount++;
            this.RefreshRedDot();
        }

        public void ClearRedDots()
        {
            DemoRedDotState.PrimaryCount = 0;
            DemoRedDotState.SecondaryCount = 0;
            this.RefreshRedDot();
        }

        public void ToggleSystemAvailability()
        {
            this.IsDemoSystemAvailable = !this.IsDemoSystemAvailable;
            this.RefreshRedDot();
        }

        private void InitializeRedDot()
        {
            DemoRedDotState.PrimaryCount = 0;
            DemoRedDotState.SecondaryCount = 0;
            this.m_RedDotService = new RedDotService(
                new RedDotExecuteNode(),
                BuildRedDotData(),
                systemId => systemId != 1 || this.IsDemoSystemAvailable);
            this.m_RedDotService.StaticBind("ROOT_Inbox", this.OnRedDotChanged);
            this.RefreshRedDot();
        }

        private void RefreshRedDot()
        {
            if (this.m_RedDotService == null)
            {
                return;
            }

            this.m_RedDotService.ExecuteNodeDataDirty("ROOT_Inbox_Rewards_ALPHA");
            this.m_RedDotService.ExecuteNodeDataDirty("ROOT_Inbox_Rewards_BETA");
            this.m_RedDotService.Update();
        }

        private void OnRedDotChanged(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            this.RedDotValue = node?.Data.nodeValue ?? 0;
            if (this.redDotIndicator != null)
            {
                this.redDotIndicator.SetActive(this.RedDotValue > 0);
            }
        }

        private static RedDotData BuildRedDotData()
        {
            RedDotSingeData root = new RedDotSingeData(
                "ROOT", null, 0, new List<RedDotData>(), null, null);
            RedDotSingeData inbox = new RedDotSingeData(
                "ROOT_Inbox", root, 0, new List<RedDotData>(), null, null);
            LinkedList<RedDotRule> rules = new LinkedList<RedDotRule>();
            rules.AddLast(new DemoPassThroughRule());
            RedDotSingeData rewards = new RedDotSingeData(
                "ROOT_Inbox_Rewards", inbox, 0, new List<RedDotData>(), null, rules);
            DemoMultiRedDotData multi = new DemoMultiRedDotData(
                rewards.NodeName, rewards, 1, new List<RedDotData>(), null);

            rewards.NextData.Add(multi);
            inbox.NextData.Add(rewards);
            root.NextData.Add(inbox);
            return root;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(20, 20, 460, 300), GUI.skin.box);
            GUILayout.Label("UnityGraphToolkit Runtime Demo");
            GUILayout.Label($"Behavior blackboard DemoRunCount: {this.BehaviorRunCount}");
            if (GUILayout.Button("Run behavior tree from JSON"))
            {
                this.RunBehaviorOnce();
            }

            GUILayout.Space(12);
            GUILayout.Label($"Red-dot aggregate: {this.RedDotValue}");
            GUILayout.Label($"System 1 available: {this.IsDemoSystemAvailable}");
            if (GUILayout.Button("Increment ALPHA"))
            {
                this.IncrementPrimary();
            }

            if (GUILayout.Button("Increment BETA"))
            {
                this.IncrementSecondary();
            }

            if (GUILayout.Button("Toggle system availability"))
            {
                this.ToggleSystemAvailability();
            }

            if (GUILayout.Button("Clear red dots"))
            {
                this.ClearRedDots();
            }

            GUILayout.EndArea();
        }
    }
}
