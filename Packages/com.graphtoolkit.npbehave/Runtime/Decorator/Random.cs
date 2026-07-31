namespace NPBehave
{
    public class Random : Decorator
    {
        private readonly float m_Probability;

        public Random(float probability, Node decorate) : base("Random", decorate)
        {
            this.m_Probability = probability;
        }

        protected override void DoStart()
        {
            if (this.RootNode.Clock.Random <= this.m_Probability)
            {
                this.Decorate.Start();
            }
            else
            {
                this.Stopped(false);
            }
        }

        protected override void DoStop()
        {
            this.Decorate.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            this.Stopped(result);
        }
    }
}