using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedDotSystem.Runtime
{
    /// <summary>
    /// 前缀树
    /// </summary>
    public class Trie<T>
    {
        /// <summary>
        /// 单词分隔符
        /// </summary>
        public readonly char Separator;

        /// <summary>
        /// 单词数量
        /// </summary>
        public int WorldCount { get; private set; }

        /// <summary>
        /// 树深度
        /// </summary>
        public int TrieDepth { get; private set; }

        /// <summary>
        /// 根节点
        /// </summary>
        public TrieNode<T> RootNode { get; private set; }

        /// <summary>
        /// 单词列表(用于缓存分割结果，优化单个单词判定时重复分割问题)
        /// </summary>
        private readonly List<string> m_WordList;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Trie(char separator = '_')
        {
            this.Separator  = separator;
            this.WorldCount = 0;
            this.TrieDepth  = 0;
            this.m_WordList = new List<string>();
        }

        /// <summary>
        /// 添加单词
        /// </summary>
        /// <param name="word"></param>
        /// <param name="data"></param>
        public TrieNode<T> AddWord(string word, T data)
        {
            if (this.RootNode == null)
            {
                this.RootNode = TrieNode<T>.Create(word, data, null, this, 0);
                return this.RootNode;
            }
            
            this.m_WordList.Clear();
            var words = word.Split(this.Separator);
            this.m_WordList.AddRange(words);
            var length = this.m_WordList.Count;
            var node   = this.RootNode;
            for (int i = 0; i < length; i++)
            {
                var splitWord = this.m_WordList[i];
                if (i == 0 && splitWord == this.RootNode.NodeValue) continue;
                
                var isLast    = i == (length - 1);
                if (!node.ContainWord(splitWord))
                {
                    node = node.AddChildNode(splitWord, data, isLast);
                }
                else
                {
                    node = node.GetChildNode(splitWord);
                    if (isLast)
                    {
                        Debug.Log($"添加重复单词:{word}");
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// 移除指定单词
        /// Note:
        /// 仅当指定单词存在时才能移除成功
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool RemoveWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                Debug.LogError($"不允许移除空单词!");
                return false;
            }

            var wordNode = GetWordNode(word);
            if (wordNode == null)
            {
                Debug.LogError($"找不到单词:{word}的节点信息，移除单词失败!");
                return false;
            }

            if (wordNode.IsRoot)
            {
                Debug.LogError($"不允许删除根节点!");
                return false;
            }

            // 从最里层节点开始反向判定更新和删除
            if (!wordNode.IsTail)
            {
                Debug.LogError($"单词:{word}的节点不是单词节点，移除单词失败!");
                return false;
            }

            // 删除的节点是叶子节点时要删除节点并往上递归更新节点数据
            // 反之只更新标记为非单词节点即可结束
            if (wordNode.ChildCount > 0)
            {
                wordNode.IsTail = false;
                return true;
            }

            wordNode.RemoveFromParent();
            // 网上遍历更新节点信息
            var node = wordNode.Parent;
            while (node is { IsRoot: false })
            {
                // 没有子节点且不是单词节点则直接删除
                if (node.ChildCount == 0 && !node.IsTail)
                {
                    node.RemoveFromParent();
                }

                node = node.Parent;
                // 有子节点则停止往上更新
                if (node.ChildCount > 0)
                {
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取指定字符串的单词节点
        /// Note:
        /// 只有满足每一层且最后一层是单词的节点才算有效单词节点
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public TrieNode<T> GetWordNode(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                Debug.LogError($"无法获取空单词的单次节点!");
                return null;
            }

            // 从最里层节点开始反向判定更新和删除
            var wordArray = word.Split(this.Separator);
            var node      = this.RootNode;

            if (wordArray.Length >= 1 && wordArray[0] != node.NodeValue)
            {
                Debug.LogError($"不存在根节点{wordArray[0]}");
                return null;
            }
            wordArray = wordArray.Skip(1).ToArray();
            foreach (var splitWord in wordArray)
            {
                var childNode = node.GetChildNode(splitWord);
                if (childNode != null)
                {
                    node = childNode;
                }
                else
                {
                    break;
                }
            }

            if (node == null || !node.IsTail)
            {
                Debug.Log($"找不到单词:{word}的单词节点!");
                return null;
            }

            return node;
        }

        /// <summary>
        /// 有按指定单词开头的词语
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool StartWith(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            this.m_WordList.Clear();
            var wordArray = word.Split(Separator);

            if (wordArray.Length >= 1 && wordArray[0] != this.RootNode.NodeValue)
            {
                return false;
            }
            wordArray = wordArray.Skip(1).ToArray();
            this.m_WordList.AddRange(wordArray);
            return FindWord(this.RootNode, this.m_WordList);
        }

        /// <summary>
        /// 查找单词
        /// </summary>
        /// <param name="trieNode"></param>
        /// <param name="wordList"></param>
        /// <returns></returns>
        private bool FindWord(TrieNode<T> trieNode, List<string> wordList)
        {
            if (wordList.Count == 0)
            {
                return true;
            }

            var firstWord = wordList[0];
            if (!trieNode.ContainWord(firstWord))
            {
                return false;
            }

            var childNode = trieNode.GetChildNode(firstWord);
            wordList.RemoveAt(0);
            return FindWord(childNode, wordList);
        }

        /// <summary>
        /// 单词是否存在
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool ContainWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return false;
            }

            this.m_WordList.Clear();
            var wordArray = word.Split(this.Separator);
            
            if (wordArray.Length >= 1 && wordArray[0] != this.RootNode.NodeValue)
            {
                return false;
            }
            wordArray = wordArray.Skip(1).ToArray();
            this.m_WordList.AddRange(wordArray);
            return MatchWord(this.RootNode, this.m_WordList);
        }

        /// <summary>
        /// 匹配单词(单词必须完美匹配)
        /// </summary>
        /// <param name="trieNode"></param>
        /// <param name="wordList"></param>
        /// <returns></returns>
        private bool MatchWord(TrieNode<T> trieNode, List<string> wordList)
        {
            if (wordList.Count == 0)
            {
                return trieNode.IsTail;
            }

            var firstWord = wordList[0];
            if (!trieNode.ContainWord(firstWord))
            {
                return false;
            }

            var childNode = trieNode.GetChildNode(firstWord);
            wordList.RemoveAt(0);
            return MatchWord(childNode, wordList);
        }

        /// <summary>
        /// 获取所有单词列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetWordList()
        {
            return GetNodeWorldList(RootNode, string.Empty);
        }

        /// <summary>
        /// 获取节点单词列表
        /// </summary>
        /// <param name="trieNode"></param>
        /// <param name="preFix"></param>
        /// <returns></returns>
        private List<string> GetNodeWorldList(TrieNode<T> trieNode, string preFix)
        {
            var wordList = new List<string>();
            foreach (var childNodeKey in trieNode.ChildNodesMap.Keys)
            {
                var    childNode = trieNode.ChildNodesMap[childNodeKey];
                string word;
                if (trieNode.IsRoot)
                {
                    word = $"{preFix}{childNodeKey}";
                }
                else
                {
                    word = $"{preFix}{this.Separator}{childNodeKey}";
                }

                if (childNode.IsTail)
                {
                    wordList.Add(word);
                }

                if (childNode.ChildNodesMap.Count > 0)
                {
                    var childNodeWorldList = GetNodeWorldList(childNode, word);
                    wordList.AddRange(childNodeWorldList);
                }
            }

            return wordList;
        }

        /// <summary>
        /// 打印树形节点
        /// </summary>
        public void PrintTreeNodes()
        {
            this.PrintNodes(this.RootNode);
        }

        /// <summary>
        /// 打印节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="depth"></param>
        private void PrintNodes(TrieNode<T> node, int depth = 1)
        {
            var count = 1;
            foreach (var childNode in node.ChildNodesMap)
            {
                Console.Write($"{childNode.Key}({depth}-{count})");
                count++;
            }

            Console.WriteLine();
            foreach (var childNode in node.ChildNodesMap)
            {
                PrintNodes(childNode.Value, depth + 1);
            }
        }
    }
}