using System;
using System.Collections.Generic;

namespace RedDotSystem.Runtime
{
    public class RedDotExecuteValueChangedEventProxy
    {
        public readonly TrieNode<(RedDotData nodeData, int nodeValue)> Node;

        private readonly List<Action<TrieNode<(RedDotData nodeData, int nodeValue)>>> m_NodeValueChangedEvent;

        public int Count => this.m_NodeValueChangedEvent.Count;
        
        public RedDotExecuteValueChangedEventProxy(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            this.Node                    = node;
            this.m_NodeValueChangedEvent = new List<Action<TrieNode<(RedDotData nodeData, int nodeValue)>>>();
        }
        
        public void AddListener(Action<TrieNode<(RedDotData nodeData, int nodeValue)>> action)
        {
            if (this.m_NodeValueChangedEvent.Contains(action)) return;

            this.m_NodeValueChangedEvent.Add(action);
            this.Node.OnValueChanged += action;
        }
        
        public void RemoveListener(Action<TrieNode<(RedDotData nodeData, int nodeValue)>> action)
        {
            if (!this.m_NodeValueChangedEvent.Contains(action)) return;

            this.m_NodeValueChangedEvent.Remove(action);
            this.Node.OnValueChanged -= action;
        }
        
        public void RemoveAllListener()
        {
            foreach (Action<TrieNode<(RedDotData nodeData, int nodeValue)>> action in this.m_NodeValueChangedEvent)
            {
                this.Node.OnValueChanged -= action;
            }
            
            this.m_NodeValueChangedEvent.Clear();
        }
    }
}