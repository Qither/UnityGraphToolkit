using System;

namespace NPBehave
{
    [Serializable]
    public abstract class ASharedValueBase<T> : ASharedValue, ISharedValue<T>
    {
        [NonSerialized]
        public T Value;

        public T GetValue()
        {
            return this.Value;
        }

        public override void SetValueFrom(ASharedValue aSharedValue)
        {
            if (aSharedValue == null || !(aSharedValue is ASharedValueBase<T>))
            {
                Log.Error($"{typeof(T)} Copy failed, anpBbValue is null or illegal type");
                return;
            }
            this.SetValueFrom((ISharedValue<T>) aSharedValue);
        }
        
        protected virtual void SetValueFrom(ISharedValue<T> sharedValue)
        {
            if (sharedValue == null || !(sharedValue is ASharedValueBase<T>) )
            {
                Log.Error($"{typeof(T)} Copy failed, anpBbValue is null or illegal type");
                return;
            }
            
            this.SetValueFrom(sharedValue.GetValue());
        }
        
        public virtual void SetValueFrom(T sharedValue)
        {
            this.Value = sharedValue;
        }
    }
}