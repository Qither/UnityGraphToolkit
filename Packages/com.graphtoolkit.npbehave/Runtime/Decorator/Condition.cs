using System;

namespace NPBehave
{
    public class Condition : ObservingDecorator
    {
        private readonly Func<bool> m_Condition;
        private readonly float      m_CheckInterval;
        private readonly float      m_CheckVariance;

        public Condition(Func<bool> condition, Node decorate) : base("Condition", Stops.None, decorate)
        {
            this.m_Condition = condition;
            this.m_CheckInterval = 0.0f;
            this.m_CheckVariance = 0.0f;
        }

        public Condition(Func<bool> condition, Stops stopsOnChange, Node decorate) : base("Condition", stopsOnChange, decorate)
        {
            this.m_Condition = condition;
            this.m_CheckInterval = 0.0f;
            this.m_CheckVariance = 0.0f;
        }

        public Condition(Func<bool> condition, Stops stopsOnChange, float checkInterval, float randomVariance, Node decorate) : base("Condition", stopsOnChange, decorate)
        {
            this.m_Condition = condition;
            this.m_CheckInterval = checkInterval;
            this.m_CheckVariance = randomVariance;
        }

        protected override void StartObserving()
        {
            this.RootNode.Clock.AddTimer(this.m_CheckInterval, this.m_CheckVariance, -1, this.Evaluate);
        }

        protected override void StopObserving()
        {
            this.RootNode.Clock.RemoveTimer(this.Evaluate);
        }

        protected override bool IsConditionMet()
        {
            return this.m_Condition();
        }
    }
}