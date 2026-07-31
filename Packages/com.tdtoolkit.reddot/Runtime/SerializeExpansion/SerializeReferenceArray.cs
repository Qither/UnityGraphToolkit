using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace SerializeExpansion.Runtime
{
    [Serializable]
	public class SerializeReferenceArray<T> : ICloneable, IList, IStructuralComparable, IStructuralEquatable, ICollection, IEnumerable
	{
		// This is protected so it can be found by the editor
        #if UNITY_EDITOR
		[SerializeField] 
        #endif
        protected T[] items;

		public T[] Array => this.items;
		public int Length => this.items.Length;

		public SerializeReferenceArray(int count)
		{
			this.items = new T[count];
		}

		public T this[int index]
		{
			get { return this.items[index]; }
			set { this.items[index] = value; }
		}

		#region ICollection Implementation

		int ICollection.Count
		{
			get { return ((ICollection)this.items).Count; }
		}

		public bool IsSynchronized
		{
			get { return this.items.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return this.items.SyncRoot; }
		}

		public void CopyTo(Array array, int index)
		{
			this.items.CopyTo(array, index);
		}

		#endregion

		#region IClonable Implementation

		public object Clone()
		{
			return this.items.Clone();
		}

		#endregion

		#region IComparable Implementation

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			return ((IStructuralComparable)this.items).CompareTo(other, comparer);
		}

		#endregion

		#region IStructuralEquatable Implementation

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			return ((IStructuralEquatable)this.items).Equals(other, comparer);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return ((IStructuralEquatable)this.items).GetHashCode(comparer);
		}

		#endregion

		#region IEnumerable Implementation

		public IEnumerator GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		#endregion

		#region IList Implementation

		object IList.this[int index]
		{
			get { return this.items[index]; }
			set { ((IList)this.items)[index] = value; }
		}

		public bool IsFixedSize
		{
			get { return this.items.IsFixedSize; }
		}

		public bool IsReadOnly
		{
			get { return this.items.IsReadOnly; }
		}

		int IList.Add(object value)
		{
			return ((IList)this.items).Add(value);
		}

		void IList.Clear()
		{
			((IList)this.items).Clear();
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.items).Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.items).IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			((IList)this.items).Insert(index, value);
		}

		void IList.Remove(object value)
		{
			((IList)this.items).Remove(value);
		}

		void IList.RemoveAt(int index)
		{
			((IList)this.items).RemoveAt(index);
		}

		#endregion
	}
}