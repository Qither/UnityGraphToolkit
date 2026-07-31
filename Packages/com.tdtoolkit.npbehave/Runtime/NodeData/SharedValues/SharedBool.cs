using System;

namespace NPBehave
{
    [Serializable]
    public class SharedBool: ASharedValueBase<bool>, IEquatable<SharedBool>
    {
        public new bool Value;
        
        public override Type ValueType => typeof(bool);

        #region 对比函数

        public bool Equals(SharedBool other)
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

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return this.Value == other.GetValue();
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SharedBool) obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public static bool operator ==(SharedBool lhs, SharedBool rhs)
        {
            // Check for null on left side.
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(SharedBool lhs, SharedBool rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >(SharedBool lhs, SharedBool rhs)
        {
            return false;
        }

        public static bool operator <(SharedBool lhs, SharedBool rhs)
        {
            return false;
        }

        public static bool operator >=(SharedBool lhs, SharedBool rhs)
        {
            return false;
        }

        public static bool operator <=(SharedBool lhs, SharedBool rhs)
        {
            return false;
        }

        #endregion
    }
}