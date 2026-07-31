using GraphProcessor;
using NPBehave;
using System;
using UnityEngine;

namespace NPBehaveEditor
{
    public abstract class ANPNodeBase : BaseNode
    {
        public abstract string         Icon { get; }
        
        public abstract ANPNodeDataBase GetNodeData();
    }
}