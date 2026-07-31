namespace NPBehave
{
    public class Wait : Task
    {
        private readonly System.Func<float> m_Function;
        private readonly string             m_BlackboardKey;
        private readonly float              m_Seconds = -1f;
        private          float              m_RandomVariance;

        public float RandomVariance
        {
            get => this.m_RandomVariance;
            set => this.m_RandomVariance = value;
        }

        public Wait(float seconds, float randomVariance) : base("Wait")
        {
            Log.AssertIsTrue(seconds >= 0);
            this.m_Seconds = seconds;
            this.m_RandomVariance = randomVariance;
        }

        public Wait(float seconds) : base("Wait")
        {
            this.m_Seconds = seconds;
            this.m_RandomVariance = this.m_Seconds * 0.05f;
        }

        public Wait(string blackboardKey, float randomVariance = 0f) : base("Wait")
        {
            this.m_BlackboardKey = blackboardKey;
            this.m_RandomVariance = randomVariance;
        }

        public Wait(System.Func<float> function, float randomVariance = 0f) : base("Wait")
        {
            this.m_Function = function;
            this.m_RandomVariance = randomVariance;
        }

        protected override void DoStart()
        {
            float seconds = this.m_Seconds;
            if (seconds < 0)
            {
                if (this.m_BlackboardKey != null)
                {
                    seconds = this.Blackboard.Get<float>(this.m_BlackboardKey);
                }
                else if (this.m_Function != null)
                {
                    seconds = this.m_Function();
                }
            }

            if (seconds < 0)
            {
                seconds = 0;
            }

            if (this.m_RandomVariance >= 0f)
            {
                this.Clock.AddTimer(seconds, this.m_RandomVariance, 0, this.OnTimer);
            }
            else
            {
                this.Clock.AddTimer(seconds, 0, this.OnTimer);
            }
        }

        protected override void DoStop()
        {
            this.Clock.RemoveTimer(this.OnTimer);
            this.Stopped(false);
        }

        private void OnTimer()
        {
            this.Clock.RemoveTimer(this.OnTimer);
            this.Stopped(true);
        }
    }
}