using NPBehave;
using System;
using UnityEditor;
using UnityEngine;
using NPBehaveEditor;
using GraphToolkit.Inspector.Editor;
using System.Reflection;

namespace NPBehaveEditor
{

    public class NPBehaveMenu
    {
        [MenuItem("Tools/NPBehave/FixAll")]
        public static void FixAllBehaveNode()
        {
            string[] graphSettingsGuid = AssetDatabase.FindAssets("t:NPBehaveGraphSettings");
            if (graphSettingsGuid.Length <= 0)
            {
                return;
            }

            string                settingsPath  = AssetDatabase.GUIDToAssetPath(graphSettingsGuid[0]);
            NPBehaveGraphSettings graphSettings = AssetDatabase.LoadAssetAtPath<NPBehaveGraphSettings>(settingsPath);
            
            string assetActionPath = graphSettings.actionPath.Substring(graphSettings.actionPath.IndexOf("Assets", StringComparison.Ordinal));
            string assetActionNodePath = graphSettings.actionNodePath.Substring(graphSettings.actionNodePath.IndexOf("Assets", StringComparison.Ordinal));
            
            string[] actionScriptGuid     = AssetDatabase.FindAssets("t:script", new []{ assetActionPath });
            foreach (string guid in actionScriptGuid)
            {
                string        path        = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null)
                {
                    var   actionName  = script.name;
                    var actionNodeName = actionName + "Node";
                    
                    var actionNodeScript = AssetDatabase.LoadAssetAtPath<MonoScript>(assetActionNodePath + "/" + actionNodeName + ".cs");
                    if (actionNodeScript == null)
                    {
                        Type type = script.GetClass();
                        string labelName = type?.GetCustomAttribute<NodeMenuLabelAttribute>()?.Label ?? actionName.Substring(2);
                        var actionNodeTemplateText = graphSettings.actionNodeTemplate.text;
                        
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

                        var actionNodeTemplatePath = graphSettings.actionNodePath + "/" + actionNodeName + ".cs";
                        System.IO.File.WriteAllText(actionNodeTemplatePath, actionNodeTemplateText);
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"Fix All Node Done!");
        }
        
        // [MenuItem("Tools/NPBehave/ExportGraph")]
        public static void ExportAllBehaveGraph()
        {
            string[] graphSettingsGuid = AssetDatabase.FindAssets("t:NPBehaveGraphSettings");
            if (graphSettingsGuid.Length <= 0)
            {
                return;
            }
            
            string                settingsPath  = AssetDatabase.GUIDToAssetPath(graphSettingsGuid[0]);
            NPBehaveGraphSettings graphSettings = AssetDatabase.LoadAssetAtPath<NPBehaveGraphSettings>(settingsPath);

            string   assetGraphPath = graphSettings.graphPath.Substring(graphSettings.graphPath.IndexOf("Assets", StringComparison.Ordinal));
            string[] allGraphGuid   = AssetDatabase.FindAssets("t:NPBehaveGraph", new[] { assetGraphPath });
            graphSettings.SetExporter();

            foreach (string guid in allGraphGuid)
            {
                string        path        = AssetDatabase.GUIDToAssetPath(guid);
                // Debug.Log($"Export Behave: {path}");
                NPBehaveGraph behaveGraph = AssetDatabase.LoadAssetAtPath<NPBehaveGraph>(path);
                if (behaveGraph != null)
                {
                    behaveGraph.Export(graphSettings, path, guid);
                }
            }
            EditorUtility.SetDirty(graphSettings);
            
            AssetDatabase.Refresh();
            
            Debug.Log($"Export All Graph Done!");
        }
        
        [MenuItem("Tools/NPBehave/Settings")]
        public static void OpenBehaveGraphSettings()
        {
            string[] graphSettingsGuid = AssetDatabase.FindAssets("t:NPBehaveGraphSettings");
            if (graphSettingsGuid.Length <= 0)
            {
                return;
            }

            string                settingsPath  = AssetDatabase.GUIDToAssetPath(graphSettingsGuid[0]);
            NPBehaveGraphSettings graphSettings = AssetDatabase.LoadAssetAtPath<NPBehaveGraphSettings>(settingsPath);
            Selection.activeObject = graphSettings;
        }
        
        [MenuItem("Assets/Create/NPBehaveGraphSettings", false, 10)]
        public static void CreateBehaveGraphAsset()
        {
            string[] graphSettingsGuid = AssetDatabase.FindAssets("t:NPBehaveGraphSettings");
            if (graphSettingsGuid.Length > 0)
            {
                return;
            }

            NPBehaveGraphSettings graphSettings = ScriptableObject.CreateInstance<NPBehaveGraphSettings>();
            ProjectWindowUtil.CreateAsset(graphSettings, "NPBehaveGraphSettings.asset");
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void ReloadScripts()
        {

        }
    }
}
