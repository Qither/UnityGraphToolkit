namespace NPBehave
{
    public class Inverter : Decorator
    {
        public Inverter(Node decorate) : base("Inverter", decorate)
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
            this.Stopped(!result);
        }
    }
}