namespace NPBehave
{
    public class Success : Decorator
    {
        public Success(Node decorate) : base("Success", decorate)
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
            this.Stopped(true);
        }
    }
}