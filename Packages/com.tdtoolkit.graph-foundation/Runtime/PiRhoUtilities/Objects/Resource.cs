using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
    public interface IResource : ISerializationCallbackReceiver
    {
        string Path { get; }
    }

    public abstract class Resource : ScriptableObject, ISerializationCallbackReceiver
    {
        public const string _invalidPathWarning =
            "(URIP) Invalid Resource location: the {0} at path {1} should be beneath a folder called 'Resources' so it can be loaded at runtime";

        private const string ResourcesFolder = "Resources/";
        private static readonly int FolderLength = ResourcesFolder.Length;
        private static readonly int ExtensionLength = ".asset".Length;

        public static Func<UnityEngine.Object, string> AssetPathResolver { private get; set; }

        [SerializeField, HideInInspector]
        private string _path = string.Empty;

        public string Path => this._path;

        public void OnBeforeSerialize()
        {
            if (AssetPathResolver != null)
            {
                this._path = GetResourcePath(this);
            }
        }

        public void OnAfterDeserialize()
        {
        }

        public static string GetResourcePath(UnityEngine.Object obj)
        {
            string path = AssetPathResolver?.Invoke(obj) ?? string.Empty;
            int index = path.IndexOf(ResourcesFolder, StringComparison.Ordinal);
            if (index < 0)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    Debug.LogWarningFormat(obj, _invalidPathWarning, obj.GetType().Name, path);
                }

                return path;
            }

            return path.Substring(index + FolderLength,
                path.Length - index - FolderLength - ExtensionLength);
        }
    }
}
