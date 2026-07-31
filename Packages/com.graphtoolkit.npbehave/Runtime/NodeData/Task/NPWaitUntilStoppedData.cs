using System;

namespace NPBehave
{
    /// <summary>
    /// 等待到停止节点数据
    /// </summary>
    [Serializable]
    public class NPWaitUntilStoppedData: ANPNodeDataBase
    {
        [NonSerialized]
        private WaitUntilStopped m_WaitUntilStopped;

        public override Node GetNode()
        {
            return this.m_WaitUntilStopped;
        }

        public override Task CreateTask(RuntimeTree runtimeTree)
        {
            this.m_WaitUntilStopped = new WaitUntilStopped();
            return this.m_WaitUntilStopped;
        }
    }
}