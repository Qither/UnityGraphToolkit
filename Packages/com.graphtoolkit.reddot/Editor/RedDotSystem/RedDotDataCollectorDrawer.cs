using GraphProcessor;
using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using RedDotSystem.Editor.Node;
using RedDotSystem.Runtime.Component;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace RedDotSystem.Editor
{
    [CustomEditor(typeof(RedDotDataCollector))]
    public class RedDotDataCollectorDrawer : UnityEditor.Editor
    {
        private Box                 m_Header;
        private Label               m_NodeNameLabel;
        private Label               m_NodeDescLabel;
        private IntegerField        m_OrderField;
        private MessageBox          m_ErrorMessage;
        private VisualElement       m_Root;
        private RedDotDataCollector m_RedDotDataCollector;
        private FieldInfo           m_NodeNameFieldInfo;
        public void OnEnable()
        {
            this.m_RedDotDataCollector = this.target as RedDotDataCollector;
            if (this.m_RedDotDataCollector == null) return;
            
            this.m_Root                = new VisualElement();
            this.m_Header              = new Box(){
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign          = TextAnchor.MiddleCenter,
                    borderTopColor          = Color.black,
                    borderLeftColor         = Color.black,
                    borderRightColor        = Color.black,
                    borderBottomColor       = Color.black,
                    borderTopWidth          = 1,
                    borderLeftWidth         = 1,
                    borderRightWidth        = 1,
                    borderBottomWidth       = 1,
                    marginTop               = 5,
                    marginBottom            = 5,
                    marginLeft              = 5,
                    marginRight             = 5,
                    paddingTop              = 5,
                    paddingBottom           = 5,
                    paddingLeft             = 5,
                    paddingRight            = 5,
                }
            };
            
            this.m_NodeNameLabel       = new Label
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    unityTextAlign          = TextAnchor.MiddleCenter,
                }
            };
            this.m_NodeDescLabel       = new Label
            {
                style =
                {
                    unityTextAlign          = TextAnchor.MiddleCenter,
                }
            };
            this.m_Header.Add(this.m_NodeNameLabel);
            this.m_Header.Add(this.m_NodeDescLabel);
            this.m_OrderField = new IntegerField("Order");
            this.m_ErrorMessage = new MessageBox(MessageBoxType.Error, "Not found RedDotNode.");
            
            this.m_NodeNameFieldInfo = this.m_RedDotDataCollector.GetType().GetField("m_NodeName", BindingFlags.NonPublic | BindingFlags.Instance);
            this.m_OrderField.value = -1;
        }

        private void RefreshNodeName()
        {
            string nodeName = this.m_NodeNameFieldInfo?.GetValue(this.m_RedDotDataCollector)?.ToString();
            string key      = nodeName?.Replace('_', '\n');
            if (!string.IsNullOrEmpty(nodeName))
            {
                BaseNode graphNode = RedDotGraphAssetCallback.RedDotGraph.nodes.FirstOrDefault(node =>
                {
                    if (node is RedDotNode curRedDotNode)
                    {
                        return curRedDotNode.Data.Key == key;
                    }

                    return false;
                });

                if (graphNode != null)
                {
                    this.m_OrderField.value = graphNode.computeOrder;
                }

                if (graphNode is RedDotNode redDotNode)
                {
                    this.m_NodeDescLabel.text = redDotNode.Data.Desc;
                }
                
                this.m_NodeNameLabel.text = nodeName;
                this.m_Root.Insert(0, this.m_Header);
            }
            else
            {
                if (this.m_Root.Contains(this.m_Header))
                {
                    this.m_Root.Remove(this.m_Header);
                }
                this.m_Root.Insert(0, this.m_ErrorMessage);
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            SerializedProperty redDotGameObject = this.serializedObject.FindProperty("m_RedDotGameObject");
            
            this.RefreshNodeName();
            this.m_OrderField.RegisterCallback((FocusOutEvent evt) =>
            {
                BaseNode graphNode = RedDotGraphAssetCallback.RedDotGraph.nodes.FirstOrDefault(node => node.computeOrder ==  this.m_OrderField.value);
                if (graphNode is RedDotNode redDotNode)
                {
                    this.m_NodeNameFieldInfo?.SetValue(this.m_RedDotDataCollector, redDotNode.Data.Key.Replace('\n', '_'));
                    this.m_NodeNameLabel.text = this.m_NodeNameFieldInfo?.GetValue(this.m_RedDotDataCollector).ToString();
                    this.m_NodeDescLabel.text = redDotNode.Data.Desc;
                    this.m_Root.Insert(0, this.m_Header);

                    if (this.m_ErrorMessage != null && this.m_Root.Contains(this.m_ErrorMessage))
                    {
                        this.m_Root.Remove(this.m_ErrorMessage);
                    }
                    EditorUtility.SetDirty(this.serializedObject.targetObject);
                }
                else
                {
                    this.m_NodeNameFieldInfo?.SetValue(this.m_RedDotDataCollector, null);
                    this.RefreshNodeName();
                }
            });

            PropertyField redDotGameObjectField = new PropertyField(redDotGameObject);
            this.m_Root.Add(this.m_OrderField);
            this.m_Root.Add(redDotGameObjectField);
            return this.m_Root;
        }
    }
}