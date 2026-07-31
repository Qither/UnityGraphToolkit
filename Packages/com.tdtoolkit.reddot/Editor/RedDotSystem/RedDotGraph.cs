using GraphProcessor;
using Newtonsoft.Json;
using RedDotSystem.Editor.Node;
using RedDotSystem.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RedDotSystem.Editor
{
    public class RedDotGraph : BaseGraph
    {
        public RedDotGraphSettings Settings;

        [NonSerialized]
        public RedDotExecuteNodeGenerate RedDotExecuteNodeGenerate;

        public string ExecuteNodePath
        {
            get => this.Settings?.executeNodePath ?? string.Empty;
            set
            {
                if (this.Settings != null)
                {
                    this.Settings.executeNodePath = value;
                }
            }
        }

        public string ExportPath
        {
            get => this.Settings?.exportPath ?? string.Empty;
            set
            {
                if (this.Settings != null)
                {
                    this.Settings.exportPath = value;
                }
            }
        }

        public string ExecuteNodeNameSpace
        {
            get => this.Settings?.executeNodeNamespace ?? "RedDot.Generated";
            set
            {
                if (this.Settings != null)
                {
                    this.Settings.executeNodeNamespace = value;
                }
            }
        }

        public List<string> UsingNameSpaces
        {
            get => this.Settings?.usingNamespaces ?? new List<string>();
            set
            {
                if (this.Settings != null)
                {
                    this.Settings.usingNamespaces = value ?? new List<string>();
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.DiscoverTypes();
            this.RedDotExecuteNodeGenerate = new RedDotExecuteNodeGenerate(this);
            this.RedDotExecuteNodeGenerate.SetupScriptFile();
        }

        public void DiscoverTypes()
        {
            RedDotHelper.RedDotMultiNodeDataTypes.Clear();
            RedDotHelper.RedDotRuleTypes.Clear();

            AddTypes(TypeCache.GetTypesDerivedFrom<RedDotMultiData>(),
                RedDotHelper.RedDotMultiNodeDataTypes);
            AddTypes(TypeCache.GetTypesDerivedFrom<RedDotRule>(), RedDotHelper.RedDotRuleTypes);
        }

        public Type[] GetKnownSerializationTypes()
        {
            this.DiscoverTypes();
            return RedDotHelper.RedDotMultiNodeDataTypes.Values
                .Concat(RedDotHelper.RedDotRuleTypes.Values)
                .Distinct()
                .OrderBy(type => type.FullName, StringComparer.Ordinal)
                .ToArray();
        }

        public void ExportRedDotNode()
        {
            if (this.Settings == null)
            {
                throw new InvalidOperationException("Assign RedDotGraphSettings before exporting.");
            }

            RedDotSingeData rootData = new RedDotSingeData(
                RedDotConst.RED_DOT_ROOT_NODE_NAME,
                null,
                0,
                new List<RedDotData>(),
                null,
                null);

            foreach (BaseNode baseNode in this.nodes)
            {
                if (baseNode is not RedDotNode redDotNode || redDotNode.GetCustomName() != redDotNode.Data.Key)
                {
                    continue;
                }

                RedDotData child = redDotNode.CreateRedDotData(RedDotConst.RED_DOT_ROOT_NODE_NAME);
                if (child != null)
                {
                    rootData.NextData.Add(child);
                }
            }

            RedDotJsonSerializer serializer = new RedDotJsonSerializer(this.GetKnownSerializationTypes());
            string json = serializer.Serialize(rootData, Formatting.Indented);
            serializer.Deserialize(json);

            string exportDirectory = ToAbsoluteProjectPath(this.Settings.exportPath);
            Directory.CreateDirectory(exportDirectory);
            string exportPath = Path.Combine(exportDirectory, this.Settings.exportFileName);
            File.WriteAllText(exportPath, json, new UTF8Encoding(false));

            EditorUtility.SetDirty(this.Settings);
            AssetDatabase.Refresh();
            Debug.Log($"Exported red-dot JSON: {exportPath}");
        }

        private static void AddTypes(IEnumerable<Type> source, IDictionary<string, Type> destination)
        {
            foreach (Type type in source
                         .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition)
                         .OrderBy(type => type.FullName, StringComparer.Ordinal))
            {
                if (!destination.ContainsKey(type.Name))
                {
                    destination.Add(type.Name, type);
                }
                else
                {
                    Debug.LogWarning($"Ignoring duplicate red-dot type name '{type.Name}' ({type.FullName}).");
                }
            }
        }

        private static string ToAbsoluteProjectPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("A project-relative path is required.", nameof(path));
            }

            if (Path.IsPathRooted(path))
            {
                return Path.GetFullPath(path);
            }

            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
            return Path.GetFullPath(Path.Combine(projectRoot ?? string.Empty, path));
        }
    }
}
