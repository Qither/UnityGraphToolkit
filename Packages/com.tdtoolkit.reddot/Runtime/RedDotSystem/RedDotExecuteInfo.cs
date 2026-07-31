using System;
using System.Linq;
using UnityEngine;

namespace RedDotSystem.Runtime
{
    public class RedDotExecuteInfo
    {
        public bool IsDirty { get; private set; }

        public float ExecuteTimestamp { get; private set; }

        private readonly RedDotExecuteDelegate m_ExecuteDelegate;

        private readonly RedDotMultiData m_MultiData;

        private readonly Func<int, bool> m_IsSystemAvailable;

        private int m_ChildCount;

        public RedDotExecuteInfo(RedDotData data, RedDotExecuteDelegate executeDelegate,
            Func<int, bool> isSystemAvailable)
        {
            this.m_ExecuteDelegate = executeDelegate;
            this.m_IsSystemAvailable = isSystemAvailable ?? (_ => true);
            this.IsDirty = true;
            this.ExecuteTimestamp = float.MaxValue;

            if (data.NextData?.FirstOrDefault() is RedDotMultiData multiData)
            {
                this.m_MultiData = multiData;
                this.m_ChildCount = multiData.ChildNodeNames.Count;
            }
        }

        public void SetDirty()
        {
            this.IsDirty = true;
        }

        public void Execute(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            RedDotData nodeData = node.Data.nodeData;
            if (nodeData.BindSystem > 0 && !this.m_IsSystemAvailable(nodeData.BindSystem))
            {
                node.Data = (nodeData, 0);
                this.MarkClean();
                return;
            }

            if (this.m_MultiData != null)
            {
                this.m_ExecuteDelegate?.Invoke(node);
                if (this.m_ChildCount > 1)
                {
                    this.m_ChildCount--;
                }
                else
                {
                    this.MarkClean();
                    this.m_ChildCount = this.m_MultiData.ChildNodeNames.Count;
                }
            }
            else
            {
                this.m_ExecuteDelegate?.Invoke(node);
                this.MarkClean();
            }
        }

        private void MarkClean()
        {
            this.IsDirty = false;
            this.ExecuteTimestamp = Time.realtimeSinceStartup;
        }
    }
}
