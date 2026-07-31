using System;

namespace NPBehave
{
    public class WaitForCondition : Decorator
    {
        private readonly Func<bool> m_Condition;
        private readonly float      m_CheckInterval;
        private readonly float      m_CheckVariance;

        public WaitForCondition(Func<bool> condition, float checkInterval, float randomVariance, Node decorate) : base("WaitForCondition", decorate)
        {
            this.m_Condition = condition;

            this.m_CheckInterval = checkInterval;
            this.m_CheckVariance = randomVariance;

            this.Label = "" + (checkInterval - randomVariance) + "..." + (checkInterval + randomVariance) + "s";
        }

        public WaitForCondition(Func<bool> condition, Node decorate) : base("WaitForCondition", decorate)
        {
            this.m_Condition = condition;
            this.m_CheckInterval = 0.0f;
            this.m_CheckVariance = 0.0f;
            this.Label = "every tick";
        }

        protected override void DoStart()
        {
            if (!this.m_Condition.Invoke())
            {
                this.Clock.AddTimer(this.m_CheckInterval, this.m_CheckVariance, -1, this.CheckCondition);
            }
            else
            {
                this.Decorate.Start();
            }
        }

        private void CheckCondition()
        {
            if (!this.m_Condition.Invoke()) return;
            this.Clock.RemoveTimer(this.CheckCondition);
            this.Decorate.Start();
        }

        protected override void DoStop()
        {
            this.Clock.RemoveTimer(this.CheckCondition);
            if (this.Decorate.IsActive)
            {
                this.Decorate.Stop();
            }
            else
            {
                this.Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Log.AssertAreNotEqual(this.CurrentState, State.Inactive);
            this.Stopped(result);
        }
    }
}