using GraphProcessor;
using Newtonsoft.Json;
using NPBehave;
using GraphToolkit.Inspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace NPBehaveEditor
{
    public class NPBehaveGraph : BaseGraph
    {
        [SerializeReference]
        [Reference]
        public IBehaveGraphConfig Config;

        [NonSerialized]
        public NPBehaveData BehaveData;

        [NonSerialized]
        public string Json;

        public NPBehaveData GetBehaveData(string graphPath)
        {
            this.BehaveData = new NPBehaveData
            {
                id = this.name
            };
            this.NodeDataSort();
            this.BehaveData.args = this.Config?.BehaveArgsExport(this.BehaveData, graphPath);
            return this.BehaveData;
        }

        public void ExportTest(NPBehaveGraphSettings settings, string path)
        {
            NPBehaveJsonSerializer serializer = CreateSerializer(settings);
            string json = serializer.Serialize(this.GetBehaveData(path), Formatting.Indented);
            serializer.Deserialize(json);
        }

        public void Export(NPBehaveGraphSettings settings, string graphPath, string guid)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            IBehaveGraphExport exporter = GetExporter(settings);
            this.Json = null;
            exporter.OnBeforeExport(this);
            this.GetBehaveData(graphPath);

            string exportDirectory = ToAbsoluteProjectPath(settings.graphExportPath);
            string configPath = Path.Combine(exportDirectory, this.name + ".json");

            try
            {
                string sourcePath = ToAbsoluteProjectPath(graphPath);
                string hashString = ComputeFileHash(sourcePath);
                if (settings.graphCRCDictionary.ContainsKey(guid) &&
                    !string.IsNullOrEmpty(hashString) &&
                    settings.graphCRCDictionary[guid] == hashString &&
                    File.Exists(configPath))
                {
                    return;
                }

                NPBehaveJsonSerializer serializer = new NPBehaveJsonSerializer(exporter);
                this.Json = serializer.Serialize(this.BehaveData);
                Directory.CreateDirectory(exportDirectory);
                File.WriteAllText(configPath, this.Json, new UTF8Encoding(false));
                settings.graphCRCDictionary[guid] = hashString;
            }
            catch (System.Exception exception)
            {
                exporter.OnExportFailed(this, exception.Message);
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                }

                throw;
            }

            exporter.OnAfterExport(this);
        }

        private static NPBehaveJsonSerializer CreateSerializer(NPBehaveGraphSettings settings)
        {
            return new NPBehaveJsonSerializer(GetExporter(settings));
        }

        private static IBehaveGraphExport GetExporter(NPBehaveGraphSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            settings.Exporter ??= new DefaultBehaveGraphExport();
            return settings.Exporter;
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

        private static string ComputeFileHash(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            using MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(File.ReadAllBytes(path));
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private void NodeDataSort()
        {
            this.BehaveData.allNode.Clear();
            List<ANPNodeBase> graphNodes = new List<ANPNodeBase>();
            foreach (BaseNode baseNode in this.nodes)
            {
                if (baseNode is ANPNodeBase node)
                {
                    graphNodes.Add(node);
                }
            }

            graphNodes.Sort((left, right) =>
            {
                int vertical = -left.position.y.CompareTo(right.position.y);
                return vertical != 0 ? vertical : left.position.x.CompareTo(right.position.x);
            });

            int order = 0;
            foreach (ANPNodeBase node in graphNodes)
            {
                node.GetNodeData().id = order++;
            }

            foreach (ANPNodeBase node in graphNodes)
            {
                ANPNodeDataBase nodeData = node.GetNodeData();
                nodeData.linkedIds.Clear();

                List<ANPNodeBase> linkedNodes = new List<ANPNodeBase>();
                foreach (BaseNode linkedNode in node.GetOutputNodes())
                {
                    if (linkedNode is ANPNodeBase child)
                    {
                        linkedNodes.Add(child);
                    }
                }

                linkedNodes.Sort((left, right) =>
                {
                    int horizontal = left.position.x.CompareTo(right.position.x);
                    return horizontal != 0 ? horizontal : left.position.y.CompareTo(right.position.y);
                });

                foreach (ANPNodeBase child in linkedNodes)
                {
                    nodeData.linkedIds.Add(child.GetNodeData().id);
                }

                if (nodeData.linkedIds.Count == 0)
                {
                    Type nodeType = node.GetType();
                    if (nodeType == typeof(NPSequenceNode) || nodeType == typeof(NPRootNode) ||
                        nodeType == typeof(NPParallelNode) || nodeType == typeof(NPCustomRepeaterNode) ||
                        nodeType == typeof(NPSuccessNode) || nodeType == typeof(NPRepeaterNodeData))
                    {
                        Log.Error($"[{this.BehaveData.id}] Cannot export {nodeType.Name} without a child node.");
                    }
                }

                this.BehaveData.allNode.Add(nodeData.id, nodeData);
            }
        }
    }
}
