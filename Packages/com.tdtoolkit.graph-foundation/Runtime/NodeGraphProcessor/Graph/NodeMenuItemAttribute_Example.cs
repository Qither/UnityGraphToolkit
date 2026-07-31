/*
using System;
using UnityEngine;
using GraphProcessor;

/// <summary>
/// NodeMenuItemAttribute 扩展使用示例
/// </summary>
namespace GraphProcessor.Examples
{
    // 示例 1: 不指定 order，默认为 0
    [NodeMenuItem("Example/Basic Node")]
    public class BasicExampleNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    // 示例 2: 指定 order = -10，会显示在前面
    [NodeMenuItem("Example/High Priority Node", order = -10)]
    public class HighPriorityNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    // 示例 3: 指定 order = 10，会显示在后面
    [NodeMenuItem("Example/Low Priority Node", order = 10)]
    public class LowPriorityNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    // 示例 4: 多个节点使用相同的 order，会按路径名称字母顺序排序
    [NodeMenuItem("Example/Group A/Node Alpha", order = 5)]
    public class SameOrderAlphaNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    [NodeMenuItem("Example/Group A/Node Beta", order = 5)]
    public class SameOrderBetaNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    [NodeMenuItem("Example/Group A/Node Charlie", order = 5)]
    public class SameOrderCharlieNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    // 示例 5: 混合不同 order 的节点，展示完整排序效果
    [NodeMenuItem("Example/Group B/First", order = -15)]
    public class FirstNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    [NodeMenuItem("Example/Group B/Middle", order = 0)]
    public class MiddleNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    [NodeMenuItem("Example/Group B/Last", order = 20)]
    public class LastNode : BaseNode
    {
        [Input("Input")]
        public float input;
        
        [Output("Output")]
        public float output;
    }
    
    // 最终菜单显示顺序为（右键菜单和搜索窗口都会按此顺序显示）:
    // Example/
    //   Group B/
    //     First (order = -15)
    //   High Priority Node (order = -10)
    //   Basic Node (order = 0, 默认)
    //   Group B/
    //     Middle (order = 0)
    //   Group A/
    //     Node Alpha (order = 5)
    //     Node Beta (order = 5)
    //     Node Charlie (order = 5)
    //   Low Priority Node (order = 10)
    //   Group B/
    //     Last (order = 20)
}
*/

