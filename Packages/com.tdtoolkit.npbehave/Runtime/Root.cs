namespace NPBehave
{
    public class Root : Decorator
    {
        private readonly Node m_MainNode;

        //private Node inProgressNode;

        private readonly Blackboard m_Blackboard;
        public override  Blackboard Blackboard => this.m_Blackboard;


        private readonly Clock m_Clock;
        public override  Clock Clock => this.m_Clock;

#if UNITY_EDITOR
        public int TotalNumStartCalls = 0;
        public int TotalNumStopCalls = 0;
        public int TotalNumStoppedCalls = 0;
#endif

        public Root(Node mainNode, Clock clock, NPRootNodeData.SharedValueDictionary blackboardData) : base("Root", mainNode)
        {
            this.m_MainNode = mainNode;
            this.m_Clock = clock;
            this.m_Blackboard = new Blackboard(this.m_Clock, blackboardData);
            this.SetRoot(this);
        }

        public Root(Blackboard blackboard, Clock clock, Node mainNode) : base("Root", mainNode)
        {
            this.m_Blackboard = blackboard;
            this.m_MainNode = mainNode;
            this.m_Clock = clock;
            this.SetRoot(this);
        }

        public sealed override void SetRoot(Root rootNode)
        {
            Log.AssertAreEqual(this, rootNode);
            base.SetRoot(rootNode);
            this.m_MainNode.SetRoot(rootNode);
        }


        protected override void DoStart()
        {
            this.m_Blackboard.Enable();
            this.m_MainNode.Start();
        }

        protected override void DoStop()
        {
            if (this.m_MainNode.IsActive)
            {
                this.m_MainNode.Stop();
            }
            else
            {
                this.m_Clock.RemoveTimer(this.m_MainNode.Start);
            }
        }


        protected override void DoChildStopped(Node node, bool success)
        {
            if (!this.IsStopRequested)
            {
                // wait one tick, to prevent endless recursions
                this.m_Clock.AddTimer(0, 0, this.m_MainNode.Start);
            }
            else
            {
                this.m_Blackboard.Disable();
                this.Stopped(success);
            }
        }
    }
}
