namespace NPBehave
{
    public abstract class Node
    {
        public enum State
        {
            Inactive,       //节点未激活
            Active,         //节点激活
            StopRequested,  //节点停止请求
        }

        protected State NodeState = State.Inactive;

        public State CurrentState => this.NodeState;

        public Root RootNode;

        public  Container ParentNode { get; private set; }

        public string Label { get; set; }

        public string Name { get; }

        public virtual Blackboard Blackboard => this.RootNode.Blackboard;

        public virtual Clock Clock => this.RootNode.Clock;

        public bool IsStopRequested => this.NodeState == State.StopRequested;

        public bool IsActive => this.NodeState == State.Active;


        public Node(string name)
        {
            this.Name = name;
        }

        public virtual void SetRoot(Root rootNode)
        {
            this.RootNode = rootNode;
        }

        public void SetParent(Container parent)
        {
            this.ParentNode = parent;
        }

#if UNITY_EDITOR
        public float DebugLastStopRequestAt;
        public float DebugLastStoppedAt;
        public int DebugNumStartCalls;
        public int DebugNumStopCalls;
        public int DebugNumStoppedCalls;
        public bool DebugLastResult;
#endif

        public void Start()
        {
            // Assert.AreEqual(this.currentState, State.INACTIVE, "can only start inactive nodes, tried to start: " + this.Name + "! PATH: " + GetPath());
            Log.AssertAreEqual(this.NodeState, State.Inactive, "can only start inactive nodes");

#if UNITY_EDITOR
            RootNode.TotalNumStartCalls++;
            this.DebugNumStartCalls++;
#endif
            this.NodeState = State.Active;
            this.DoStart();
        }

        /// <summary>
        /// TODO: Rename to "Cancel" in next API-Incompatible version
        /// </summary>
        public void Stop()
        {
            // Assert.AreEqual(this.currentState, State.ACTIVE, "can only stop active nodes, tried to stop " + this.Name + "! PATH: " + GetPath());
            Log.AssertAreEqual(this.NodeState, State.Active, "can only stop active nodes, tried to stop");
            this.NodeState = State.StopRequested;
#if UNITY_EDITOR
            this.RootNode.TotalNumStopCalls++;
            // this.DebugLastStopRequestAt = UnityEngine.Time.time;
            this.DebugNumStopCalls++;
#endif
            this.DoStop();
        }

        protected virtual void DoStart()
        {

        }

        protected virtual void DoStop()
        {

        }


        /// THIS ABSOLUTLY HAS TO BE THE LAST CALL IN YOUR FUNCTION, NEVER MODIFY
        /// ANY STATE AFTER CALLING Stopped !!!!
        protected virtual void Stopped(bool success)
        {
            // Assert.AreNotEqual(this.currentState, State.INACTIVE, "The Node " + this + " called 'Stopped' while in state INACTIVE, something is wrong! PATH: " + GetPath());
            Log.AssertAreNotEqual(this.NodeState, State.Inactive, "Called 'Stopped' while in state INACTIVE, something is wrong!");
            this.NodeState = State.Inactive;
#if UNITY_EDITOR
            this.RootNode.TotalNumStoppedCalls++;
            this.DebugNumStoppedCalls++;
            // this.DebugLastStoppedAt = UnityEngine.Time.time;
            this.DebugLastResult       = success;
#endif
            if (this.ParentNode != null)
            {
                this.ParentNode.ChildStopped(this, success);
            }
        }

        public virtual void ParentCompositeStopped(Composite composite)
        {
            this.DoParentCompositeStopped(composite);
        }

        /// THIS IS CALLED WHILE YOU ARE INACTIVE, IT's MEANT FOR DECORATORS TO REMOVE ANY PENDING
        /// OBSERVERS
        protected virtual void DoParentCompositeStopped(Composite composite)
        {
            /// be careful with this!
        }

        // public Composite ParentComposite
        // {
        //     get
        //     {
        //         if (ParentNode != null && !(ParentNode is Composite))
        //         {
        //             return ParentNode.ParentComposite;
        //         }
        //         else
        //         {
        //             return ParentNode as Composite;
        //         }
        //     }
        // }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.Label) ? (this.Name + "{"+ this.Label+"}") : this.Name;
        }

        protected string GetPath()
        {
            if (this.ParentNode != null)
            {
                return this.ParentNode.GetPath() + "/" + this.Name;
            }
            else
            {
                return this.Name;
            }
        }
    }
}