namespace NPBehave
{
    public class Failure : Decorator
    {
        public Failure(Node decorate) : base("Failure", decorate)
        {
        }

        protected override void DoStart()
        {
            this.Decorate.Start();
        }

        protected override void DoStop()
        {
            this.Decorate.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            this.Stopped(false);
        }
    }

}