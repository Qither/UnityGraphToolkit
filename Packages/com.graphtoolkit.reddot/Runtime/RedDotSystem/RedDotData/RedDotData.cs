using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedDotSystem.Runtime
{
    [Serializable]
    public class RedDotData
    {
        public string NodeName;

        public int BindSystem;

        public RedDotData PreData;

        [SerializeReference]
        public List<RedDotData> NextData;

        public LinkedList<RedDotRule> Rules;

        private string m_FunctionName;
        public string FunctionName
        {
            get
            {
                if (null == this.m_FunctionName)
                {
                    this.m_FunctionName = this.NodeName.Remove(0, RedDotConst.RED_DOT_ROOT_NODE_NAME.Length + 1);
                }

                return this.m_FunctionName;
            }
        }

        protected RedDotData()
        {
        }

        protected RedDotData(string nodeName, RedDotData preData, int system, List<RedDotData> nextData, LinkedList<RedDotRule> rules)
        {
            this.NodeName = nodeName;
            this.PreData = preData;
            this.NextData = nextData;
            this.Rules = rules;
            this.BindSystem = system;
        }
    }
}
