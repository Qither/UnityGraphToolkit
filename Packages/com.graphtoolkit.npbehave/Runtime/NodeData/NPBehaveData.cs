using System;
using System.Runtime.Serialization;

namespace NPBehave
{
    [Serializable]
    public class NPBehaveData
    {
        [Serializable]
        public class NodeDictionary : SerializeReferenceDictionary<int, ANPNodeDataBase>
        {
            public NodeDictionary() { }
            
            protected NodeDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }

        public string id;

        /// <summary>
        /// 单个行为树所有结点
        /// </summary>
        public NodeDictionary allNode = new NodeDictionary();

        /// <summary>
        /// 行为数参数
        /// </summary>
        public object args;
    }
}