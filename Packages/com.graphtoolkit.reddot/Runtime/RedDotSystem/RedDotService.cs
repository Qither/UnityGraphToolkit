using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Time = UnityEngine.Time;

namespace RedDotSystem.Runtime
{
    public delegate void RedDotExecuteDelegate(TrieNode<(RedDotData nodeData, int nodeValue)> node);

    public delegate void RedDotBindEvent(string key, TrieNode<(RedDotData nodeData, int nodeValue)> node);

    public class RedDotService : IDisposable
    {
        private const float EXECUTE_INTERVAL = 15;

        private Trie<(RedDotData nodeData, int nodeValue)> m_Trie;

        private Dictionary<string, RedDotExecuteInfo> m_ExecuteNodeMap;

        private Dictionary<string, RedDotExecuteValueChangedEventProxy> m_BindNodeMap;

        private IRedDotExecuteNode m_RedDotExecuteNode;

        private List<TrieNode<(RedDotData nodeData, int nodeValue)>> m_AllExecuteNode;

        private Dictionary<string, TrieNode<(RedDotData nodeData, int nodeValue)>> m_CacheBindNodeMap;

        private event RedDotBindEvent BindEvent;

        private List<RedDotBindEvent> m_BindEvents;

        private bool m_IsBindChanged;

        private readonly Func<int, bool> m_IsSystemAvailable;

        public RedDotService(IRedDotExecuteNode executeNode, RedDotData data, Func<int, bool> isSystemAvailable = null)
        {
            if (executeNode == null)
            {
                throw new ArgumentNullException(nameof(executeNode));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.m_Trie = new Trie<(RedDotData nodeData, int nodeValue)>();
            this.m_AllExecuteNode = new List<TrieNode<(RedDotData nodeData, int nodeValue)>>();
            this.m_ExecuteNodeMap = new Dictionary<string, RedDotExecuteInfo>();
            this.m_BindNodeMap = new Dictionary<string, RedDotExecuteValueChangedEventProxy>();

            this.m_CacheBindNodeMap = new Dictionary<string, TrieNode<(RedDotData nodeData, int nodeValue)>>();
            this.m_BindEvents = new List<RedDotBindEvent>();
            this.m_RedDotExecuteNode = executeNode;
            this.m_IsSystemAvailable = isSystemAvailable ?? (_ => true);

            this.CreateNode(data);
        }

        #region RedDotBind

        public void StaticBind(string nodeName, Action<TrieNode<(RedDotData nodeData, int nodeValue)>> onValueChanged)
        {
            if (onValueChanged == null)
            {
                Debug.Log($"RedDotService: StaticUnbind {nodeName} value changed callback cannot be null.");
                return;
            }

            RedDotExecuteValueChangedEventProxy valueChangedEventProxy;
            if (this.m_BindNodeMap.TryGetValue(nodeName, out RedDotExecuteValueChangedEventProxy proxy))
            {
                valueChangedEventProxy = proxy;
                if (valueChangedEventProxy == null)
                {
                    Debug.Log($"RedDotService: StaticBind BindNodeTable not find {nodeName}.");
                    return;
                }

                if (!this.m_CacheBindNodeMap.ContainsKey(nodeName))
                {
                    this.m_CacheBindNodeMap.Add(nodeName, proxy.Node);
                }
            }
            else
            {
                TrieNode<(RedDotData nodeData, int nodeValue)> node = this.m_Trie.GetWordNode(nodeName);
                if (node == null)
                {
                    Debug.Log($"RedDotService: StaticBind non-exist {nodeName}.");
                    return;
                }

                valueChangedEventProxy = new RedDotExecuteValueChangedEventProxy(node);
                this.m_BindNodeMap.Add(nodeName, valueChangedEventProxy);
                if (!this.m_CacheBindNodeMap.ContainsKey(nodeName))
                {
                    this.m_CacheBindNodeMap.Add(nodeName, node);
                }

                this.m_IsBindChanged = true;
            }

            valueChangedEventProxy.AddListener(onValueChanged);
            if (valueChangedEventProxy.Node != null)
            {
                onValueChanged.Invoke(valueChangedEventProxy.Node);
            }
        }

        public void StaticUnbind(string nodeName, Action<TrieNode<(RedDotData nodeData, int nodeValue)>> onValueChanged)
        {
            if (this.m_BindNodeMap.TryGetValue(nodeName, out RedDotExecuteValueChangedEventProxy proxy))
            {
                if (proxy == null)
                {
                    Debug.Log($"RedDotService: StaticUnbind BindNodeTable not find {nodeName}.");
                    return;
                }

                proxy.RemoveListener(onValueChanged);
                if (this.m_CacheBindNodeMap.ContainsKey(nodeName))
                {
                    this.m_CacheBindNodeMap.Remove(nodeName);
                }

                if (proxy.Count != 0) return;

                this.m_BindNodeMap.Remove(nodeName);
                this.m_IsBindChanged = true;
            }
            else
            {
                Debug.Log($"RedDotService: StaticUnbind BindNodeTable non-exist {nodeName}.");
            }
        }

        public void StaticUnbindAll(string nodeName)
        {
            if (this.m_BindNodeMap.TryGetValue(nodeName, out RedDotExecuteValueChangedEventProxy proxy))
            {
                if (proxy == null)
                {
                    Debug.Log($"RedDotService: StaticUnbindAll BindNodeTable not find {nodeName}.");
                    return;
                }

                proxy.RemoveAllListener();
                this.m_BindNodeMap.Remove(nodeName);
                if (this.m_CacheBindNodeMap.ContainsKey(nodeName))
                {
                    this.m_CacheBindNodeMap.Remove(nodeName);
                }

                this.m_IsBindChanged = true;
            }
            else
            {
                Debug.Log($"RedDotService: StaticUnbindAll BindNodeTable non-exist {nodeName}.");
            }
        }

        public void DynamicBind(string nodeName, Action<TrieNode<(RedDotData nodeData, int nodeValue)>> onValueChanged, string args)
        {
            if (onValueChanged == null)
            {
                Debug.Log($"RedDotService: DynamicBind {nodeName} value changed callback cannot be null.");
                return;
            }

            string fullNodeName = nodeName;
            if (!string.IsNullOrEmpty(args))
            {
                string curChildNodeName = args.ToUpper();
                fullNodeName = $"{fullNodeName}{RedDotConst.RED_DOT_NODE_NAME_SEPARATOR}{curChildNodeName}";
            }
            else
            {
                Debug.Log($"RedDotService: DynamicBind {nodeName} args is null.");
                return;
            }

            RedDotExecuteValueChangedEventProxy valueChangedEventProxy;
            if (this.m_BindNodeMap.TryGetValue(fullNodeName, out RedDotExecuteValueChangedEventProxy proxy))
            {
                valueChangedEventProxy = proxy;
                if (valueChangedEventProxy == null)
                {
                    Debug.Log($"RedDotService: DynamicBind BindNodeTable not find {fullNodeName}.");
                    return;
                }

                if (!this.m_CacheBindNodeMap.ContainsKey(fullNodeName))
                {
                    this.m_CacheBindNodeMap.Add(fullNodeName, proxy.Node);
                }
            }
            else
            {
                TrieNode<(RedDotData nodeData, int nodeValue)> node = this.m_Trie.GetWordNode(fullNodeName);
                if (node == null)
                {
                    Debug.Log($"RedDotService: DynamicBind non-exist {fullNodeName}.");
                    return;
                }

                valueChangedEventProxy = new RedDotExecuteValueChangedEventProxy(node);
                this.m_BindNodeMap.Add(fullNodeName, valueChangedEventProxy);
                if (!this.m_CacheBindNodeMap.ContainsKey(fullNodeName))
                {
                    this.m_CacheBindNodeMap.Add(fullNodeName, node);
                }

                this.m_IsBindChanged = true;
            }

            valueChangedEventProxy.AddListener(onValueChanged);
            if (valueChangedEventProxy.Node != null)
            {
                onValueChanged.Invoke(valueChangedEventProxy.Node);
            }
        }

        public void DynamicUnbind(string nodeName, Action<TrieNode<(RedDotData nodeData, int nodeValue)>> onValueChanged, string args)
        {
            if (onValueChanged == null)
            {
                Debug.Log($"RedDotService: DynamicUnbind {nodeName} value changed callback cannot be null.");
                return;
            }

            string fullNodeName = nodeName;
            if (!string.IsNullOrEmpty(args))
            {
                string curChildNodeName = args.ToUpper();
                fullNodeName = $"{fullNodeName}{RedDotConst.RED_DOT_NODE_NAME_SEPARATOR}{curChildNodeName}";
            }
            else
            {
                Debug.Log($"RedDotService: DynamicUnbind {nodeName} args is null.");
                return;
            }

            if (this.m_BindNodeMap.TryGetValue(fullNodeName, out RedDotExecuteValueChangedEventProxy proxy))
            {
                if (proxy == null)
                {
                    Debug.Log($"RedDotService: DynamicUnbind BindNodeTable not find {fullNodeName}.");
                    return;
                }

                proxy.RemoveListener(onValueChanged);
                if (this.m_CacheBindNodeMap.ContainsKey(fullNodeName))
                {
                    this.m_CacheBindNodeMap.Remove(fullNodeName);
                }

                if (proxy.Count != 0) return;

                this.m_BindNodeMap.Remove(fullNodeName);
                this.m_IsBindChanged = true;
            }
            else
            {
                Debug.Log($"RedDotService: DynamicUnbind BindNodeTable non-exist {nodeName}.");
            }
        }

        public void DynamicUnbindAll(string nodeName, string args)
        {
            string fullNodeName = nodeName;

            if (!string.IsNullOrEmpty(args))
            {
                string curChildNodeName = args.ToUpper();
                fullNodeName = $"{fullNodeName}{RedDotConst.RED_DOT_NODE_NAME_SEPARATOR}{curChildNodeName}";
            }
            else
            {
                Debug.Log($"RedDotService: DynamicUnbindAll {nodeName} args is null.");
                return;
            }

            if (this.m_BindNodeMap.TryGetValue(fullNodeName, out RedDotExecuteValueChangedEventProxy proxy))
            {
                if (proxy == null)
                {
                    Debug.Log($"RedDotService: DynamicUnbindAll BindNodeTable not find {fullNodeName}.");
                    return;
                }

                proxy.RemoveAllListener();
                this.m_BindNodeMap.Remove(fullNodeName);
                if (this.m_CacheBindNodeMap.ContainsKey(fullNodeName))
                {
                    this.m_CacheBindNodeMap.Remove(fullNodeName);
                }

                this.m_IsBindChanged = true;
            }
            else
            {
                Debug.Log($"RedDotService: DynamicUnbindAll BindNodeTable non-exist {nodeName}.");
            }
        }

        private void UnbindAll()
        {
            foreach (KeyValuePair<string, RedDotExecuteValueChangedEventProxy> proxy in this.m_BindNodeMap)
            {
                proxy.Value.RemoveAllListener();
            }
            this.m_BindNodeMap.Clear();
            this.m_CacheBindNodeMap.Clear();

            this.m_IsBindChanged = true;
        }

        public void RegisterBindEvent(RedDotBindEvent bindEvent)
        {
            if (bindEvent == null)
            {
                Debug.Log("RedDotService: RegisterBindEvent bindEvent cannot be null.");
                return;
            }

            this.BindEvent += bindEvent;
            this.m_BindEvents.Add(bindEvent);
        }

        public void UnregisterBindEvent(RedDotBindEvent bindEvent)
        {
            if (bindEvent == null)
            {
                Debug.Log("RedDotService: UnregisterBindEvent bindEvent cannot be null.");
                return;
            }

            this.BindEvent -= bindEvent;
            this.m_BindEvents.Remove(bindEvent);
        }

        private void UnregisterAllBindEvent()
        {
            foreach (RedDotBindEvent bindEvent in this.m_BindEvents)
            {
                this.BindEvent -= bindEvent;
            }
            this.m_BindEvents.Clear();
        }

        #endregion

        #region RedDotExecute

        public void ExecuteNode(TrieNode<(RedDotData nodeData, int nodeValue)> executeNode,
            TrieNode<(RedDotData nodeData, int nodeValue)> curNode = null)
        {
            curNode ??= executeNode;
            if (executeNode?.Data.nodeData == null || curNode?.Data.nodeData == null)
            {
                Debug.Log("RedDotService: ExecuteNode Error.");
                return;
            }

            LinkedListNode<RedDotRule> rule = curNode.Data.nodeData.Rules?.First;
            if (rule?.Value == null)
            {
                this.CalculateNodeValue(executeNode, curNode);
            }
            else
            {
                this.ExecuteRule(executeNode, rule);
            }
        }

        private void ExecuteRule(TrieNode<(RedDotData nodeData, int nodeValue)> executeNode, LinkedListNode<RedDotRule> rule)
        {
            rule.Value.Execute(executeNode, orderNode =>
            {
                if (rule.Next != null)
                {
                    this.ExecuteRule(executeNode, rule.Next);
                }
                else
                {
                    this.CalculateNodeValue(executeNode, orderNode);
                }
            });
        }

        private void CalculateNodeValue(TrieNode<(RedDotData nodeData, int nodeValue)> executeNode,
            TrieNode<(RedDotData nodeData, int nodeValue)> curNode)
        {
            if (executeNode?.Data.nodeData == null || curNode?.Data.nodeData == null)
            {
                Debug.Log("RedDotService: CalculateNodeValue Error.");
                return;
            }

            if (curNode.ChildCount == 0)
            {
                this.ExecuteDelegate(curNode);
                return;
            }

            int nodeValue = 0;
            foreach (KeyValuePair<string, TrieNode<(RedDotData nodeData, int nodeValue)>> pair in
                     curNode.ChildNodesMap)
            {
                if (pair.Value?.Data.nodeData == null)
                {
                    Debug.Log("RedDotService: CalculateChildNodeValue Error.");
                    return;
                }

                this.ExecuteNode(executeNode, pair.Value);

                nodeValue += pair.Value.Data.nodeValue;
            }

            curNode.Data = (curNode.Data.nodeData, nodeValue);
        }

        private void ExecuteDelegate(TrieNode<(RedDotData nodeData, int nodeValue)> orderNode)
        {
            RedDotData funcNodeData = orderNode.Data.nodeData;
            if (funcNodeData is RedDotMultiData)
            {
                funcNodeData = funcNodeData.PreData;
            }

            string functionName = GetFunctionName(funcNodeData);
            if (!(this.m_ExecuteNodeMap.TryGetValue(functionName, out RedDotExecuteInfo executeInfo)))
            {
                Debug.Log($"RedDotService: {orderNode.Parent.NodeValue}_{orderNode.NodeValue} ExecuteRule Error.");
                return;
            }

            if (!executeInfo.IsDirty &&
                Time.realtimeSinceStartup - executeInfo.ExecuteTimestamp < EXECUTE_INTERVAL) return;

            executeInfo.Execute(orderNode);
        }

        #endregion

        #region Public

        public TrieNode<(RedDotData nodeData, int nodeValue)> GetNode(string nodeName)
        {
            return this.m_Trie.GetWordNode(nodeName);
        }

        /// <summary>
        /// 置脏并执行刷新
        /// </summary>
        /// <param name="nodeName"></param>
        public void ExecuteNodeDataDirty(string nodeName)
        {
            TrieNode<(RedDotData nodeData, int nodeValue)> node = this.m_Trie.GetWordNode(nodeName);
            string functionName = GetFunctionName(node.Data.nodeData);

            if (this.m_ExecuteNodeMap.TryGetValue(functionName, out RedDotExecuteInfo executeInfo))
            {
                executeInfo.SetDirty();
                this.ExecuteNode(node);
            }
            else
            {
                Debug.LogError($"RedDotService: ExecuteNodeDataDirty {functionName} is not exist.");
            }
        }

        /// <summary>
        /// 查是否有执行节点
        /// </summary>
        /// <returns></returns>
        public bool ExistExecuteNode(string nodeName)
        {
            TrieNode<(RedDotData nodeData, int nodeValue)> node = this.m_Trie.GetWordNode(nodeName);
            string functionName = GetFunctionName(node.Data.nodeData);

            return this.m_ExecuteNodeMap.ContainsKey(functionName);
        }

        public void Update()
        {
            if (this.m_IsBindChanged)
            {
                this.m_IsBindChanged = false;
                this.RefreshExecuteNode();
            }

            if (this.m_CacheBindNodeMap.Count > 0)
            {
                try
                {
                    foreach (KeyValuePair<string, TrieNode<(RedDotData nodeData, int nodeValue)>> pair in this
                                 .m_CacheBindNodeMap)
                    {
                        this.BindEvent?.Invoke(pair.Key, pair.Value);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
                finally
                {
                    this.m_CacheBindNodeMap.Clear();
                }
            }

            this.m_AllExecuteNode.ForEach(node => this.ExecuteNode(node));
        }

        #endregion

        private void RefreshExecuteNode()
        {
            List<TrieNode<(RedDotData nodeData, int nodeValue)>> allExecuteNode =
                new List<TrieNode<(RedDotData nodeData, int nodeValue)>>();

            foreach (RedDotExecuteValueChangedEventProxy nodeProxy in this.m_BindNodeMap.Values)
            {
                if (nodeProxy == null)
                {
                    Debug.Log("RedDotService: RefreshExecuteNode Error.");
                    continue;
                }

                bool isAdd = true;
                foreach (RedDotExecuteValueChangedEventProxy scanProxy in this.m_BindNodeMap.Values)
                {
                    if (nodeProxy == scanProxy) continue;

                    string curNodeName = nodeProxy.Node.GetFullWord();
                    string scanNodeName = scanProxy.Node.GetFullWord();

                    string[] curNodeNameSplit = curNodeName.Split(RedDotConst.RED_DOT_NODE_NAME_SEPARATOR);
                    string[] scanNodeNameSplit = scanNodeName.Split(RedDotConst.RED_DOT_NODE_NAME_SEPARATOR);

                    if (curNodeNameSplit.Length < scanNodeNameSplit.Length) continue;
                    if (curNodeName.StartsWith(scanNodeName, StringComparison.Ordinal) && scanNodeNameSplit.Length < curNodeNameSplit.Length)
                    {
                        isAdd = false;
                        break;
                    }
                }

                if (isAdd)
                {
                    allExecuteNode.Add(nodeProxy.Node);
                }
            }

            this.m_AllExecuteNode.Clear();
            this.m_AllExecuteNode.AddRange(allExecuteNode);
        }

        private void CreateNode(RedDotData data)
        {
            TrieNode<(RedDotData nodeData, int nodeValue)> node = this.m_Trie.AddWord(data.NodeName, (data, 0));
            if (data.Rules != null && data.Rules.Count > 0)
            {
                foreach (RedDotRule redDotRule in data.Rules)
                {
                    redDotRule.Setup(this, node);
                }
            }

            if (data.NextData == null || !data.NextData.Any())
            {
                this.AddExecuteNodeDelegate(data);
                return;
            }

            foreach (RedDotData next in data.NextData)
            {
                switch (next)
                {
                    case RedDotSingeData _:
                        this.CreateNode(next);
                        break;
                    case RedDotMultiData multiData:
                        {
                            this.AddExecuteNodeDelegate(multiData.PreData);
                            foreach (string nodeName in multiData.ChildNodeNames)
                            {
                                string fullNodeName =
                                    $"{multiData.PreData.NodeName}{RedDotConst.RED_DOT_NODE_NAME_SEPARATOR}{nodeName}";
                                this.CreateNode(multiData, fullNodeName);
                            }

                            break;
                        }
                }
            }
        }

        private void AddExecuteNodeDelegate(RedDotData data)
        {
            string funcName = GetFunctionName(data);
            if (this.m_ExecuteNodeMap.ContainsKey(funcName)) return;

            MethodInfo methodInfo = this.m_RedDotExecuteNode?.GetType()
                .GetMethod(funcName, BindingFlags.Public | BindingFlags.Instance);
            if (methodInfo == null)
            {
                Debug.LogError($"RedDotService: AddExecuteNodeDelegate {funcName} is not exist.");
                return;
            }

            RedDotExecuteDelegate executeDelegate = Delegate.CreateDelegate(typeof(RedDotExecuteDelegate),
            this.m_RedDotExecuteNode, methodInfo) as RedDotExecuteDelegate;
            this.m_ExecuteNodeMap.Add(funcName,
                new RedDotExecuteInfo(data, executeDelegate, this.m_IsSystemAvailable));
        }

        private void CreateNode(RedDotData data, string prefixNodeName)
        {
            TrieNode<(RedDotData nodeData, int nodeValue)> node = this.m_Trie.AddWord(prefixNodeName, (data, 0));
            if (data.Rules != null && data.Rules.Any())
            {
                foreach (RedDotRule redDotRule in data.Rules)
                {
                    redDotRule?.Setup(this, node);
                }
            }

            List<RedDotData> allMultiData = data.NextData?.Where(nextNode => nextNode is RedDotMultiData).ToList();
            if (allMultiData == null || !allMultiData.Any()) return;

            foreach (RedDotData curData in allMultiData)
            {
                if (!(curData is RedDotMultiData nextMultiData)) continue;

                foreach (string nodeName in nextMultiData.ChildNodeNames)
                {
                    prefixNodeName = $"{prefixNodeName}{RedDotConst.RED_DOT_NODE_NAME_SEPARATOR}{nodeName}";
                    this.CreateNode(nextMultiData, prefixNodeName);
                }
            }
        }

        private static string GetFunctionName(RedDotData redDotData)
        {
            return redDotData.FunctionName;
        }

        public void Dispose()
        {
            this.UnbindAll();
            // this.m_BindNodeMap.Clear();
            // this.m_CacheBindNodeMap.Clear();
            this.UnregisterAllBindEvent();
            this.m_AllExecuteNode.Clear();
            this.m_ExecuteNodeMap.Clear();
            this.m_IsBindChanged = false;
            this.m_Trie = null;
            this.m_RedDotExecuteNode = null;
            this.m_AllExecuteNode = null;
            this.m_ExecuteNodeMap = null;
            this.m_BindNodeMap = null;
            this.m_CacheBindNodeMap = null;
            this.m_BindEvents = null;
        }
    }
}
