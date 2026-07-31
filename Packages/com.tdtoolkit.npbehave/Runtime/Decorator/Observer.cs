namespace NPBehave
{
    public class Observer : Decorator
    {
        private readonly System.Action       m_OnStart;
        private readonly System.Action<bool> m_OnStop;

        public Observer(System.Action onStart, System.Action<bool> onStop, Node decorate) : base("Observer", decorate)
        {
            this.m_OnStart = onStart;
            this.m_OnStop = onStop;
        }

        protected override void DoStart()
        {
            this.m_OnStart();
            this.Decorate.Start();
        }

        protected override void DoStop()
        {
            this.Decorate.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            this.m_OnStop(result);
            this.Stopped(result);
        }
    }
}