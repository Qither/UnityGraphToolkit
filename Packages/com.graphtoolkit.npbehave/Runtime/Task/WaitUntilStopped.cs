namespace NPBehave
{
    public class WaitUntilStopped : Task
    {
        private readonly bool m_SuccessWhenStopped;
        public WaitUntilStopped(bool successWhenStopped = false) : base("WaitUntilStopped")
        {
            this.m_SuccessWhenStopped = successWhenStopped;
        }

        protected override void DoStop()
        {
            this.Stopped(this.m_SuccessWhenStopped);
        }
    }
}