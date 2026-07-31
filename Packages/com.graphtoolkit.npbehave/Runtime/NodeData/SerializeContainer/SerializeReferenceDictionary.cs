using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Serialization;
#endif

namespace NPBehave
{
    [Serializable]
    public class SerializeReferenceDictionary<TKeyType, TValueType> : Dictionary<TKeyType, TValueType>
#if UNITY_EDITOR
        ,ISerializationCallbackReceiver
#endif
    {
        public const string KEY_PROPERTY   = nameof(keys);
        public const string VALUE_PROPERTY = nameof(values);
        
#if UNITY_EDITOR
        [SerializeField]
#endif
        protected List<TKeyType> keys = new List<TKeyType>();

#if UNITY_EDITOR
        [SerializeReference]
#endif
        protected List<TValueType> values = new List<TValueType>();
        
        public SerializeReferenceDictionary() { }
        
        protected SerializeReferenceDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }
        
#if UNITY_EDITOR
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.ConvertToLists(default);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.ConvertFromLists(default);
        }
#endif
        
        [OnSerializing]
        private void ConvertToLists(StreamingContext context)
        {
            this.keys.Clear();
            this.values.Clear();

            foreach (KeyValuePair<TKeyType, TValueType> entry in this)
            {
                this.keys.Add(entry.Key);
                this.values.Add(entry.Value);
            }
        }

        [OnDeserialized]
        private void ConvertFromLists(StreamingContext context)
        {
            int count = Math.Min(this.keys.Count, this.values.Count);
            if (count > 0)
            {
                this.Clear();
            }
            for (int i = 0; i < count; i++) this.Add(this.keys[i], this.values[i]);
        }
    }
}