namespace NPBehave
{
    public class RandomSequence : Composite
    {
        private static System.Random s_Rng = new System.Random();


#if UNITY_EDITOR
        public static void DebugSetSeed( int seed )
        {
            s_Rng = new System.Random( seed );
        }
#endif

        private          int   m_CurrentIndex = -1;
        private readonly int[] m_RandomizedOrder;

        public RandomSequence(params Node[] children) : base("Random Sequence", children)
        {
            this.m_RandomizedOrder = new int[children.Length];
            for (int i = 0; i < this.Children.Length; i++)
            {
                this.m_RandomizedOrder[i] = i;
            }
        }

        protected override void DoStart()
        {
            foreach (Node child in this.Children)
            {
                Log.AssertAreEqual(child.CurrentState, State.Inactive);
            }

            this.m_CurrentIndex = -1;

            // Shuffling
            int n = this.m_RandomizedOrder.Length;
            while (n > 1)
            {
                int k = s_Rng.Next(n--);
                (this.m_RandomizedOrder[n], this.m_RandomizedOrder[k]) = (this.m_RandomizedOrder[k], this.m_RandomizedOrder[n]);
            }

            this.ProcessChildren();
        }

        protected override void DoStop()
        {
            this.Children[this.m_RandomizedOrder[this.m_CurrentIndex]].Stop();
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
                    this.Children[this.m_RandomizedOrder[this.m_CurrentIndex]].Start();
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