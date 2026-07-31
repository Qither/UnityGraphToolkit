using NPBehave;
using System;

namespace ToolkitDemo.NPBehaveDemo
{
    [Serializable]
    public sealed class DemoSetBlackboardAction : ANPActionData
    {
        public string key = "DemoRunCount";

        public override System.Action GetActionToBeDone()
        {
            this.Action = () =>
            {
                Blackboard blackboard = this.OwnerRuntimeTree.GetBlackboard();
                int current = blackboard.Get<int>(this.key);
                blackboard.Set(this.key, current + 1);
            };
            return this.Action;
        }
    }
}
