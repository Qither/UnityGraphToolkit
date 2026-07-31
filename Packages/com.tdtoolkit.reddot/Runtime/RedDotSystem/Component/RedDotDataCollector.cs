using PiRhoSoft.Utilities;
using System;
using UnityEngine;

namespace RedDotSystem.Runtime.Component
{
    public class RedDotDataCollector : MonoBehaviour
    {
        [SerializeField]
        private string m_NodeName;

        [SerializeField]
        private GameObject m_RedDotGameObject;

        private int m_Value;

        private void Start()
        {
            this.m_RedDotGameObject.SetActive(false);
        }

        public void StaticBind(RedDotService service)
        {
            service.StaticBind(this.m_NodeName, this.OnValueChanged);
        }

        public void StaticUnbind(RedDotService service)
        {
            service.StaticUnbind(this.m_NodeName, this.OnValueChanged);
        }

        public void DynamicBind(RedDotService service, string args)
        {
            service.DynamicBind(this.m_NodeName, this.OnValueChanged, args);
        }

        public void DynamicUnBind(RedDotService service, string args)
        {
            service.DynamicUnbind(this.m_NodeName, this.OnValueChanged, args);
        }

        public void OnValueChanged(TrieNode<(RedDotData nodeData, int nodeValue)> node)
        {
            if (this.m_RedDotGameObject != null && node != null && node.Data.nodeValue != this.m_Value)
            {
                this.m_Value = node.Data.nodeValue;
                this.m_RedDotGameObject.SetActive(node.Data.nodeValue > 0);
            }
            else
            {
                Debug.LogError(
                    "RedDotDataCollector.OnValueChanged: m_RedDotGameObject is null or node is null or nodeValue is equal to m_Value");
            }
        }
    }
}