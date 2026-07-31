using System;
#if UNITY_EDITOR
using GraphToolkit.Inspector;
using UnityEngine;
#endif

namespace NPBehave
{
    [Serializable]
    public class NPServiceNodeData : ANPNodeDataBase
    {
        [NonSerialized]
        public Service Service;

        /// <summary>
        /// 委托执行时间间隔
        /// </summary>
        public float interval;

#if UNITY_EDITOR
        [ChangeTrigger(nameof(ActionChanged))]
        [SerializeReference]
        [Reference]
#endif
        public ANPActionData actionData;

#if UNITY_EDITOR
        [NonSerialized]
        private NPRootNodeData m_RootNodeDataData;
        
        public void Setup(NPRootNodeData rootNodeData)
        {
            this.m_RootNodeDataData = rootNodeData;

            this.actionData?.Setup(this.m_RootNodeDataData);
        }
        
        private void ActionChanged(object from, object to)
        {
            ANPActionData data = to as ANPActionData;
            data?.Setup(this.m_RootNodeDataData);
        }
        
#endif
        public override Node GetNode()
        {
            return this.Service;
        }

        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            Log.Error($"{runtimeTree}");
            this.actionData.OwnerRuntimeTree = runtimeTree;
            this.Service                              = new Service(this.interval, this.actionData.GetActionToBeDone(), node);
            return this.Service;
        }
    }
}