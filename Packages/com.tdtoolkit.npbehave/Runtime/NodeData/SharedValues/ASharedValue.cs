using System;

namespace NPBehave
{
    [Serializable]
    public abstract class ASharedValue
    {
        public abstract Type ValueType { get; }

        /// <summary>
        /// 从另一个NP行为树黑板值设置数据
        /// </summary>
        /// <param name="sharedValue"></param>
        public abstract void SetValueFrom(ASharedValue sharedValue);
    }
}