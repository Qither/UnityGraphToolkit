using System;
using System.Collections.Generic;

namespace NPBehave
{
    [Serializable]
    public abstract class ANPNodeDataBase
    {
        /// <summary>
        /// 此结点ID
        /// </summary>
#if UNITY_EDITOR
        [UnityEngine.HideInInspector]
#endif
        public int id;
        
        /// <summary>
        /// 与此结点相连的ID
        /// </summary>
#if UNITY_EDITOR
        [UnityEngine.HideInInspector]
#endif
        public List<int> linkedIds = new List<int>();
        
        /// <summary>
        /// 节点信息描述
        /// </summary>
#if UNITY_EDITOR
        [PiRhoSoft.Utilities.Stretch]
        [PiRhoSoft.Utilities.Multiline]
#endif
        public string nodeDes;

        /// <summary>
        /// 获取结点
        /// </summary>
        /// <returns></returns>
        public abstract Node GetNode();

        /// <summary>
        /// 创建组合结点
        /// </summary>
        /// <returns></returns>
        public virtual Composite CreateComposite(Node[] nodes)
        {
            return null;
        }

        /// <summary>
        /// 创建装饰结点
        /// </summary>
        /// <param name="runtimeTree">运行时归属的行为树</param>
        /// <param name="node">所装饰的结点</param>
        /// <returns></returns>
        public virtual Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            return null;
        }

        /// <summary>
        /// 创建任务节点
        /// </summary>
        /// <param name="runtimeTree">运行时归属的行为树</param>
        /// <returns></returns>
        public virtual Task CreateTask(RuntimeTree runtimeTree)
        {
            return null;
        }
    }
}