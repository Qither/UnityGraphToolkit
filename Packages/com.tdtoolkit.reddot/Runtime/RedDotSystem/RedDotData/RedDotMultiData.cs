using System;
using System.Collections.Generic;

namespace RedDotSystem.Runtime
{
    [Serializable]
    public abstract class RedDotMultiData : RedDotData
    {
        [NonSerialized]
        private IList<string> m_NodeNames;

        public IList<string> ChildNodeNames => this.m_NodeNames ??= this.GetNodeNames();

        protected abstract IList<string> GetNodeNames();

        protected RedDotMultiData()
        {
        }

        protected RedDotMultiData(string nodeName, RedDotData preData, int system, List<RedDotData> nextData,
            LinkedList<RedDotRule> rules) : base(nodeName,
            preData, system, nextData, rules)
        {
        }
    }
}
