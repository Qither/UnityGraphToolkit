using GraphProcessor;
using System.Collections.Generic;
using UnityEditor;

namespace NPBehaveEditor
{
    public static class GraphWindowHelper
    {
        private static Dictionary<string, BaseGraphWindow> s_AllGraphWindows =
            new Dictionary<string, BaseGraphWindow>();
        
        public static T GetAndShowGraphWindow<T>(string path) where T : BaseGraphWindow
        {
            if (s_AllGraphWindows.TryGetValue(path, out var graphWindow))
            {
                graphWindow.Focus();
                return graphWindow as T;
            }

            T resultWindow = EditorWindow.CreateWindow<T>(typeof(T));
            s_AllGraphWindows[path] = resultWindow;
            return resultWindow;
        }
        
        public static T GetAndShowGraphWindow<T>(BaseGraph owner) where T : BaseGraphWindow
        {
            return GetAndShowGraphWindow<T>(AssetDatabase.GetAssetPath(owner));
        }
        
        public static void AddGraphWindow(BaseGraph owner, BaseGraphWindow graphWindow)
        {
            s_AllGraphWindows[AssetDatabase.GetAssetPath(owner)] = graphWindow;
        }
        
        public static void RemoveGraphWindow(string path)
        {
            s_AllGraphWindows.Remove(path);
        }
        
        public static void RemoveGraphWindow(BaseGraph owner)
        {
            s_AllGraphWindows.Remove(AssetDatabase.GetAssetPath(owner));
        }
    }
}