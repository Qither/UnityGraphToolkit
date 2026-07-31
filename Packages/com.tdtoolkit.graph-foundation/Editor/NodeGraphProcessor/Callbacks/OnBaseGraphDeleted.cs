using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Unity 6 can invoke AssetModificationProcessor deletion callbacks on an import worker,
    /// where loading Unity objects is forbidden. Observe completed asset deletions instead and
    /// close graph windows on the main thread.
    /// </summary>
    public sealed class DeleteCallback : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (deletedAssets == null || !deletedAssets.Any(path =>
                    path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            foreach (BaseGraphWindow graphWindow in Resources.FindObjectsOfTypeAll<BaseGraphWindow>())
            {
                graphWindow.OnGraphDeleted();
            }
        }
    }
}
