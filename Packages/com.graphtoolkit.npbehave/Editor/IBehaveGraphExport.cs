using NPBehave;

namespace NPBehaveEditor
{
    public interface IBehaveGraphExport : INPBehaveTypeProvider
    {
        void OnBeforeExport(NPBehaveGraph graph);
        void OnAfterExport(NPBehaveGraph graph);
        void OnExportFailed(NPBehaveGraph graph, string error);
    }
}
