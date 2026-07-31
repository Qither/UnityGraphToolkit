using GraphProcessor;
using NPBehave;
using NPBehaveEditor;
using RedDotSystem.Editor;
using RedDotSystem.Editor.Node;
using System;
using System.Collections.Generic;
using System.IO;
using ToolkitDemo.NPBehaveDemo;
using ToolkitDemo.RedDotDemo;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToolkitDemo.Editor
{
    public static class ToolkitDemoBuilder
    {
        private const string RootFolder = "Assets/ToolkitDemo";
        private const string BehaviorFolder = RootFolder + "/NPBehave";
        private const string BehaviorGraphPath = BehaviorFolder + "/Graphs/BehaviorTreeDemo.asset";
        private const string BehaviorSettingsPath = BehaviorFolder + "/NPBehaveGraphSettings.asset";
        private const string BehaviorJsonPath = BehaviorFolder + "/Exports/BehaviorTreeDemo.json";
        private const string RedDotFolder = RootFolder + "/RedDot";
        private const string RedDotGraphPath = RedDotFolder + "/Graphs/RedDotDemo.asset";
        private const string RedDotSettingsPath = RedDotFolder + "/RedDotGraphSettings.asset";
        private const string DemoScenePath = RootFolder + "/Scenes/ToolkitDemo.unity";

        [MenuItem("Tools/Unity Graph Toolkit/Create or Rebuild Samples", priority = 1)]
        public static void BuildAll()
        {
            EnsureFolders();
            NPBehaveGraphSettings behaviorSettings = BuildBehaviorSettings();
            NPBehaveGraph behaviorGraph = BuildBehaviorGraph();
            ExportBehaviorGraph(behaviorSettings, behaviorGraph);

            RedDotGraphSettings redDotSettings = BuildRedDotSettings();
            RedDotGraph redDotGraph = BuildRedDotGraph(redDotSettings);
            redDotGraph.RedDotExecuteNodeGenerate = new RedDotExecuteNodeGenerate(redDotGraph);
            redDotGraph.RedDotExecuteNodeGenerate.Generate();
            redDotGraph.ExportRedDotNode();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            BuildDemoScene();
            AssetDatabase.SaveAssets();
            Debug.Log("UnityGraphToolkit samples rebuilt successfully.");
        }

        [MenuItem("Tools/Unity Graph Toolkit/Behavior Tree/Open Graph", priority = 20)]
        public static void OpenBehaviorGraph()
        {
            OpenAsset<NPBehaveGraph>(BehaviorGraphPath);
        }

        [MenuItem("Tools/Unity Graph Toolkit/Behavior Tree/Auto Layout", priority = 21)]
        public static void LayoutBehaviorGraphMenu()
        {
            NPBehaveGraph graph = AssetDatabase.LoadAssetAtPath<NPBehaveGraph>(BehaviorGraphPath);
            LayoutBehaviorGraph(graph);
            SaveGraph(graph);
        }

        [MenuItem("Tools/Unity Graph Toolkit/Behavior Tree/Export JSON", priority = 22)]
        public static void ExportBehaviorGraphMenu()
        {
            ExportBehaviorGraph(
                AssetDatabase.LoadAssetAtPath<NPBehaveGraphSettings>(BehaviorSettingsPath),
                AssetDatabase.LoadAssetAtPath<NPBehaveGraph>(BehaviorGraphPath));
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Unity Graph Toolkit/Red Dot/Open Graph", priority = 30)]
        public static void OpenRedDotGraph()
        {
            OpenAsset<RedDotGraph>(RedDotGraphPath);
        }

        [MenuItem("Tools/Unity Graph Toolkit/Red Dot/Auto Layout", priority = 31)]
        public static void LayoutRedDotGraphMenu()
        {
            RedDotGraph graph = AssetDatabase.LoadAssetAtPath<RedDotGraph>(RedDotGraphPath);
            LayoutRedDotGraph(graph);
            SaveGraph(graph);
        }

        [MenuItem("Tools/Unity Graph Toolkit/Red Dot/Generate Code and Export JSON", priority = 32)]
        public static void ExportRedDotGraphMenu()
        {
            RedDotGraph graph = AssetDatabase.LoadAssetAtPath<RedDotGraph>(RedDotGraphPath);
            graph.RedDotExecuteNodeGenerate = new RedDotExecuteNodeGenerate(graph);
            graph.RedDotExecuteNodeGenerate.Generate();
            graph.ExportRedDotNode();
        }

        [MenuItem("Tools/Unity Graph Toolkit/Demo/Open Scene", priority = 40)]
        public static void OpenDemoScene()
        {
            EditorSceneManager.OpenScene(DemoScenePath, OpenSceneMode.Single);
        }

        [MenuItem("Tools/Unity Graph Toolkit/Demo/Run", priority = 41)]
        public static void RunDemo()
        {
            OpenDemoScene();
            EditorApplication.isPlaying = true;
        }

        private static NPBehaveGraphSettings BuildBehaviorSettings()
        {
            DeleteAssetIfPresent(BehaviorSettingsPath);
            NPBehaveGraphSettings settings = ScriptableObject.CreateInstance<NPBehaveGraphSettings>();
            settings.graphPath = BehaviorFolder + "/Graphs";
            settings.graphExportPath = BehaviorFolder + "/Exports";
            settings.actionPath = BehaviorFolder + "/Actions";
            settings.actionNodePath = BehaviorFolder + "/Actions";
            settings.assetBundleName = string.Empty;
            AssetDatabase.CreateAsset(settings, BehaviorSettingsPath);
            return settings;
        }

        private static NPBehaveGraph BuildBehaviorGraph()
        {
            DeleteAssetIfPresent(BehaviorGraphPath);
            NPBehaveGraph graph = ScriptableObject.CreateInstance<NPBehaveGraph>();
            AssetDatabase.CreateAsset(graph, BehaviorGraphPath);

            NPRootNode root = BaseNode.CreateFromType<NPRootNode>(Vector2.zero);
            root.RootNodeData.nodeDes = "Demo root";
            root.RootNodeData.blackboardValues.Add("DemoRunCount", new SharedInt());
            NPSequenceNode sequence = BaseNode.CreateFromType<NPSequenceNode>(Vector2.zero);
            DemoSetBlackboardActionNode action =
                BaseNode.CreateFromType<DemoSetBlackboardActionNode>(Vector2.zero);

            graph.AddNode(root);
            graph.AddNode(sequence);
            graph.AddNode(action);
            graph.Connect(sequence.GetPort(nameof(ANPCompositeNodeBase.PrevNode), null),
                root.GetPort(nameof(NPRootNode.NextNode), null));
            graph.Connect(action.GetPort(nameof(ANPTaskNodeBase.PrevNode), null),
                sequence.GetPort(nameof(ANPCompositeNodeBase.NextNode), null));
            LayoutBehaviorGraph(graph);
            graph.UpdateComputeOrder();
            SaveGraph(graph);
            return graph;
        }

        private static void ExportBehaviorGraph(NPBehaveGraphSettings settings, NPBehaveGraph graph)
        {
            if (settings == null || graph == null)
            {
                throw new InvalidOperationException("Build the NPBehave sample before exporting it.");
            }

            settings.SetExporter();
            string guid = AssetDatabase.AssetPathToGUID(BehaviorGraphPath);
            graph.Export(settings, BehaviorGraphPath, guid);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        private static RedDotGraphSettings BuildRedDotSettings()
        {
            DeleteAssetIfPresent(RedDotSettingsPath);
            RedDotGraphSettings settings = ScriptableObject.CreateInstance<RedDotGraphSettings>();
            settings.executeNodePath = RootFolder + "/Runtime/RedDot/Generated";
            settings.exportPath = RedDotFolder + "/Exports";
            settings.exportFileName = "RedDotDemo.json";
            settings.executeNodeNamespace = "ToolkitDemo.RedDot.Generated";
            settings.usingNamespaces = new List<string>
            {
                "RedDotSystem.Runtime",
                "ToolkitDemo.RedDotDemo"
            };
            settings.systemOptions = new List<RedDotSystemOption>
            {
                new RedDotSystemOption { value = 0, label = "Always available" },
                new RedDotSystemOption { value = 1, label = "Demo system" }
            };
            AssetDatabase.CreateAsset(settings, RedDotSettingsPath);
            return settings;
        }

        private static RedDotGraph BuildRedDotGraph(RedDotGraphSettings settings)
        {
            DeleteAssetIfPresent(RedDotGraphPath);
            RedDotGraph graph = ScriptableObject.CreateInstance<RedDotGraph>();
            graph.Settings = settings;
            AssetDatabase.CreateAsset(graph, RedDotGraphPath);
            graph.DiscoverTypes();

            RedDotNode inbox = BaseNode.CreateFromType<RedDotNode>(Vector2.zero);
            inbox.SetCustomName("Inbox");
            inbox.Data.Key = "Inbox";
            inbox.Data.Desc = "Normal aggregate node";

            RedDotNode rewards = BaseNode.CreateFromType<RedDotNode>(Vector2.zero);
            rewards.SetCustomName("Rewards");
            rewards.Data.Key = "Inbox\nRewards";
            rewards.Data.Desc = "Leaf execution node with a custom rule";
            rewards.Rule.Rules.Add(new RuleData
            {
                Name = nameof(DemoPassThroughRule),
                Graph = graph,
                NodeRule = rewards.Rule
            });

            RedDotMultiNode multi = BaseNode.CreateFromType<RedDotMultiNode>(Vector2.zero);
            multi.Data.MultiNodeData = nameof(DemoMultiRedDotData);
            multi.Data.System = 1;

            graph.AddNode(inbox);
            graph.AddNode(rewards);
            graph.AddNode(multi);
            graph.Connect(rewards.GetPort(nameof(RedDotNode.WordInput), null),
                inbox.GetPort(nameof(RedDotNode.WordOutput), null));
            graph.Connect(multi.GetPort(nameof(RedDotMultiNode.WordInput), null),
                rewards.GetPort(nameof(RedDotNode.WordOutput), null));
            LayoutRedDotGraph(graph);
            graph.UpdateComputeOrder();
            inbox.SetUp();
            rewards.SetUp();
            multi.SetUp();
            SaveGraph(graph);
            return graph;
        }

        private static void LayoutBehaviorGraph(NPBehaveGraph graph)
        {
            if (graph == null)
            {
                return;
            }

            foreach (BaseNode node in graph.nodes)
            {
                Vector2 position = node switch
                {
                    NPRootNode => new Vector2(100, 50),
                    NPSequenceNode => new Vector2(100, 250),
                    DemoSetBlackboardActionNode => new Vector2(100, 450),
                    _ => node.position.position
                };
                node.position = new Rect(position, new Vector2(240, 120));
            }
        }

        private static void LayoutRedDotGraph(RedDotGraph graph)
        {
            if (graph == null)
            {
                return;
            }

            foreach (BaseNode node in graph.nodes)
            {
                Vector2 position = node switch
                {
                    RedDotNode redDotNode when redDotNode.GetCustomName() == "Inbox" => new Vector2(80, 80),
                    RedDotNode => new Vector2(380, 80),
                    RedDotMultiNode => new Vector2(680, 80),
                    _ => node.position.position
                };
                node.position = new Rect(position, new Vector2(250, 160));
            }
        }

        private static void BuildDemoScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "ToolkitDemo";

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0, 0, -10);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.1f, 0.14f);

            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.name = "Red Dot Indicator";
            indicator.transform.position = new Vector3(3.3f, 1.4f, 0);
            indicator.transform.localScale = Vector3.one * 0.8f;
            indicator.SetActive(false);

            GameObject demoObject = new GameObject("UnityGraphToolkit Demo");
            ToolkitDemoController controller = demoObject.AddComponent<ToolkitDemoController>();
            controller.behaviorTreeJson = AssetDatabase.LoadAssetAtPath<TextAsset>(BehaviorJsonPath);
            controller.redDotIndicator = indicator;

            EditorSceneManager.SaveScene(scene, DemoScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(DemoScenePath, true) };
        }

        private static void EnsureFolders()
        {
            EnsureFolder(RootFolder);
            EnsureFolder(BehaviorFolder);
            EnsureFolder(BehaviorFolder + "/Graphs");
            EnsureFolder(BehaviorFolder + "/Exports");
            EnsureFolder(BehaviorFolder + "/Actions");
            EnsureFolder(RedDotFolder);
            EnsureFolder(RedDotFolder + "/Graphs");
            EnsureFolder(RedDotFolder + "/Exports");
            EnsureFolder(RootFolder + "/Scenes");
        }

        private static void EnsureFolder(string assetPath)
        {
            string[] parts = assetPath.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private static void DeleteAssetIfPresent(string path)
        {
            if (AssetDatabase.LoadMainAssetAtPath(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        private static void SaveGraph(BaseGraph graph)
        {
            if (graph == null)
            {
                return;
            }

            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
        }

        private static void OpenAsset<T>(string path) where T : UnityEngine.Object
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                throw new FileNotFoundException("Build the sample assets first.", path);
            }

            AssetDatabase.OpenAsset(asset);
        }
    }
}
