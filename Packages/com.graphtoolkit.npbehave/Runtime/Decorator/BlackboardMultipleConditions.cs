using System;
using System.Linq;

namespace NPBehave
{
    /// <summary>
    /// 条件的匹配类型
    /// </summary>
    public enum MatchType: byte
    {
        /// <summary>
        /// 与
        /// </summary>
        And,

        /// <summary>
        /// 或
        /// </summary>
        Or
    }

    /// <summary>
    /// 匹配信息
    /// </summary>
    [Serializable]
    public class MatchInfo
    { 
        public Operator                 op = Operator.IsEqual;
        public NPBlackBoardHandleData blackBoardHandleData = new NPBlackBoardHandleData();
    }

    /// <summary>
    /// 多条件的黑板条件结点
    /// </summary>
    public class BlackboardMultipleConditions : ObservingDecorator
    {
        private readonly NPBlackboardMultipleConditionsNodeData.MatchInfoList m_MatchInfos;
        private readonly MatchType                                            m_MatchType;

        public BlackboardMultipleConditions(NPBlackboardMultipleConditionsNodeData.MatchInfoList matchInfos, MatchType matchType, Stops stopsOnChange,
        Node decorate): base("BlackboardMultipleConditions",
            stopsOnChange, decorate)
        {
            this.m_MatchInfos = matchInfos;
            this.m_MatchType = matchType;
            this.StopsOnChange = stopsOnChange;
        }

        protected override void StartObserving()
        {
            foreach (MatchInfo matchInfo in this.m_MatchInfos)
            {
                this.RootNode.Blackboard.AddObserver(matchInfo.blackBoardHandleData.blackBoardKey, this.OnValueChanged);
            }
        }

        protected override void StopObserving()
        {
            foreach (MatchInfo matchInfo in this.m_MatchInfos)
            {
                this.RootNode.Blackboard.RemoveObserver(matchInfo.blackBoardHandleData.blackBoardKey, this.OnValueChanged);
            }
        }

        private void OnValueChanged(Blackboard.Type type, ASharedValue newValue)
        {
            this.Evaluate();
        }

        protected override bool IsConditionMet()
        {
            int realMatchCount = this.m_MatchInfos.Count(matchInfo => this.CheckCondition(matchInfo.blackBoardHandleData.blackBoardKey, matchInfo.blackBoardHandleData.blackBoardValue, matchInfo.op));

            return this.m_MatchType switch
            {
                MatchType.Or when realMatchCount >= 1                        => true,
                MatchType.Or                                                 => false,
                MatchType.And when realMatchCount == this.m_MatchInfos.Count => true,
                MatchType.And                                                => false,
                _                                                            => false
            };
        }

        public bool CheckCondition(string key, ASharedValue value, Operator op)
        {
            if (op == Operator.AlwaysTrue)
            {
                return true;
            }

            if (!this.RootNode.Blackboard.IsSet(key))
            {
                return op == Operator.IsNotSet;
            }

            ASharedValue sharedValue = this.RootNode.Blackboard.Get(key);

            return SharedValueHelper.Compare(value, sharedValue, op);
        }
    }
}