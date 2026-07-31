using System;
using NPBehave;

namespace NPBehaveEditor
{
    [Serializable]
    public class DefaultBehaveGraphConfig : IBehaveGraphConfig
    {
        public string name;
        
        public object BehaveArgsExport(NPBehaveData behave, string path)
        {
            return this;
        }
    }
}