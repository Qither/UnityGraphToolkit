using System;
using System.Collections.Generic;

namespace NPBehave
{
    /// <summary>
    /// 运行时行为树节点装配辅助类。
    /// </summary>
    public static class RuntimeTreeHelper
    {
        private static Dictionary<Type, NodeType> s_NodeRegister = new Dictionary<Type, NodeType>()
        {
            {typeof(NPRootNodeData), NodeType.Decorator},
            {typeof(NPParallelNodeData), NodeType.Composite},
            {typeof(NPSequenceNodeData), NodeType.Composite},
            {typeof(NPSelectorNodeData), NodeType.Composite},
            
            {typeof(NPBlackboardConditionNodeData), NodeType.Decorator},
            {typeof(NPBlackboardMultipleConditionsNodeData), NodeType.Decorator},
            {typeof(NPRepeaterNodeData), NodeType.Decorator},
            {typeof(NPCustomRepeaterNodeData), NodeType.Decorator},
            {typeof(NPServiceNodeData), NodeType.Decorator},
            {typeof(NPSuccessNodeData), NodeType.Decorator},
            {typeof(NPFailureNodeData), NodeType.Decorator},
            
            {typeof(NPActionNodeData), NodeType.Task},
            {typeof(NPWaitNodeData), NodeType.Task},
            {typeof(NPWaitUntilStoppedData), NodeType.Task},
        };

        /// <summary>
        /// 根据当前运行树独占的节点数据创建可执行节点图。
        /// </summary>
        /// <param name="runtimeTree">待装配的运行树。</param>
        /// <returns>完成节点装配的运行树。</returns>
        public static RuntimeTree SetupRuntimeTree(this RuntimeTree runtimeTree)
        {
            foreach (KeyValuePair<int, ANPNodeDataBase> nodeDataBase in runtimeTree.OwnerData.allNode)
            {
                switch (s_NodeRegister[nodeDataBase.Value.GetType()])
                {
                    case NodeType.Task:
                        try
                        {
                            nodeDataBase.Value.CreateTask(runtimeTree);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"{e}-----{nodeDataBase.Value.nodeDes}");
                            throw;
                        }

                        break;
                    case NodeType.Decorator:
                        try
                        {
                            nodeDataBase.Value.CreateDecoratorNode(runtimeTree,
                                runtimeTree.OwnerData.allNode[nodeDataBase.Value.linkedIds[0]]
                                    .GetNode());
                            
                            if (nodeDataBase.Value is NPRootNodeData)
                            {
                                runtimeTree.SetRootNode(nodeDataBase.Value.GetNode() as Root);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error($"{e}-----{nodeDataBase.Value.nodeDes}");
                            throw;
                        }

                        break;
                    case NodeType.Composite:
                        try
                        {
                            List<int> linkedIds = nodeDataBase.Value.linkedIds;
                            Node[] children = new Node[linkedIds.Count];
                            for (int i = 0; i < linkedIds.Count; i++)
                            {
                                children[i] = runtimeTree.OwnerData.allNode[linkedIds[i]].GetNode();
                            }

                            nodeDataBase.Value.CreateComposite(children);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"{e}-----{nodeDataBase.Value.nodeDes}");
                            throw;
                        }

                        break;
                }
            }

            return runtimeTree;
        }
    }
}