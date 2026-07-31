namespace NPBehave
{
    public class Repeater : Decorator
    {
        private readonly int m_LoopCount = -1;
        private          int m_CurrentLoop;

        /// <param name="loopCount">number of times to execute the decorate. Set to -1 to repeat forever, be careful with endless loops!</param>
        /// <param name="decorate">Decorated Node</param>
        public Repeater(int loopCount, Node decorate) : base("Repeater", decorate)
        {
            this.m_LoopCount = loopCount;
        }

        /// <param name="decorate">Decorated Node, repeated forever</param>
        public Repeater(Node decorate) : base("Repeater", decorate)
        {
        }

        protected override void DoStart()
        {
            if (this.m_LoopCount != 0)
            {
                this.m_CurrentLoop = 0;
                this.Decorate.Start();
            }
            else
            {
                this.Stopped(true);
            }
        }

        protected override void DoStop()
        {
            this.Clock.RemoveTimer(this.RestartDecorate);
            
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
            if (result)
            {
                if (this.IsStopRequested || (this.m_LoopCount > 0 && ++this.m_CurrentLoop >= this.m_LoopCount))
                {
                    this.Stopped(true);
                }
                else
                {
                    this.Clock.AddTimer(0, 0, this.RestartDecorate);
                }
            }
            else
            {
                this.Stopped(false);
            }
        }

        protected void RestartDecorate()
        {
            this.Decorate.Start();
        }
    }
}