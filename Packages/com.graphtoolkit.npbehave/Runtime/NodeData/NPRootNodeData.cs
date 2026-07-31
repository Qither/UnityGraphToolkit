using System;
using System.Runtime.Serialization;

namespace NPBehave
{
    [Serializable]
    public class NPRootNodeData : ANPNodeDataBase
    {
        [Serializable]
        public class SharedValueDictionary : SerializeReferenceDictionary<string, ASharedValue>
        {
            public SharedValueDictionary()
            {
            }
            
            protected SharedValueDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
        
        [NonSerialized]
        public Root Root;

#if UNITY_EDITOR
        [SerializeDictionary]
#endif
        public SharedValueDictionary blackboardValues = new SharedValueDictionary();

        public override Decorator CreateDecoratorNode(RuntimeTree runtimeTree, Node node)
        {
            this.Root = new Root(node, runtimeTree.GetClock, this.blackboardValues);
            return this.Root;
        }

        public override Node GetNode()
        {
            return this.Root;
        }
    }
}