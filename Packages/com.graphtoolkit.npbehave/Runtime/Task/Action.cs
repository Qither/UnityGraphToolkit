namespace NPBehave
{
    public class Action : Task
    {
        public enum Result
        {
            Success,
            Failed,
            Blocked,
            Progress
        }

        public enum Request
        {
            Start,
            Update,
            Cancel,
        }

        private readonly System.Func<bool>            m_SingleFrameFunc;
        private readonly System.Func<bool, Result>    m_MultiFrameFunc;
        private readonly System.Func<Request, Result> m_MultiFrameFunc2;
        private readonly System.Action                m_Action;
        private          bool                         m_BWasBlocked;

        public Action(System.Action action) : base("Action")
        {
            this.m_Action = action;
        }

        public Action(System.Func<bool, Result> multiframeFunc) : base("Action")
        {
            this.m_MultiFrameFunc = multiframeFunc;
        }

        public Action(System.Func<Request, Result> multiframeFunc2) : base("Action")
        {
            this.m_MultiFrameFunc2 = multiframeFunc2;
        }


        public Action(System.Func<bool> singleFrameFunc) : base("Action")
        {
            this.m_SingleFrameFunc = singleFrameFunc;
        }

        protected override void DoStart()
        {
            if (this.m_Action != null)
            {
                this.m_Action.Invoke();
                this.Stopped(true);
            }
            else if (this.m_MultiFrameFunc != null)
            {
                Result result = this.m_MultiFrameFunc.Invoke(false);
                switch (result)
                {
                    case Result.Progress:
                        this.RootNode.Clock.AddUpdateObserver( this.OnUpdateFunc );
                        break;
                    case Result.Blocked:
                        this.m_BWasBlocked = true;
                        this.RootNode.Clock.AddUpdateObserver( this.OnUpdateFunc );
                        break;
                    case Result.Success:
                    case Result.Failed:
                    default:
                        this.Stopped(result == Result.Success);
                        break;
                }
            }
            else if (this.m_MultiFrameFunc2 != null)
            {
                Result result = this.m_MultiFrameFunc2.Invoke(Request.Start);
                switch (result)
                {
                    case Result.Progress:
                        this.RootNode.Clock.AddUpdateObserver(this.OnUpdateFunc2);
                        break;
                    case Result.Blocked:
                        this.m_BWasBlocked = true;
                        this.RootNode.Clock.AddUpdateObserver( this.OnUpdateFunc2 );
                        break;
                    case Result.Success:
                    case Result.Failed:
                    default:
                        this.Stopped(result == Result.Success);
                        break;
                }
            }
            else if (this.m_SingleFrameFunc != null)
            {
                this.Stopped(this.m_SingleFrameFunc.Invoke());
            }
        }

        private void OnUpdateFunc()
        {
            Result result = this.m_MultiFrameFunc.Invoke(false);
            if (result == Result.Progress || result == Result.Blocked) return;
            this.RootNode.Clock.RemoveUpdateObserver(this.OnUpdateFunc);
            this.Stopped(result == Result.Success);
        }

        private void OnUpdateFunc2()
        {
            Result result = this.m_MultiFrameFunc2.Invoke( this.m_BWasBlocked ? Request.Start : Request.Update);

            switch (result)
            {
                case Result.Blocked:
                    this.m_BWasBlocked = true;
                    break;
                case Result.Progress:
                    this.m_BWasBlocked = false;
                    break;
                case Result.Success:
                case Result.Failed:
                default:
                    this.RootNode.Clock.RemoveUpdateObserver( this.OnUpdateFunc2 );
                    this.Stopped( result == Result.Success );
                    break;
            }
        }

        protected override void DoStop()
        {
            if (this.m_MultiFrameFunc != null)
            {
                Result result = this.m_MultiFrameFunc.Invoke(true);
                Log.AssertAreNotEqual(result, Result.Progress, "The Task has to return Result.Success, Result.Failed/Blocked after beeing cancelled!");
                this.RootNode.Clock.RemoveUpdateObserver(this.OnUpdateFunc);
                this.Stopped(result == Result.Success);
            }
            else if (this.m_MultiFrameFunc2 != null)
            {
                Result result = this.m_MultiFrameFunc2.Invoke(Request.Cancel);
                Log.AssertAreNotEqual(result, Result.Progress, "The Task has to return Result.Success or Result.Failed/Blocked after beeing cancelled!");
                this.RootNode.Clock.RemoveUpdateObserver(this.OnUpdateFunc2);
                this.Stopped(result == Result.Success);
            }
            else
            {
                Log.AssertIsTrue(false, "DoStop called for a single frame action on " + this);
            }
        }
    }
}