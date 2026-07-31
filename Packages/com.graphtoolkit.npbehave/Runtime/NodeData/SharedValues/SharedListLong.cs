using System;
using System.Collections.Generic;

namespace NPBehave
{
    [Serializable]
    public class SharedListLong : ASharedValueBase<List<long>>, IEquatable<SharedListLong>
    {
        public new      List<long> Value;
        
        public override Type ValueType => typeof(List<long>);

        protected override void SetValueFrom(ISharedValue<List<long>> sharedValue)
        {
            this.Value.Clear();
            foreach (long item in sharedValue.GetValue())
            {
                this.Value.Add(item);
            }
        }

        public override void SetValueFrom(List<long> sharedValue)
        {
            this.Value.Clear();
            foreach (long item in sharedValue)
            {
                this.Value.Add(item);
            }
        }

        #region 对比函数

        public bool Equals(SharedListLong other)
        {
            // If parameter is null, return false.
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            if (this.Value.Count != other.GetValue().Count)
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            for (int i = this.Value.Count - 1; i >= 0; i--)
            {
                if (this.Value[i] != other.GetValue()[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == this.GetType() && this.Equals((SharedListLong) obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static bool operator ==(SharedListLong lhs, SharedListLong rhs)
        {
            // Check for null on left side.
            return lhs?.Equals(rhs) ?? ReferenceEquals(rhs, null);
        }

        public static bool operator !=(SharedListLong lhs, SharedListLong rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >(SharedListLong lhs, SharedListLong rhs)
        {
            Log.Error("no support!");
            return false;
        }

        public static bool operator <(SharedListLong lhs, SharedListLong rhs)
        {
            Log.Error("no support!");
            return false;
        }

        public static bool operator >= (SharedListLong lhs, SharedListLong rhs)
        {
            Log.Error("no support!");
            return false;
        }

        public static bool operator <=(SharedListLong lhs, SharedListLong rhs)
        {
            Log.Error("no support!");
            return false;
        }

        #endregion
    }
}