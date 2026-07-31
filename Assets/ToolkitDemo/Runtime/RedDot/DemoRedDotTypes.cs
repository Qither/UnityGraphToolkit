using RedDotSystem.Runtime;
using System;
using System.Collections.Generic;

namespace ToolkitDemo.RedDotDemo
{
    public static class DemoRedDotState
    {
        public static int PrimaryCount;

        public static int SecondaryCount;
    }

    [Serializable]
    public sealed class DemoMultiRedDotData : RedDotMultiData
    {
        public DemoMultiRedDotData()
        {
        }

        public DemoMultiRedDotData(string nodeName, RedDotData preData, int system,
            List<RedDotData> nextData, LinkedList<RedDotRule> rules)
            : base(nodeName, preData, system, nextData, rules)
        {
        }

        protected override IList<string> GetNodeNames()
        {
            return new[] { "ALPHA", "BETA" };
        }
    }

    [Serializable]
    public sealed class DemoPassThroughRule : RedDotRule
    {
        protected override bool IsExecute()
        {
            return true;
        }
    }
}
