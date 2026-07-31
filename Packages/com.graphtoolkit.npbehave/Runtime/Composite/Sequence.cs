namespace NPBehave
{
    public class Sequence : Composite
    {
        private int m_CurrentIndex = -1;

        public Sequence(params Node[] children) : base("Sequence", children)
        {
        }

        protected override void DoStart()
        {
            foreach (Node child in this.Children)
            {
                Log.AssertAreEqual(child.CurrentState, State.Inactive);
            }

            this.m_CurrentIndex = -1;

            this.ProcessChildren();
        }

        protected override void DoStop()
        {
            this.Children[this.m_CurrentIndex].Stop();
        }


        protected override void DoChildStopped(Node child, bool result)
        {
            if (result)
            {
                this.ProcessChildren();
            }
            else
            {
                this.Stopped(false);
            }
        }

        private void ProcessChildren()
        {
            if (++this.m_CurrentIndex < this.Children.Length)
            {
                if (this.IsStopRequested)
                {
                    this.Stopped(false);
                }
                else
                {
                    this.Children[this.m_CurrentIndex].Start();
                }
            }
            else
            {
                this.Stopped(true);
            }
        }

        public override void StopLowerPriorityChildrenForChild(Node abortForChild, bool immediateRestart)
        {
            int indexForChild = 0;
            bool found = false;
            foreach (Node currentChild in this.Children)
            {
                if (currentChild == abortForChild)
                {
                    found = true;
                }
                else if (!found)
                {
                    indexForChild++;
                }
                else if (currentChild.IsActive)
                {
                    if (immediateRestart)
                    {
                        this.m_CurrentIndex = indexForChild - 1;
                    }
                    else
                    {
                        this.m_CurrentIndex = this.Children.Length;
                    }
                    currentChild.Stop();
                    break;
                }
            }
        }

        public override string ToString()
        {
            return base.ToString() + "[" + this.m_CurrentIndex + "]";
        }
    }
}