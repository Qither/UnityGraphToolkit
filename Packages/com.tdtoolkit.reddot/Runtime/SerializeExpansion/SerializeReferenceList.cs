using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace SerializeExpansion.Runtime
{
    [Serializable]
    public class SerializeReferenceList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        public const string ITEMS_PROPERTY = nameof(items);

#if UNITY_EDITOR
        [SerializeReference]
#endif
        protected List<T> items = new List<T>();

        public List<T> List => this.items;

        #region ICollection<T> Implementation

        public int Count => this.items.Count;

        bool ICollection<T>.IsReadOnly => false;

        public void Add(T item)
        {
            this.items.Add(item);
        }

        public bool Remove(T item)
        {
            return this.items.Remove(item);
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        #endregion

        #region ICollection Implementation

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)this.items).CopyTo(array, index);
        }

        #endregion

        #region IEnumerable<T> Implementation

        public IEnumerator<T> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        #endregion

        #region IList<T> Implementation

        public T this[int index]
        {
            get => this.items[index];
            set => this.items[index] = value;
        }

        public int IndexOf(T item)
        {
            return this.items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.items.RemoveAt(index);
        }

        #endregion

        #region IList Implementation

        object IList.this[int index]
        {
            get => this.items[index];
            set => ((IList)this.items)[index] = value;
        }

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        int IList.Add(object value)
        {
            return ((IList)this.items).Add(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)this.items).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ((IList)this.items).Remove(value);
        }

        bool IList.Contains(object value)
        {
            return ((IList)this.items).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)this.items).IndexOf(value);
        }

        #endregion
    }
}