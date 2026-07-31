using System;
using System.Collections.Generic;

namespace RedDotSystem.Runtime
{
    [Serializable]
    public class RedDotSingeData : RedDotData
    {
        [Serializable]
        public class LinkDataNames
        {
            public List<string> PreNodeData;
        
            public List<string> NextNodeData;

            public LinkDataNames()
            {
            }
            
            public LinkDataNames(List<string> preNodeData, List<string> nextNodeData)
            {
                this.PreNodeData  = preNodeData;
                this.NextNodeData = nextNodeData;
            }
        }
        
        public LinkDataNames LinkData;

        public RedDotSingeData()
        {
        }

        public RedDotSingeData(string nodeName, RedDotData preData, int system, List<RedDotData> nextNodeData,
            LinkDataNames linkData, LinkedList<RedDotRule> rules) :
            base(nodeName, preData, system,
                nextNodeData, rules)
        {
            this.LinkData = linkData;
        }
    }
}
