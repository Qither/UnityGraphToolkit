using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEngine;
using PiRhoSoft.Utilities;
#endif

namespace NPBehave
{
    /// <summary>
    /// 与黑板节点相关的数据
    /// </summary>
    [Serializable]
    public class NPBlackBoardHandleData
    {
        /// <summary>
        /// 字典值
        /// </summary>
#if UNITY_EDITOR
        [ChangeTrigger(nameof(OnBlackBoardKeySelected))]
        [Popup(nameof(GetBlackBoardKeys))]
#endif
        public string blackBoardKey;

        /// <summary>
        /// 是否可以把值写入黑板，或者是否与黑板进行值对比
        /// </summary>
        public bool writeOrCompareToBlackBoard;
        
#if UNITY_EDITOR
        [SerializeReference]
        [SharedValueReference]
        [Conditional(nameof(writeOrCompareToBlackBoard), BoolTest.ShowIfTrue)]
#endif
        public ASharedValue blackBoardValue;

        [NonSerialized]
        public NPRootNodeData RootNodeDataData;
#if UNITY_EDITOR

        [NonReorderable]
        public Type KeyType;

        [NonSerialized]
        public bool IsLink;
        
        [NonSerialized]
        private List<string> m_Keys = new List<string>();

        private List<string> GetBlackBoardKeys()
        {
            this.m_Keys.Clear();
            if (!(this.RootNodeDataData is { blackboardValues: { } }) || this.RootNodeDataData.blackboardValues.Count <= 0)
            {
                this.m_Keys.Add("Null");
                return this.m_Keys;
            }

            if (!string.IsNullOrEmpty(this.blackBoardKey) && this.blackBoardKey != "Null")
            {
                ASharedValue sharedValue = this.RootNodeDataData.blackboardValues[this.blackBoardKey];
                if (this.blackBoardValue == null || this.blackBoardValue.ValueType != sharedValue.ValueType)
                {
                    this.blackBoardValue = Activator.CreateInstance(sharedValue.GetType()) as ASharedValue;
                }
            }
            
            if (this.KeyType == null)
                return this.RootNodeDataData.blackboardValues.Keys.ToList();

            foreach (string valueKey in this.RootNodeDataData.blackboardValues.Keys.Where(
                         valueKey => this.RootNodeDataData.blackboardValues[valueKey].ValueType == this.KeyType))
            {
                this.m_Keys.Add(valueKey);
            }
            return this.m_Keys;
        }

        private void OnBlackBoardKeySelected(string from, string to)
        {
            if (!(this.RootNodeDataData is { blackboardValues: { } }))
            {
                return;
            }

            foreach (KeyValuePair<string, ASharedValue> sharedValue in
                     this.RootNodeDataData.blackboardValues.Where(pair => pair.Key == this.blackBoardKey))
            {
                if (this.IsLink)
                {
                    this.blackBoardValue = sharedValue.Value;
                }
                else
                {
                    if (this.blackBoardValue == null || this.blackBoardValue.ValueType != sharedValue.Value.ValueType)
                    {
                        this.blackBoardValue = Activator.CreateInstance(sharedValue.Value.GetType()) as ASharedValue;
                    }
                }
            }
        }
        
        public void Setup(NPRootNodeData rootNodeData)
        {
            this.RootNodeDataData = rootNodeData;
            if (this.RootNodeDataData.blackboardValues.Keys.Count <= 0)
            {
                this.blackBoardKey   = "Null";
                return;
            }

            if (this.blackBoardKey == null || this.blackBoardKey.Equals("Null"))
            {
                this.blackBoardKey = this.RootNodeDataData.blackboardValues.Keys.FirstOrDefault();
            }
            
            if (this.blackBoardValue != null && this.blackBoardKey != null &&
                this.blackBoardValue.GetType() == this.RootNodeDataData.blackboardValues[this.blackBoardKey].GetType())
            {
                return;
            }
            this.OnBlackBoardKeySelected(null, this.blackBoardKey);
        }
#endif

        /// <summary>
        /// 获取目标黑板对应的此处的键的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBlackBoardValue<T>(Blackboard blackboard)
        {
            return blackboard.Get<T>(this.blackBoardKey);
        }

        /// <summary>
        /// 获取配置的BB值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetBlackboardDataValue<T>()
        {
            return ((ASharedValueBase<T>)this.blackBoardValue).GetValue();
        }

        /// <summary>
        /// 自动根据预先设定的值设置值
        /// </summary>
        /// <param name="blackboard">要修改的黑板</param>
        public void SetBlackBoardValue(Blackboard blackboard)
        {
            SharedValueHelper.SetTargetBlackboardUseBlackboardValue(this.blackBoardValue, blackboard, this.blackBoardKey);
        }

        /// <summary>
        /// 自动根据传来的值设置值
        /// </summary>
        /// <param name="blackboard">将要改变的黑板值</param>
        /// <param name="value">值</param>
        public void SetBlackBoardValue<T>(Blackboard blackboard, T value)
        {
            blackboard.Set(this.blackBoardKey, value);
        }

        /// <summary>
        /// 自动将一个黑板的对应key的value设置到另一个黑板上
        /// </summary>
        /// <param name="form">数据源黑板</param>
        /// <param name="to">目标黑板</param>
        public void BlackboardCopy(Blackboard form, Blackboard to)
        {
            SharedValueHelper.SetTargetBlackboardUseBlackboardValue(form.Get(this.blackBoardKey), to, this.blackBoardKey);
        }
    }
}