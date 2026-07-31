using GraphProcessor;
using RedDotSystem.Editor.Node;
using RedDotSystem.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace RedDotSystem.Editor
{
    public class RedDotExecuteNodeGenerate
    {
        private const string RED_DOT_EXECUTE_NODE_FUNCTION_REGEX = @"public void (\w+)\(TrieNode\<\(RedDotData \w+, int \w+\)\> \w+\)";
        
        private const string RED_DOT_EXECUTE_NODE_USING_REGEX = @"using (\w+(\.\w+)*);";
        
        public const string RED_DOT_NAMES = "RedDotNames";
        
        public const string RED_DOT_MULTI_NAMES = "RedDotMultiNames";

        private const int FILE_MAX_FUNCTION_COUNT = 35;
        
        private readonly RedDotGraph m_RedDotGraph;

        public Dictionary<string, RedDotExecuteNodeCodeInfo> ExecuteNodeCodeInfoMap;

        private Dictionary<int, BaseNode> m_ExecuteNodeMap;
        
        private Dictionary<int, BaseNode> m_ExecuteMultiNodeMap;

        private MonoScript m_RedDotExecuteNodeScript;
        
        private Dictionary<string, MonoScript> m_RedDotExecuteNodeExpansionScriptMap;

        private List<string> m_UsingNameSpaces = new List<string>();
        
        private List<(string name, string desc)> m_RedDotNames      = new List<(string name, string desc)>();
        private List<(string name, string desc)> m_MultiRedDotNames = new List<(string name, string desc)>();

        public RedDotExecuteNodeGenerate(RedDotGraph redDotGraph)
        {
            this.m_RedDotGraph            = redDotGraph;
        }

        public void SetupScriptFile()
        {
            string path = this.m_RedDotGraph.ExecuteNodePath;
            if (string.IsNullOrEmpty(path)) return;
            
            this.ExecuteNodeCodeInfoMap              = new Dictionary<string, RedDotExecuteNodeCodeInfo>();
            this.m_RedDotExecuteNodeExpansionScriptMap = new Dictionary<string, MonoScript>();
            this.m_UsingNameSpaces                     = new List<string>();
            
            string[] guids = AssetDatabase.FindAssets($"t:MonoScript", new[] {path});
            foreach (string guid in guids)
            {
                string filePath      = AssetDatabase.GUIDToAssetPath(guid);

                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);
                if (script != null && !string.IsNullOrEmpty(script.text))
                {
                    Type type = script.GetClass();
                    if (type != null && type.GetInterfaces().Any(i => i == typeof(IRedDotExecuteNode)) &&
                        script.name.Equals(RedDotConst.RED_DOT_EXECUTE_NODE_NAME))
                    {
                        this.m_RedDotExecuteNodeScript = script;
                    }
                    else if (script.name.Contains(RedDotConst.RED_DOT_EXECUTE_NODE_EXPANSION_NAME) ||
                             script.name.Contains(RedDotConst.RED_DOT_EXECUTE_MULTI_NODE_EXPANSION_NAME))
                    {
                        this.m_RedDotExecuteNodeExpansionScriptMap.Add(script.name, script);

                        string   source    = script.text;
                        string[] lines     = source.Split('\n');
                        int      lineCount = 0;

                        string funcName  = string.Empty;
                        int    startLine = 0;

                        int curlyBracesLeft  = 0;
                        int curlyBracesRight = 0;

                        foreach (string code in lines)
                        {
                            lineCount++;

                            if (code.StartsWith("using"))
                            {
                                Regex regex = new Regex(RED_DOT_EXECUTE_NODE_USING_REGEX);
                                Match match = regex.Match(code);
                                if (match.Success)
                                {
                                    string usingNameSpace = match.Groups[1].Value;
                                    if (!string.IsNullOrEmpty(usingNameSpace) &&
                                        !this.m_UsingNameSpaces.Contains(usingNameSpace))
                                    {
                                        this.m_UsingNameSpaces.Add(usingNameSpace);
                                    }
                                }
                            }
                            else if (string.IsNullOrEmpty(funcName))
                            {
                                Regex regex = new Regex(RED_DOT_EXECUTE_NODE_FUNCTION_REGEX);
                                Match match = regex.Match(code);
                                if (match.Success)
                                {
                                    funcName  = match.Groups[1].Value;
                                    startLine = lineCount;
                                }
                            }
                            else
                            {
                                Regex           curlyBracesLeftRegex           = new Regex(@"\{");
                                MatchCollection curlyBracesLeftMatchCollection = curlyBracesLeftRegex.Matches(code);
                                curlyBracesLeft += curlyBracesLeftMatchCollection.Count;

                                Regex           curlyBracesRightRegex           = new Regex(@"\}");
                                MatchCollection curlyBracesRightMatchCollection = curlyBracesRightRegex.Matches(code);
                                curlyBracesRight += curlyBracesRightMatchCollection.Count;

                                if (curlyBracesLeft != 0 && curlyBracesLeft == curlyBracesRight)
                                {
                                    curlyBracesLeft  = 0;
                                    curlyBracesRight = 0;

                                    this.ExecuteNodeCodeInfoMap.Add(funcName,
                                        new RedDotExecuteNodeCodeInfo(script.name, funcName, startLine, lineCount));

                                    funcName  = string.Empty;
                                    startLine = 0;
                                }
                            }
                        }
                    }
                }
            }
            
            this.m_RedDotGraph.UsingNameSpaces = this.m_RedDotGraph.UsingNameSpaces.Union(this.m_UsingNameSpaces).ToList();
            EditorUtility.SetDirty(this.m_RedDotGraph);
            if (this.m_RedDotGraph.Settings != null)
            {
                EditorUtility.SetDirty(this.m_RedDotGraph.Settings);
            }
        }

        public void SetupExecuteNodeMap()
        {
            this.m_ExecuteNodeMap = new Dictionary<int, BaseNode>();
            this.m_ExecuteMultiNodeMap = new Dictionary<int, BaseNode>();
            foreach (BaseNode baseNode in this.m_RedDotGraph.nodes)
            {
                if (!(baseNode is RedDotNode executeNode)) continue;

                if (executeNode.outputPorts.FirstOrDefault(port => port.fieldName.Equals("WordOutput"))?.GetEdges().Count <= 0)
                {
                    this.m_ExecuteNodeMap.Add(executeNode.computeOrder, executeNode);
                }
                else if (executeNode.GetOutputNodes().Any(node => node is RedDotMultiNode))
                {
                    this.m_ExecuteMultiNodeMap.Add(executeNode.computeOrder, executeNode);
                }
            }
        }

        public void Generate()
        {
            if (this.m_RedDotGraph.Settings == null)
            {
                throw new InvalidOperationException("Assign RedDotGraphSettings before generating code.");
            }

            if (string.IsNullOrWhiteSpace(this.m_RedDotGraph.ExecuteNodePath))
            {
                throw new InvalidOperationException("RedDotGraphSettings.executeNodePath cannot be empty.");
            }

            Directory.CreateDirectory(this.m_RedDotGraph.ExecuteNodePath);
            this.SetupScriptFile();
            this.SetupExecuteNodeMap();
            HashSet<string> generatedExpansionPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (this.m_RedDotExecuteNodeScript == null)
            {
                string redDotExecuteNodeScriptPath = Path.Combine(this.m_RedDotGraph.ExecuteNodePath,
                    RedDotConst.RED_DOT_EXECUTE_NODE_NAME + ".cs");
                if (!File.Exists(redDotExecuteNodeScriptPath))
                {
                    WriteGeneratedFile(redDotExecuteNodeScriptPath, this.GetExecuteNodeClassTemplate());
                }
            }

            if (this.m_ExecuteNodeMap.Count > 0)
            {
                int executeNodeCount = Mathf.CeilToInt(this.m_ExecuteNodeMap.Count / (float)FILE_MAX_FUNCTION_COUNT);

                List<int> nodeOrder = this.m_ExecuteNodeMap.Keys.ToList();
                nodeOrder.Sort();
                for (int i = 0; i < executeNodeCount; i++)
                {
                    int take = nodeOrder.Count - i * FILE_MAX_FUNCTION_COUNT;
                    if (take > FILE_MAX_FUNCTION_COUNT)
                    {
                        take = FILE_MAX_FUNCTION_COUNT;
                    }
                    
                    List<int>      curNode = nodeOrder.Skip(i * FILE_MAX_FUNCTION_COUNT).Take(take).ToList();
                    List<BaseNode> nodes   = curNode.Select(order => this.m_ExecuteNodeMap[order]).ToList();
                    string         code    = this.GetExecuteNodePartialClassTemplate(nodes);
                    string expansionPath = Path.Combine(this.m_RedDotGraph.ExecuteNodePath,
                        RedDotConst.RED_DOT_EXECUTE_NODE_EXPANSION_NAME + i + ".cs");
                    WriteGeneratedFile(expansionPath, code);
                    generatedExpansionPaths.Add(ToAssetPath(expansionPath));
                }
            }

            if (this.m_ExecuteMultiNodeMap.Count > 0)
            {
                int executeMultiNodeCount =
                    Mathf.CeilToInt(this.m_ExecuteMultiNodeMap.Count / (float)FILE_MAX_FUNCTION_COUNT);

                List<int> multiNodeOrder = this.m_ExecuteMultiNodeMap.Keys.ToList();
                multiNodeOrder.Sort();
                for (int i = 0; i < executeMultiNodeCount; i++)
                {
                    int take = Math.Min(FILE_MAX_FUNCTION_COUNT,
                        multiNodeOrder.Count - i * FILE_MAX_FUNCTION_COUNT);
                    List<BaseNode> nodes = multiNodeOrder
                        .Skip(i * FILE_MAX_FUNCTION_COUNT)
                        .Take(take)
                        .Select(order => this.m_ExecuteMultiNodeMap[order])
                        .ToList();
                    string         code  = this.GetExecuteNodePartialClassTemplate(nodes);
                    string expansionPath = Path.Combine(this.m_RedDotGraph.ExecuteNodePath,
                        RedDotConst.RED_DOT_EXECUTE_MULTI_NODE_EXPANSION_NAME + i + ".cs");
                    WriteGeneratedFile(expansionPath, code);
                    generatedExpansionPaths.Add(ToAssetPath(expansionPath));
                }
            }

            this.ScanNode();
            string redDotNamesClassTemplate = this.GetRedDotNamesClassTemplate();
            WriteGeneratedFile(
                Path.Combine(this.m_RedDotGraph.ExecuteNodePath, RED_DOT_NAMES + ".cs"),
                redDotNamesClassTemplate);

            string redDotMultiNamesClassTemplate = this.GetRedDotMultiNamesClassTemplate();
            WriteGeneratedFile(
                Path.Combine(this.m_RedDotGraph.ExecuteNodePath, RED_DOT_MULTI_NAMES + ".cs"),
                redDotMultiNamesClassTemplate);

            DeleteObsoleteExpansionFiles(this.m_RedDotGraph.ExecuteNodePath, generatedExpansionPaths);

            Debug.Log("Generate RedDotExecuteNode Success!");
            AssetDatabase.Refresh();
        }

        private string GetExecuteNodeClassTemplate()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetUsingNameSpaces());
            sb.AppendLine($"namespace {this.m_RedDotGraph.ExecuteNodeNameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("    public partial class RedDotExecuteNode : IRedDotExecuteNode");
            sb.AppendLine("    {");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GetExecuteNodePartialClassTemplate(List<BaseNode> nodes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetUsingNameSpaces());
            sb.AppendLine($"namespace {this.m_RedDotGraph.ExecuteNodeNameSpace}");
            sb.AppendLine("{");
            sb.AppendLine("    public partial class RedDotExecuteNode");
            sb.AppendLine("    {");
            foreach (BaseNode baseNode in nodes)
            {
                if (baseNode is RedDotNode executeNode)
                {
                    if (executeNode.outputPorts.FirstOrDefault(port => port.fieldName.Equals("WordOutput"))?.GetEdges().Count <= 0 ||
                        executeNode.GetOutputNodes().Any(node => node is RedDotMultiNode))
                    {
                        sb.AppendLine(this.GetExecuteNodeFunctionTemplate(executeNode));
                    }
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
        
        private string GetExecuteNodeFunctionTemplate(RedDotNode node)
        {
            StringBuilder sb       = new StringBuilder();
            string        funcName = node.GetFunctionName();
            if (!string.IsNullOrEmpty(node.Data.Desc))
            {
                sb.AppendLine("        /// <summary>");
                sb.AppendLine($"        /// {node.Data.Desc}");
                sb.AppendLine("        /// </summary>");
            }
            
            if (this.ExecuteNodeCodeInfoMap.TryGetValue(funcName, out RedDotExecuteNodeCodeInfo codeInfo))
            {
                MonoScript script = this.m_RedDotExecuteNodeExpansionScriptMap[codeInfo.FileName];
                string     source = script.text;
                string[]   lines  = source.Split('\n');
                for (int i = codeInfo.StartLine; i <= codeInfo.EndLine; i++)
                {
                    if(i == codeInfo.EndLine && string.IsNullOrEmpty(lines[i - 1]))
                    {
                        continue;
                    }
                    sb.AppendLine(lines[i - 1].Replace("\r", ""));
                }
            }
            else
            {
                sb.AppendLine($"        public void {funcName}(TrieNode<(RedDotData nodeData, int nodeValue)> node)");
                sb.AppendLine("        {");
                sb.AppendLine("        }");
            }

            return sb.ToString();
        }
        
        private string GetUsingNameSpaces()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string nameSpace in this.m_RedDotGraph.UsingNameSpaces)
            {
                sb.AppendLine($"using {nameSpace};");
            }

            return sb.ToString();
        }

        private void ScanNode(char separator = RedDotConst.RED_DOT_NODE_NAME_SEPARATOR)
        {
            this.m_RedDotNames.Clear();
            this.m_MultiRedDotNames.Clear();
            this.m_RedDotGraph.nodes.ForEach(baseNode =>
            {
                if (!(baseNode is RedDotNode redDotNode)) return;

                string curRedDotName = $"{RedDotConst.RED_DOT_ROOT_NODE_NAME}{separator}{redDotNode.Data.Key.Replace('\n', separator)}";
                if (redDotNode.outputPorts.FirstOrDefault(port => port.fieldName.Equals("WordOutput"))?.GetEdges().Count <= 0)
                {
                    this.m_RedDotNames.Add((curRedDotName, redDotNode.Data.Desc));
                }
                else if (redDotNode.GetOutputNodes().Any(node => node is RedDotMultiNode))
                {
                    this.m_MultiRedDotNames.Add((curRedDotName, redDotNode.Data.Desc));
                }
                else
                {
                    this.m_RedDotNames.Add((curRedDotName, redDotNode.Data.Desc));
                }
            });
        }
        
        private string GetRedDotNamesClassTemplate()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {this.m_RedDotGraph.ExecuteNodeNameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {RED_DOT_NAMES}");
            sb.AppendLine("    {");
            foreach (var item in this.m_RedDotNames)
            {
                if (!string.IsNullOrEmpty(item.desc))
                {
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine($"        /// {item.desc}");
                    sb.AppendLine("        /// </summary>");
                }
                sb.AppendLine($"        public const string {item.name} = nameof({item.name});");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
        
        private string GetRedDotMultiNamesClassTemplate()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"namespace {this.m_RedDotGraph.ExecuteNodeNameSpace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {RED_DOT_MULTI_NAMES}");
            sb.AppendLine("    {");
            foreach (var item in this.m_MultiRedDotNames)
            {
                if (!string.IsNullOrEmpty(item.desc))
                {
                    sb.AppendLine("        /// <summary>");
                    sb.AppendLine($"        /// {item.desc}");
                    sb.AppendLine("        /// </summary>");
                }
                sb.AppendLine($"        public const string {item.name} = nameof({item.name});");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static void WriteGeneratedFile(string path, string content)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string crlf = (content ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", "\r\n");
            File.WriteAllText(path, crlf, new UTF8Encoding(false));
        }

        private static void DeleteObsoleteExpansionFiles(string directory, ISet<string> generatedPaths)
        {
            IEnumerable<string> candidates = Directory
                .GetFiles(directory, RedDotConst.RED_DOT_EXECUTE_NODE_EXPANSION_NAME + "*.cs")
                .Concat(Directory.GetFiles(directory,
                    RedDotConst.RED_DOT_EXECUTE_MULTI_NODE_EXPANSION_NAME + "*.cs"));
            foreach (string candidate in candidates)
            {
                string assetPath = ToAssetPath(candidate);
                if (!generatedPaths.Contains(assetPath))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }
        }

        private static string ToAssetPath(string path)
        {
            string assetPath = FileUtil.GetProjectRelativePath(Path.GetFullPath(path));
            return string.IsNullOrEmpty(assetPath) ? path.Replace('\\', '/') : assetPath;
        }
    }
}
