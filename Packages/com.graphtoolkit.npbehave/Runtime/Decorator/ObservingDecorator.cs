namespace NPBehave
{
    public abstract class ObservingDecorator : Decorator
    {
        protected Stops StopsOnChange;
        private   bool  m_IsObserving;

        public ObservingDecorator(string name, Stops stopsOnChange, Node decorate) : base(name, decorate)
        {
            this.StopsOnChange = stopsOnChange;
            this.m_IsObserving = false;
        }

        protected override void DoStart()
        {
            if (this.StopsOnChange != Stops.None)
            {
                if (!this.m_IsObserving)
                {
                    this.m_IsObserving = true;
                    this.StartObserving();
                }
            }

            if (!this.IsConditionMet())
            {
                this.Stopped(false);
            }
            else
            {
                this.Decorate.Start();
            }
        }

        protected override void DoStop()
        {
            this.Decorate.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Log.AssertAreNotEqual(this.CurrentState, State.Inactive);
            if (this.StopsOnChange == Stops.None || this.StopsOnChange == Stops.Self)
            {
                if (this.m_IsObserving)
                {
                    this.m_IsObserving = false;
                    this.StopObserving();
                }
            }

            this.Stopped(result);
        }

        protected override void DoParentCompositeStopped(Composite parentComposite)
        {
            if (!this.m_IsObserving) return;
            this.m_IsObserving = false;
            this.StopObserving();
        }

        protected void Evaluate()
        {
            switch (this.IsActive)
            {
                case true when !this.IsConditionMet():
                {
                    if (this.StopsOnChange == Stops.Self || this.StopsOnChange == Stops.Both || this.StopsOnChange == Stops.ImmediateRestart)
                    {
                        // Debug.Log( this.key + " stopped self ");
                        this.Stop();
                    }

                    break;
                }
                case false when this.IsConditionMet():
                {
                    if (this.StopsOnChange == Stops.LowerPriority || this.StopsOnChange == Stops.Both || this.StopsOnChange == Stops.ImmediateRestart || this.StopsOnChange == Stops.LowerPriorityImmediateRestart)
                    {
                        // Debug.Log( this.key + " stopped other ");
                        Container parentNode = this.ParentNode;
                        Node      childNode  = this;
                        while (parentNode != null && !(parentNode is Composite))
                        {
                            childNode  = parentNode;
                            parentNode = parentNode.ParentNode;
                        }
                        Log.AssertIsNotNull(parentNode, "NTBtrStops is only valid when attached to a parent composite");
                        Log.AssertIsNotNull(childNode);
                        if (parentNode is Parallel)
                        {
                            Log.AssertIsTrue(this.StopsOnChange == Stops.ImmediateRestart, "On Parallel Nodes all children have the same priority, thus Stops.LOWER_PRIORITY or Stops.BOTH are unsupported in this context!");
                        }

                        if (this.StopsOnChange == Stops.ImmediateRestart || this.StopsOnChange == Stops.LowerPriorityImmediateRestart)
                        {
                            if (this.m_IsObserving)
                            {
                                this.m_IsObserving = false;
                                this.StopObserving();
                            }
                        }

                        ((Composite)parentNode)?.StopLowerPriorityChildrenForChild(childNode, this.StopsOnChange == Stops.ImmediateRestart || this.StopsOnChange == Stops.LowerPriorityImmediateRestart);
                    }

                    break;
                }
            }
        }

        protected abstract void StartObserving();

        protected abstract void StopObserving();

        protected abstract bool IsConditionMet();

    }
}