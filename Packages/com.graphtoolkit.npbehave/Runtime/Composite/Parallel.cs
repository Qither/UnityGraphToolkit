using System.Collections.Generic;

namespace NPBehave
{
    public class Parallel : Composite
    {
        public enum Policy
        {
            One,
            All,
        }

        // public enum Wait
        // {
        //     NEVER,
        //     ON_FAILURE,
        //     ON_SUCCESS,
        //     BOTH
        // }

        // private Wait waitForPendingChildrenRule;
        private readonly Policy                 m_FailurePolicy;
        private readonly Policy                 m_SuccessPolicy;
        private readonly int                    m_ChildrenCount;
        private          int                    m_RunningCount;
        private          int                    m_SucceededCount;
        private          int                    m_FailedCount;
        private readonly Dictionary<Node, bool> m_ChildrenResults;
        private          bool                   m_SuccessState;
        private          bool                   m_ChildrenAborted;

        public Parallel(Policy successPolicy, Policy failurePolicy, /*Wait waitForPendingChildrenRule,*/ params Node[] children) : base("Parallel", children)
        {
            this.m_SuccessPolicy = successPolicy;
            this.m_FailurePolicy = failurePolicy;
            // this.waitForPendingChildrenRule = waitForPendingChildrenRule;
            this.m_ChildrenCount = children.Length;
            this.m_ChildrenResults = new Dictionary<Node, bool>();
        }

        protected override void DoStart()
        {
            foreach (Node child in this.Children)
            {
                Log.AssertAreEqual(child.CurrentState, State.Inactive);
            }

            this.m_ChildrenAborted       = false;
            this.m_RunningCount   = 0;
            this.m_SucceededCount = 0;
            this.m_FailedCount    = 0;
            foreach (Node child in this.Children)
            {
                this.m_RunningCount++;
                child.Start();
            }
        }

        protected override void DoStop()
        {
            Log.AssertIsTrue(this.m_RunningCount + this.m_SucceededCount + this.m_FailedCount == this.m_ChildrenCount);

            foreach (Node child in this.Children)
            {
                if (child.IsActive)
                {
                    child.Stop();
                }
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            this.m_RunningCount--;
            if (result)
            {
                this.m_SucceededCount++;
            }
            else
            {
                this.m_FailedCount++;
            }
            this.m_ChildrenResults[child] = result;

            bool allChildrenStarted = this.m_RunningCount + this.m_SucceededCount + this.m_FailedCount == this.m_ChildrenCount;
            if (!allChildrenStarted) return;
            if (this.m_RunningCount == 0)
            {
                if (!this.m_ChildrenAborted) // if children got aborted because rule was evaluated previously, we don't want to override the successState 
                {
                    if (this.m_FailurePolicy == Policy.One && this.m_FailedCount > 0)
                    {
                        this.m_SuccessState = false;
                    }
                    else switch (this.m_SuccessPolicy)
                    {
                        case Policy.One when this.m_SucceededCount > 0:
                        case Policy.All when this.m_SucceededCount == this.m_ChildrenCount:
                            this.m_SuccessState = true;
                            break;
                        default:
                            this.m_SuccessState = false;
                            break;
                    }
                }

                this.Stopped(this.m_SuccessState);
            }
            else if (!this.m_ChildrenAborted)
            {
                Log.AssertIsFalse(this.m_SucceededCount == this.m_ChildrenCount);
                Log.AssertIsFalse(this.m_FailedCount == this.m_ChildrenCount);

                if (this.m_FailurePolicy == Policy.One && this.m_FailedCount > 0/* && waitForPendingChildrenRule != Wait.ON_FAILURE && waitForPendingChildrenRule != Wait.BOTH*/)
                {
                    this.m_SuccessState    = false;
                    this.m_ChildrenAborted = true;
                }
                else if (this.m_SuccessPolicy == Policy.One && this.m_SucceededCount > 0/* && waitForPendingChildrenRule != Wait.ON_SUCCESS && waitForPendingChildrenRule != Wait.BOTH*/)
                {
                    this.m_SuccessState    = true;
                    this.m_ChildrenAborted = true;
                }

                if (!this.m_ChildrenAborted) return;
                foreach (Node currentChild in this.Children)
                {
                    if (currentChild.IsActive)
                    {
                        currentChild.Stop();
                    }
                }
            }
        }

        public override void StopLowerPriorityChildrenForChild(Node abortForChild, bool immediateRestart)
        {
            if (immediateRestart)
            {
                Log.AssertIsFalse(abortForChild.IsActive);
                if (this.m_ChildrenResults[abortForChild])
                {
                    this.m_SucceededCount--;
                }
                else
                {
                    this.m_FailedCount--;
                }
                this.m_RunningCount++;
                abortForChild.Start();
            }
            else
            {
                throw new Exception("On Parallel Nodes all children have the same priority, thus the method does nothing if you pass false to 'immediateRestart'!");
            }
        }
    }
}