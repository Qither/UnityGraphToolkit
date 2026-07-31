using System;

namespace NPBehave
{
    /// <summary>
    /// 行为节点数据
    /// </summary>
    [Serializable]
    public class NPActionNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        private Action m_ActionNode;

#if UNITY_EDITOR
        [UnityEngine.SerializeReference]
        [GraphToolkit.Inspector.Reference]
#endif
        public ANPActionData actionData;
        
        public virtual void Setup(NPRootNodeData rootNodeData)
        {
            this.actionData.Setup(rootNodeData);
        }

        public override Task CreateTask(RuntimeTree runtimeTree)
        {
            this.actionData.OwnerRuntimeTree = runtimeTree;
            this.m_ActionNode = this.actionData.CreateBehaveAction();
            return this.m_ActionNode;
        }

        public override Node GetNode()
        {
            return this.m_ActionNode;
        }
    }
}