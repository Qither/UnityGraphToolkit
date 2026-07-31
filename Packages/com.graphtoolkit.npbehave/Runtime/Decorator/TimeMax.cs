namespace NPBehave
{
    public class TimeMax : Decorator
    {
        private readonly float m_Limit;
        private readonly float m_RandomVariation;
        private readonly bool  m_WaitForChildButFailOnLimitReached;
        private          bool  m_IsLimitReached;

        public TimeMax(float limit, bool waitForChildButFailOnLimitReached, Node decorate) : base("TimeMax", decorate)
        {
            this.m_Limit = limit;
            this.m_RandomVariation = limit * 0.05f;
            this.m_WaitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
            Log.AssertIsTrue(limit > 0f, "limit has to be set");
        }

        public TimeMax(float limit, float randomVariation, bool waitForChildButFailOnLimitReached, Node decorate) : base("TimeMax", decorate)
        {
            this.m_Limit = limit;
            this.m_RandomVariation = randomVariation;
            this.m_WaitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
            Log.AssertIsTrue(limit > 0f, "limit has to be set");
        }

        protected override void DoStart()
        {
            this.m_IsLimitReached = false;
            this.Clock.AddTimer(this.m_Limit, this.m_RandomVariation, 0, this.TimeoutReached);
            this.Decorate.Start();
        }

        protected override void DoStop()
        {
            this.Clock.RemoveTimer(this.TimeoutReached);
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
            this.Clock.RemoveTimer(this.TimeoutReached);
            this.Stopped(!this.m_IsLimitReached && result);
        }

        private void TimeoutReached()
        {
            if (!this.m_WaitForChildButFailOnLimitReached)
            {
                this.Decorate.Stop();
            }
            else
            {
                this.m_IsLimitReached = true;
                Log.AssertIsTrue(this.Decorate.IsActive);
            }
        }
    }
}