using System;

namespace RedDotSystem.Runtime
{
    [Serializable]
    public sealed class RedDotConfigDocumentV1
    {
        public int formatVersion = 1;

        public RedDotData root;

        public RedDotConfigDocumentV1()
        {
        }

        public RedDotConfigDocumentV1(RedDotData rootData)
        {
            this.root = rootData;
        }
    }
}
