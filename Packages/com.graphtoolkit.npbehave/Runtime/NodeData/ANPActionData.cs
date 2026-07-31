using System;

namespace NPBehave
{
    // 用于包含Action的数据类
    [Serializable]
    public abstract class ANPActionData
    {
        /// <summary>
        /// 归属的运行时行为树实例
        /// </summary>
        [NonSerialized]
        public RuntimeTree OwnerRuntimeTree;
        
        public System.Action Action;
        
        public Func<bool> Func1;
        
        public Func<bool, Action.Result> Func2;
        
        public virtual void Setup(NPRootNodeData rootNodeData)
        {
        }

        /// <summary>
        /// 获取将要执行的委托函数，也可以在这里面做一些初始化操作
        /// </summary>
        /// <returns></returns>
        public virtual System.Action GetActionToBeDone()
        {
            return null;
        }

        public virtual Func<bool> GetFunc1ToBeDone()
        {
            return null;
        }

        public virtual Func<bool, Action.Result> GetFunc2ToBeDone()
        {
            return null;
        }

        public Action CreateBehaveAction()
        {
            this.GetActionToBeDone();
            if (this.Action != null)
            {
                return new Action(this.Action);
            }

            this.GetFunc1ToBeDone();
            if (this.Func1 != null)
            {
                return new Action(this.Func1);
            }

            this.GetFunc2ToBeDone();
            if (this.Func2 != null)
            {
                return new Action(this.Func2);
            }

            Log.Info($"{this.GetType()} create behave action fail, because no delegate could be found to bind");
            return null;
        }
    }
}