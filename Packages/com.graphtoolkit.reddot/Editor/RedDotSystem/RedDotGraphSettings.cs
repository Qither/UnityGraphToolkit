using GraphToolkit.Inspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedDotSystem.Editor
{
    [Serializable]
    public sealed class RedDotSystemOption
    {
        public int value;

        public string label = "Always available";
    }

    [CreateAssetMenu(fileName = "RedDotGraphSettings", menuName = "Unity Graph Toolkit/Red Dot Settings")]
    public sealed class RedDotGraphSettings : ScriptableObject
    {
        public string executeNodePath = "Assets/ToolkitDemo/RedDot/Generated";

        public string exportPath = "Assets/ToolkitDemo/RedDot/Exports";

        public string exportFileName = "DefaultRedDotConfig.json";

        public string executeNodeNamespace = "ToolkitDemo.RedDot.Generated";

        public List<string> usingNamespaces = new List<string>
        {
            "RedDotSystem.Runtime"
        };

        public List<RedDotSystemOption> systemOptions = new List<RedDotSystemOption>
        {
            new RedDotSystemOption()
        };

        public PopupValues<int> GetSystemValues()
        {
            IEnumerable<RedDotSystemOption> options = this.systemOptions
                .Where(option => option != null)
                .OrderBy(option => option.value);
            return new PopupValues<int>
            {
                Values = options.Select(option => option.value).ToList(),
                Options = options.Select(option => string.IsNullOrWhiteSpace(option.label)
                    ? option.value.ToString()
                    : option.label).ToList()
            };
        }
    }
}
