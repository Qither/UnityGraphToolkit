using System;

namespace NPBehave
{
    /// <summary>
    /// 黑板条件节点配置
    /// </summary>
    [Serializable]
    public class NPBlackboardConditionNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private BlackboardCondition m_BlackboardConditionNode;

        /// <summary>
        /// 运算符号
        /// </summary>
        public Operator op = Operator.IsEqual;

        /// <summary>
        /// 终止条件
        /// </summary>
        public Stops stop = Stops.ImmediateRestart;

        public NPBlackBoardHandleData condition = new NPBlackBoardHandleData() { writeOrCompareToBlackBoard = true };

        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.m_BlackboardConditionNode = new BlackboardCondition(this.condition.blackBoardKey,
                this.op,
                this.condition.blackBoardValue, this.stop, node);
            //此处的value参数可以随便设，因为我们在游戏中这个value是需要动态改变的
            return this.m_BlackboardConditionNode;
        }

        public override Node GetNode()
        {
            return this.m_BlackboardConditionNode;
        }
    }
}