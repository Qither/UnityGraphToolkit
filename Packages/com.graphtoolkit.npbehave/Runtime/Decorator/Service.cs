namespace NPBehave
{
    public class Service : Decorator
    {
        private readonly System.Action m_ServiceMethod;

        private readonly float m_Interval = -1.0f;
        private readonly float m_RandomVariation;

        public Service(float interval, float randomVariation, System.Action service, Node decorate) : base("Service", decorate)
        {
            this.m_ServiceMethod = service;
            this.m_Interval = interval;
            this.m_RandomVariation = randomVariation;

            this.Label = "" + (interval - randomVariation) + "..." + (interval + randomVariation) + "s";
        }

        public Service(float interval, System.Action service, Node decorate) : base("Service", decorate)
        {
            this.m_ServiceMethod = service;
            this.m_Interval = interval;
            this.m_RandomVariation = interval * 0.05f;
            this.Label = "" + (interval - this.m_RandomVariation) + "..." + (interval + this.m_RandomVariation) + "s";
        }

        public Service(System.Action service, Node decorate) : base("Service", decorate)
        {
            this.m_ServiceMethod = service;
            this.Label = "every tick";
        }

        protected override void DoStart()
        {
            if (this.m_Interval <= 0f)
            {
                this.Clock.AddUpdateObserver(this.m_ServiceMethod);
                this.m_ServiceMethod();
            }
            else if (this.m_RandomVariation <= 0f)
            {
                this.Clock.AddTimer(this.m_Interval, -1, this.m_ServiceMethod);
                this.m_ServiceMethod();
            }
            else
            {
                this.InvokeServiceMethodWithRandomVariation();
            }

            this.Decorate.Start();
        }

        protected override void DoStop()
        {
            this.Decorate.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            if (this.m_Interval <= 0f)
            {
                this.Clock.RemoveUpdateObserver(this.m_ServiceMethod);
            }
            else if (this.m_RandomVariation <= 0f)
            {
                this.Clock.RemoveTimer(this.m_ServiceMethod);
            }
            else
            {
                this.Clock.RemoveTimer(this.InvokeServiceMethodWithRandomVariation);
            }

            this.Stopped(result);
        }

        private void InvokeServiceMethodWithRandomVariation()
        {
            this.m_ServiceMethod();
            this.Clock.AddTimer(this.m_Interval, this.m_RandomVariation, 0, this.InvokeServiceMethodWithRandomVariation);
        }
    }
}