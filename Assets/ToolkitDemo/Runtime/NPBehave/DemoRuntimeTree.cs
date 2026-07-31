using NPBehave;

namespace ToolkitDemo.NPBehaveDemo
{
    public sealed class DemoRuntimeTree : RuntimeTree
    {
        public DemoRuntimeTree(string unitId, NPBehaveData data, Clock clock)
            : base(unitId, data, clock)
        {
        }
    }
}
