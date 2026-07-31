using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedDotSystem.Runtime
{
    /// <summary>
    /// TrieNode.cs
    /// 前缀树节点类
    /// </summary>
    public class TrieNode<T>
    {
        /// <summary>
        /// 节点字符串
        /// </summary>
        public string NodeValue { get; private set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public TrieNode<T> Parent { get; private set; }

        /// <summary>
        /// 所属前缀树
        /// </summary>
        public Trie<T> OwnerTree { get; private set; }

        /// <summary>
        /// 节点深度(根节点为0)
        /// </summary>
        public int Depth { get; private set; }

        /// <summary>
        /// 是否是单词节点
        /// </summary>
        public bool IsTail { get; set; }

        /// <summary>
        /// 是否是根节点
        /// </summary>
        public bool IsRoot => Parent == null;

        /// <summary>
        /// 子节点映射
        /// </summary>
        public Dictionary<string, TrieNode<T>> ChildNodesMap
        {
            get;
            private set;
        }

        /// <summary>
        /// 子节点数量
        /// </summary>
        public int ChildCount => this.ChildNodesMap.Count;

        /// <summary>
        /// 关联数据
        /// </summary>
        public T Data
        {
            get => this.m_Data;
            set
            {
                if (this.Data.Equals(value)) return;
                this.m_Data = value;
                this.OnDataChanged();
            }
        }
        private T m_Data;

        public event Action<TrieNode<T>> OnValueChanged; 

        private string m_FullWord = string.Empty;
        
        private TrieNode()
        {
            this.ChildNodesMap = new Dictionary<string, TrieNode<T>>();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="data">关联数据</param>
        /// <param name="parent">父节点</param>
        /// <param name="ownerTree">所属前缀树</param>
        /// <param name="depth">节点深度</param>
        /// <param name="isTail">是否是单词节点</param>
        public static TrieNode<T> Create(string value, T data, TrieNode<T> parent, Trie<T> ownerTree, int depth, bool isTail = false)
        {
            TrieNode<T> node = new TrieNode<T>()
            {
                NodeValue = value,
                Parent    = parent,
                OwnerTree = ownerTree,
                Depth     = depth,
                IsTail    = isTail,
                Data      = data
            };

            return node;
        }

        public void OnDispose()
        {
            this.NodeValue  = null;
            this.Parent     = null;
            this.OwnerTree  = null;
            this.Depth      = 0;
            this.IsTail     = false;
            this.m_FullWord = string.Empty;
            this.ChildNodesMap.Clear();
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="nodeWord"></param>
        /// <param name="data"></param>
        /// <param name="isTail"></param>
        /// <returns></returns>
        public TrieNode<T> AddChildNode(string nodeWord, T data, bool isTail)
        {
            if (this.ChildNodesMap.TryGetValue(nodeWord, out TrieNode<T> node))
            {
                Debug.LogError($"节点字符串:{this.NodeValue}已存在字符串:{nodeWord}的子节点,不重复添加子节点!");
                return node;
            }
            node = Create(nodeWord, data, this, this.OwnerTree, this.Depth + 1, isTail);
            this.ChildNodesMap.Add(nodeWord, node);
            return node;
        }

        /// <summary>
        /// 移除指定子节点
        /// </summary>
        /// <param name="nodeWord"></param>
        /// <returns></returns>
        public bool RemoveChildNodeByWord(string nodeWord)
        {
            var childNode = this.GetChildNode(nodeWord);
            return this.RemoveChildNode(childNode);
        }

        /// <summary>
        /// 移除指定子节点
        /// </summary>
        /// <param name="childNode"></param>
        /// <returns></returns>
        public bool RemoveChildNode(TrieNode<T> childNode)
        {
            if (childNode == null)
            {
                Debug.LogError($"无法移除空节点!");
                return false;
            }
            var realChildNode = GetChildNode(childNode.NodeValue);
            if (realChildNode != childNode)
            {
                Debug.LogError($"移除的子节点单词:{childNode.NodeValue}对象不是同一个,移除子节点失败!");
                return false;
            }

            this.ChildNodesMap.Remove(childNode.NodeValue);

            return true;
        }

        /// <summary>
        /// 当前节点从父节点移除
        /// </summary>
        /// <returns></returns>
        public bool RemoveFromParent()
        {
            if (this.IsRoot)
            {
                Debug.LogError($"当前节点是根节点，不允许从父节点移除，从父节点移除当前节点失败!");
                return false;
            }
            return this.Parent.RemoveChildNode(this);
        }

        /// <summary>
        /// 获取指定字符串的子节点
        /// </summary>
        /// <param name="nodeWord"></param>
        /// <returns></returns>
        public TrieNode<T> GetChildNode(string nodeWord)
        {
            if (!this.ChildNodesMap.TryGetValue(nodeWord, out TrieNode<T> trieNode))
            {
                Debug.LogError($"节点字符串:{this.NodeValue}找不到子节点字符串:{nodeWord},获取子节点失败!");
                return null;
            }
            return trieNode;
        }

        /// <summary>
        /// 是否包含指定字符串的子节点
        /// </summary>
        /// <param name="nodeWord"></param>
        /// <returns></returns>
        public bool ContainWord(string nodeWord)
        {
            return this.ChildNodesMap.ContainsKey(nodeWord);
        }

        /// <summary>
        /// 获取当前节点构成的单词
        /// Note:
        /// 不管当前节点是否是单词节点,都返回从当前节点回溯到根节点拼接的单词
        /// 若当前节点为根节点，则返回根节点的字符串(默认为"Root")
        /// </summary>
        /// <returns></returns>
        public string GetFullWord()
        {
            if (!string.IsNullOrEmpty(this.m_FullWord))
            {
                return this.m_FullWord;
            }
            
            string trieNodeWord = this.NodeValue;
            TrieNode<T> node = this.Parent;
            while (node != null)
            {
                trieNodeWord = $"{node.NodeValue}{this.OwnerTree.Separator}{trieNodeWord}";
                node = node.Parent;
            }
            this.m_FullWord = trieNodeWord;
            return this.m_FullWord;
        }

        protected virtual void OnDataChanged()
        {
            this.OnValueChanged?.Invoke(this);
        }
    }
}
