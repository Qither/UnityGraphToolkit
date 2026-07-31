namespace NPBehave
{
    public class TimeMin : Decorator
    {
        private readonly float m_Limit;
        private readonly float m_RandomVariation;
        private readonly bool  m_WaitOnFailure;
        private          bool  m_IsLimitReached;
        private          bool  m_IsDecorateDone;
        private          bool  m_IsDecorateSuccess;

        public TimeMin(float limit, Node decorate) : base("TimeMin", decorate)
        {
            this.m_Limit = limit;
            this.m_RandomVariation = this.m_Limit * 0.05f;
            this.m_WaitOnFailure = false;
            Log.AssertIsTrue(limit > 0f, "limit has to be set");
        }

        public TimeMin(float limit, bool waitOnFailure, Node decorate) : base("TimeMin", decorate)
        {
            this.m_Limit = limit;
            this.m_RandomVariation = this.m_Limit * 0.05f;
            this.m_WaitOnFailure = waitOnFailure;
            Log.AssertIsTrue(limit > 0f, "limit has to be set");
        }

        public TimeMin(float limit, float randomVariation, bool waitOnFailure, Node decorate) : base("TimeMin", decorate)
        {
            this.m_Limit = limit;
            this.m_RandomVariation = randomVariation;
            this.m_WaitOnFailure = waitOnFailure;
            Log.AssertIsTrue(limit > 0f, "limit has to be set");
        }

        protected override void DoStart()
        {
            this.m_IsDecorateDone    = false;
            this.m_IsDecorateSuccess = false;
            this.m_IsLimitReached     = false;
            this.Clock.AddTimer(this.m_Limit, this.m_RandomVariation, 0, this.TimeoutReached);
            this.Decorate.Start();
        }

        protected override void DoStop()
        {
            if (this.Decorate.IsActive)
            {
                this.Clock.RemoveTimer(this.TimeoutReached);
                this.m_IsLimitReached = true;
                this.Decorate.Stop();
            }
            else
            {
                this.Clock.RemoveTimer(this.TimeoutReached);
                this.Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            this.m_IsDecorateDone    = true;
            this.m_IsDecorateSuccess = result;
            if (this.m_IsLimitReached || (!result && !this.m_WaitOnFailure))
            {
                this.Clock.RemoveTimer(this.TimeoutReached);
                this.Stopped(this.m_IsDecorateSuccess);
            }
            else
            {
                Log.AssertIsTrue(this.Clock.HasTimer(this.TimeoutReached));
            }
        }

        private void TimeoutReached()
        {
            this.m_IsLimitReached = true;
            if (this.m_IsDecorateDone)
            {
                this.Stopped(this.m_IsDecorateSuccess);
            }
            else
            {
                Log.AssertIsTrue(this.Decorate.IsActive);
            }
        }
    }
}