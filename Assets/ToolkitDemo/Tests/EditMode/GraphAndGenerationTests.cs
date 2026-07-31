using GraphProcessor;
using NPBehaveEditor;
using NUnit.Framework;
using RedDotSystem.Editor;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ToolkitDemo.Tests
{
    public sealed class GraphAndGenerationTests
    {
        private const string BehaviorGraphPath =
            "Assets/ToolkitDemo/NPBehave/Graphs/BehaviorTreeDemo.asset";
        private const string RedDotGraphPath =
            "Assets/ToolkitDemo/RedDot/Graphs/RedDotDemo.asset";

        [Test]
        public void SampleGraphs_HaveDeterministicTopologyAndExports()
        {
            NPBehaveGraph behavior = AssetDatabase.LoadAssetAtPath<NPBehaveGraph>(BehaviorGraphPath);
            RedDotGraph redDot = AssetDatabase.LoadAssetAtPath<RedDotGraph>(RedDotGraphPath);

            Assert.That(behavior, Is.Not.Null);
            Assert.That(behavior.nodes.Count, Is.EqualTo(3));
            Assert.That(behavior.edges.Count, Is.EqualTo(2));
            Assert.That(behavior.nodes.Select(node => node.position.y).OrderBy(value => value),
                Is.EqualTo(new[] { 50f, 250f, 450f }));

            Assert.That(redDot, Is.Not.Null);
            Assert.That(redDot.nodes.Count, Is.EqualTo(3));
            Assert.That(redDot.edges.Count, Is.EqualTo(2));
            Assert.That(redDot.nodes.Select(node => node.position.x).OrderBy(value => value),
                Is.EqualTo(new[] { 80f, 380f, 680f }));

            Assert.That(File.Exists("Assets/ToolkitDemo/NPBehave/Exports/BehaviorTreeDemo.json"), Is.True);
            Assert.That(File.Exists("Assets/ToolkitDemo/RedDot/Exports/RedDotDemo.json"), Is.True);
        }

        [Test]
        public void GeneratedRedDotCode_PreservesMethodBodyAndUsesCrLf()
        {
            string folder = "Assets/ToolkitDemo/Runtime/RedDot/Generated";
            string[] expansionFiles = Directory.GetFiles(folder, "*Expansion*.cs");
            string combined = string.Join("\n", expansionFiles.Select(File.ReadAllText));

            Assert.That(combined.Split(new[] { "public void Inbox_Rewards" },
                System.StringSplitOptions.None).Length - 1, Is.EqualTo(1));
            Assert.That(combined, Does.Contain("DemoRedDotState.PrimaryCount"));

            foreach (string file in Directory.GetFiles(folder, "*.cs"))
            {
                AssertCrLfOnly(File.ReadAllBytes(file), file);
            }
        }

        private static void AssertCrLfOnly(byte[] bytes, string file)
        {
            for (int index = 0; index < bytes.Length; index++)
            {
                if (bytes[index] == 0x0A)
                {
                    Assert.That(index > 0 && bytes[index - 1] == 0x0D,
                        $"Lone LF found in {file} at byte {index}.");
                }

                if (bytes[index] == 0x0D)
                {
                    Assert.That(index + 1 < bytes.Length && bytes[index + 1] == 0x0A,
                        $"Lone CR found in {file} at byte {index}.");
                }
            }
        }
    }
}
