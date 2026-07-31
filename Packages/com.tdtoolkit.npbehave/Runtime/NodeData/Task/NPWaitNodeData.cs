using System;

namespace NPBehave
{
    /// <summary>
    /// 等待节点数据
    /// </summary>
    [Serializable]
    public class NPWaitNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private Wait m_WaitNode;

        public NPBlackBoardHandleData blackBoardHandleData = new NPBlackBoardHandleData();

        public override Task CreateTask(RuntimeTree runtimeTree)
        {
            this.m_WaitNode = new Wait(this.blackBoardHandleData.blackBoardKey);
            return this.m_WaitNode;
        }

        public override Node GetNode()
        {
            return this.m_WaitNode;
        }
    }
}