using NPBehave;
using System;
using System.Linq;
using GraphToolkit.Inspector;
using GraphToolkit.Inspector.Editor;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Exception = System.Exception;
using Object = UnityEngine.Object;

namespace NPBehaveEditor
{
    [CustomEditor(typeof(NPBehaveGraphSettings))]
    public class NPBehaveGraphSettingsInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var settings = target as NPBehaveGraphSettings;

            if (settings != null)
            {
                var actionPath = new TextField("ActionPath") { style = { flexGrow = 1 } };
                actionPath.Q<VisualElement>("unity-text-input").style.unityTextAlign = TextAnchor.MiddleRight;
                actionPath.value                = settings.actionPath;
                actionPath.RegisterValueChangedCallback(evt =>
                {
                    settings.actionPath = evt.newValue;
                    EditorUtility.SetDirty(settings);
                });
                Button browseActionPath = new Button(() =>
                {
                    var path = EditorUtility.OpenFolderPanel("ActionPath", settings.actionPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        path                = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        settings.actionPath = path;
                        actionPath.value    = path;
                        EditorUtility.SetDirty(settings);
                    }
                }) { text = "Browse" };
                actionPath.Add(browseActionPath);
                root.Add(actionPath);

                var actionNodePath = new TextField("ActionNodePath") { style = { flexGrow = 1 } };
                actionNodePath.Q<VisualElement>("unity-text-input").style.unityTextAlign = TextAnchor.MiddleRight;
                actionNodePath.value                = settings.actionNodePath;
                actionNodePath.RegisterValueChangedCallback(evt =>
                {
                    settings.actionNodePath = evt.newValue;
                    EditorUtility.SetDirty(settings);
                });
                Button browseActionNodePath = new Button(() =>
                {
                    var path = EditorUtility.OpenFolderPanel("ActionNodePath", settings.actionNodePath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        path                    = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        settings.actionNodePath = path;
                        actionNodePath.value    = path;
                        EditorUtility.SetDirty(settings);
                    }
                }) { text = "Browse" };

                actionNodePath.Add(browseActionNodePath);
                root.Add(actionNodePath);

                var graphPath = new TextField("GraphPath") { style = { flexGrow = 1 } };
                graphPath.Q<VisualElement>("unity-text-input").style.unityTextAlign = TextAnchor.MiddleRight;
                graphPath.value                = settings.graphPath;
                graphPath.RegisterValueChangedCallback(evt =>
                {
                    settings.graphPath = evt.newValue;
                    EditorUtility.SetDirty(settings);
                });
                Button browseGraphPath = new Button(() =>
                {
                    var path = EditorUtility.OpenFolderPanel("GraphPath", settings.graphPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        path               = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        settings.graphPath = path;
                        graphPath.value    = path;
                        EditorUtility.SetDirty(settings);
                    }
                }) { text = "Browse" };
                graphPath.Add(browseGraphPath);
                root.Add(graphPath);

                var graphExportPath = new TextField("GraphExportPath") { style = { flexGrow = 1 } };
                graphExportPath.Q<VisualElement>("unity-text-input").style.unityTextAlign = TextAnchor.MiddleRight;
                graphExportPath.value                = settings.graphExportPath;
                graphExportPath.RegisterValueChangedCallback(evt =>
                {
                    settings.graphExportPath = evt.newValue;
                    EditorUtility.SetDirty(settings);
                });
                Button browseGraphExportPath = new Button(() =>
                {
                    var path = EditorUtility.OpenFolderPanel("GraphExportPath", settings.graphExportPath, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        path                     = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));
                        settings.graphExportPath = path;
                        graphExportPath.value    = path;
                        EditorUtility.SetDirty(settings);
                    }
                }) { text = "Browse" };
                graphExportPath.Add(browseGraphExportPath);
                root.Add(graphExportPath);

                var actionNodeTemplate = new ObjectField("ActionNodeTemplate") { style = { flexGrow = 1 } };
                actionNodeTemplate.objectType = typeof(TextAsset);
                actionNodeTemplate.value      = settings.actionNodeTemplate;
                actionNodeTemplate.RegisterValueChangedCallback(evt =>
                {
                    settings.actionNodeTemplate = evt.newValue as TextAsset;
                    EditorUtility.SetDirty(settings);
                });
                root.Add(actionNodeTemplate);

                var behaveGraphBundleName = new TextField("AssetBundleName") { style = { flexGrow = 1 } };
                behaveGraphBundleName.value = settings.assetBundleName;
                behaveGraphBundleName.RegisterValueChangedCallback(evt =>
                {
                    settings.assetBundleName = evt.newValue;
                    EditorUtility.SetDirty(settings);
                });
                root.Add(behaveGraphBundleName);
                
                SerializedProperty property     = serializedObject.FindProperty("exportScript");
                VisualElement      exportScript = property.CreateField();
                exportScript.style.flexGrow = 1;
                root.Add(exportScript);


                if (settings.actionPath == null || settings.actionNodePath == null || settings.actionNodeTemplate == null)
                    return root;

                string assetActionPath = settings.actionPath.Substring(settings.actionPath.IndexOf("Assets", StringComparison.Ordinal));

                string[] actionScriptGuid = AssetDatabase.FindAssets("t:script", new[] { assetActionPath });
                if (!actionScriptGuid.Any()) return root;

                Box box = new Box()
                {
                    style =
                    {
                        borderTopColor          = Color.black,
                        borderTopWidth          = 1,
                        borderLeftColor         = Color.black,
                        borderLeftWidth         = 1,
                        borderRightColor        = Color.black,
                        borderRightWidth        = 1,
                        borderBottomColor       = Color.black,
                        borderBottomWidth       = 1,
                        marginBottom            = 5,
                        marginLeft              = 5,
                        marginRight             = 5,
                        marginTop               = 5,
                        borderTopLeftRadius     = 5,
                        borderTopRightRadius    = 5,
                        borderBottomLeftRadius  = 5,
                        borderBottomRightRadius = 5,
                    }
                };

                Foldout actionFoldout = new Foldout()
                {
                    text  = "Action",
                    value = settings.IsShowAction,
                    style =
                    {
                        marginBottom = 5,
                        marginLeft   = 20,
                        marginRight  = 5,
                        marginTop    = 5,
                    }
                };

                List<string> fixList = null;

                actionFoldout.RegisterValueChangedCallback(evt =>
                {
                    settings.IsShowAction = evt.newValue;

                    if (settings.IsShowAction)
                    {
                        fixList = this.CreateActionElement(actionFoldout.contentContainer, settings, actionScriptGuid);
                    }
                    else
                    {
                        actionFoldout.contentContainer.Clear();
                    }
                });
                box.Add(actionFoldout);

                if (settings.IsShowAction)
                {
                    fixList = this.CreateActionElement(actionFoldout.contentContainer, settings, actionScriptGuid);
                }

                string   assetGraphPath = settings.graphPath.Substring(settings.graphPath.IndexOf("Assets", StringComparison.Ordinal));
                string[] graphGuid      = AssetDatabase.FindAssets("t:NPBehaveGraph", new[] { assetGraphPath });
                if (!graphGuid.Any())
                {
                    root.Add(box);
                    return root;
                }

                Foldout behaveFoldout = new Foldout()
                {
                    text  = "Behave Graph",
                    value = settings.IsShowBehaveGraph,
                    style =
                    {
                        marginBottom = 5,
                        marginLeft   = 20,
                        marginRight  = 5,
                        marginTop    = 5,
                    }
                };

                behaveFoldout.RegisterValueChangedCallback(evt =>
                {
                    settings.IsShowBehaveGraph = evt.newValue;

                    if (settings.IsShowBehaveGraph)
                    {
                        this.CreateGraphElement(behaveFoldout.contentContainer, settings, graphGuid);
                    }
                    else
                    {
                        behaveFoldout.contentContainer.Clear();
                    }
                });
                box.Add(behaveFoldout);

                if (settings.IsShowBehaveGraph)
                {
                    this.CreateGraphElement(behaveFoldout.contentContainer, settings, graphGuid);
                }

                root.Add(box);

                Button fixButton = new Button(() =>
                {
                    if (null != fixList)
                    {
                        for (int i = 0; i < fixList.Count; i += 2)
                        {
                            var actionNodeTemplateText = settings.actionNodeTemplate.text;
                            actionNodeTemplateText = actionNodeTemplateText.Replace("_CLASSNAME_", fixList[i].Substring(2));
                            actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEMENULABEL_", fixList[i + 1]);
                            
                            int lastNameIndex = fixList[i + 1].LastIndexOf('/');
                            string realName = fixList[i + 1];
                            if (lastNameIndex >= 0)
                            {
                                realName = realName.Substring(lastNameIndex + 1);
                            }
                            actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEMENULABELNAME_", realName);
                            
                            if (fixList[i].Contains("NPCond"))
                            {
                                actionNodeTemplateText = actionNodeTemplateText.Replace("Task/", "Condition/");
                                actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEICON_", "DarkConditionalIcon");
                            }
                            else
                            {
                                actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEICON_", "DarkActionIcon");
                            }

                            var actionNodeTemplatePath = settings.actionNodePath + "/" + fixList[i] + "Node.cs";
                            actionNodeTemplatePath = Application.dataPath + actionNodeTemplatePath.Substring(6);
                            System.IO.File.WriteAllText(actionNodeTemplatePath, actionNodeTemplateText);
                        }

                        AssetDatabase.Refresh();
                    }
                }) { text = "Fix All Action" };
                root.Add(fixButton);

                Button exportButton = new Button(() =>
                {
                    this.ExportAllBehaveGraph(settings);
                    behaveFoldout.contentContainer.Clear();
                    this.CreateGraphElement(behaveFoldout.contentContainer, settings, graphGuid);
                    Debug.Log($"Export All Graph Done!");
                }) { text = "Export All Graph" };
                root.Add(exportButton);
                Button clearExportCRCButton = new Button(() =>
                {
                    settings.graphCRCDictionary.Clear();
                    EditorUtility.SetDirty(settings);
                    Debug.Log($"Clear Export CRC Done!");
                }) { text = "Clear Export CRC" };
                root.Add(clearExportCRCButton);
            }

            return root;
        }
        
        private List<string> CreateActionElement(VisualElement root, NPBehaveGraphSettings settings, string[] actionScriptGuid)
        {
            string assetActionNodePath = settings.actionNodePath.Substring(settings.actionNodePath.IndexOf("Assets", StringComparison.Ordinal));
            List<string> fixList = new List<string>();
            
            foreach (string guid in actionScriptGuid)
            {
                string     path   = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null)
                {
                    var   actionName  = script.name;
                    VisualElement scriptContainer = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Row,
                            marginBottom  = 5,
                            marginLeft    = 5,
                            marginRight   = 5,
                            marginTop     = 5,
                        }
                    };
                    Label actionLabel = new Label(actionName)
                    {
                        style =
                        {
                            flexGrow = 1,
                        }
                    };
                    scriptContainer.Add(actionLabel);
                    root.Add(scriptContainer);
                    var actionNodeName = actionName + "Node";
                    var actionNodeScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetActionNodePath + "/" + actionNodeName + ".cs");
                    if (actionNodeScript == null)
                    {
                        fixList.Add(actionName);

                        string labelName = script.GetClass()?.GetCustomAttribute<NodeMenuLabelAttribute>()?.Label ?? actionName.Substring(2);
                        fixList.Add(labelName);

                        Button fixButton = new Button(() =>
                        {
                            var actionNodeTemplateText = settings.actionNodeTemplate.text;
                            actionNodeTemplateText = actionNodeTemplateText.Replace("_CLASSNAME_", actionName.Substring(2));
                            actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEMENULABEL_", labelName);
                            
                            int lastNameIndex = labelName.LastIndexOf('/');
                            string realName = labelName;
                            if (lastNameIndex >= 0)
                            {
                                realName = realName.Substring(lastNameIndex + 1);
                            }
                            actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEMENULABELNAME_", realName);

                            if (actionName.Contains("NPCond"))
                            {
                                actionNodeTemplateText = actionNodeTemplateText.Replace("Task/", "Condition/");
                                actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEICON_", "DarkConditionalIcon");
                            }
                            else
                            {
                                actionNodeTemplateText = actionNodeTemplateText.Replace("_NODEICON_", "DarkActionIcon");
                            }

                            var actionNodeTemplatePath = settings.actionNodePath + "/" + actionNodeName + ".cs";
                            System.IO.File.WriteAllText(actionNodeTemplatePath, actionNodeTemplateText);
                            AssetDatabase.Refresh();
                        }) {text = "Fix"};
                        scriptContainer.Add(fixButton);
                        MessageBox messageBox = new MessageBox(MessageBoxType.Error, "Not find ActionNode for " + actionName);
                        root.Add(messageBox);
                    }
                    else
                    {
                        Button nodeButton = new Button(() =>
                        {
                            EditorGUIUtility.PingObject(actionNodeScript);
                            //Selection.activeObject = actionNodeScript;
                        }) {text = "Select Node"};
                        scriptContainer.Add(nodeButton);
                        Button dataButton = new Button(() =>
                        {
                            EditorGUIUtility.PingObject(script);
                            //Selection.activeObject = script;
                        }) {text = "Select Data"};
                        scriptContainer.Add(dataButton);
                    }
                }
            }

            return fixList;
        }
        
        private void CreateGraphElement(VisualElement root, NPBehaveGraphSettings settings, string[] graphGuid)
        {
            string assetGraphExportPath = settings.graphExportPath.Substring(settings.graphExportPath.IndexOf("Assets", StringComparison.Ordinal));
            
            foreach (string guid in graphGuid)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var graph = AssetDatabase.LoadAssetAtPath<NPBehaveGraph>(path);
                if (graph != null)
                {
                    var graphName = graph.name;
                    VisualElement graphContainer = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Row,
                            marginBottom  = 5,
                            marginLeft    = 5,
                            marginRight   = 5,
                            marginTop     = 5,
                        }
                    };
                    Label graphLabel = new Label(graphName)
                    {
                        style =
                        {
                            flexGrow = 1,
                        }
                    };
                    graphContainer.Add(graphLabel);
                    root.Add(graphContainer);
                    var graphExportPath = assetGraphExportPath + "/" + graph.name + ".json";
                    var graphExport     = AssetDatabase.LoadAssetAtPath<TextAsset>(graphExportPath);
                    if (!System.IO.File.Exists(graphExportPath))
                    {
                        Button fixButton = new Button(() =>
                        {
                            settings.SetExporter();
                            graph.Export(settings, path, guid);
                            EditorUtility.SetDirty(settings);
                            AssetDatabase.Refresh();
                        }) {text = "Fix"};
                        graphContainer.Add(fixButton);
                        MessageBox messageBox = new MessageBox(MessageBoxType.Error, "Graph JSON has not been exported for " + graphName);
                        root.Add(messageBox);
                    }
                    else
                    {
                        Button graphButton = new Button(() =>
                        {
                            EditorGUIUtility.PingObject(graph);
                            //Selection.activeObject = graph;
                        }) {text = "Select Graph"};
                        graphContainer.Add(graphButton);
                        Button graphExportButton = new Button(() =>
                        {
                            EditorGUIUtility.PingObject(graphExport);
                            //Selection.activeObject = graphExport;
                        }) {text = "Select JSON"};
                        graphContainer.Add(graphExportButton);
                    }
                }
            }
        }
        
        private void ExportAllBehaveGraph(NPBehaveGraphSettings settings)
        {
            settings.SetExporter();
            string assetGraphPath = settings.graphPath.Substring(settings.graphPath.IndexOf("Assets", StringComparison.Ordinal));
            string[] graphGuid     = AssetDatabase.FindAssets("t:NPBehaveGraph", new []{ assetGraphPath });
            foreach (string guid in graphGuid)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                try
                {
                    Debug.Log($"Export Graph: {path}");
                    var graph = AssetDatabase.LoadAssetAtPath<NPBehaveGraph>(path);
                
                    if (graph != null)
                    {
                        graph.Export(settings, path, guid);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Export Graph {path} Error: {e.Message}");
                    throw;
                }
            }
            EditorUtility.SetDirty(settings);
            AssetDatabase.Refresh();
        }
    }
}
