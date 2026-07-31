using System;

namespace NPBehave
{
    /// <summary>
    /// 黑板多条件节点配置
    /// </summary>
    [Serializable]
    public class NPBlackboardMultipleConditionsNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private BlackboardMultipleConditions m_BlackboardMultipleConditions;

        
        [Serializable]public class MatchInfoList : SerializeReferenceList<MatchInfo> { }

        /// <summary>
        /// 逻辑类型
        /// </summary>
        public MatchType matchType = MatchType.And;

        /// <summary>
        /// 终止条件
        /// </summary>
        public Stops stop = Stops.ImmediateRestart;
        
        /// <summary>
        /// 对比内容
        /// </summary>
#if UNITY_EDITOR
        [SerializeList(AddItem = nameof(AddItem))]
#endif
        public MatchInfoList matchInfos = new MatchInfoList();

#if UNITY_EDITOR
        [NonSerialized] public NPRootNodeData RootNodeData;
        
        private object AddItem()
        {
            MatchInfo info = new MatchInfo
            {
                blackBoardHandleData =
                {
                    RootNodeDataData = this.RootNodeData
                }
            };

            return info;
        }
#endif

        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.m_BlackboardMultipleConditions = new BlackboardMultipleConditions(this.matchInfos, this.matchType, this.stop, node);
            //此处的value参数可以随便设，因为我们在游戏中这个value是需要动态改变的
            return this.m_BlackboardMultipleConditions;
        }

        public override Node GetNode()
        {
            return this.m_BlackboardMultipleConditions;
        }
    }
}