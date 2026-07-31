using NPBehave;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NPBehaveEditor
{
    public class DefaultBehaveGraphExport : IBehaveGraphExport
    {
        public virtual Type[] GetKnownTypes()
        {
            return TypeCache.GetTypesDerivedFrom<ASharedValue>()
                .Concat(TypeCache.GetTypesDerivedFrom<ANPNodeDataBase>())
                .Concat(TypeCache.GetTypesDerivedFrom<ANPActionData>())
                .Concat(TypeCache.GetTypesDerivedFrom<IBehaveGraphConfig>())
                .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition)
                .Distinct()
                .OrderBy(type => type.FullName, StringComparer.Ordinal)
                .ToArray();
        }

        public virtual void OnBeforeExport(NPBehaveGraph graph)
        {
            Debug.Log($"OnBeforeExport: {graph.name}");
        }

        public virtual void OnAfterExport(NPBehaveGraph graph)
        {
            Debug.Log($"OnAfterExport: {graph.name}");
        }

        public virtual void OnExportFailed(NPBehaveGraph graph, string error)
        {
            Debug.LogError($"OnExportFailed: {graph.name}, {error}");
        }
    }
}
