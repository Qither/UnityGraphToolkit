#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
using UnityEngine;
using System.Collections.Generic;

namespace NPBehave
{
    public class UnityContext : MonoBehaviour
    {
        private static UnityContext s_Instance = null;

        private static UnityContext GetInstance()
        {
            if (s_Instance != null) return s_Instance;
            
            GameObject gameObject = new GameObject
            {
                name = "~Context"
            };
            
            s_Instance          = (UnityContext)gameObject.AddComponent(typeof(UnityContext));
            gameObject.isStatic = true;
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
            return s_Instance;
        }

        public static Clock GetClock()
        {
            return GetInstance().m_Clock;
        }

        public static Blackboard GetSharedBlackboard(string key)
        {
            UnityContext context = GetInstance();
            if (!context.m_Blackboards.ContainsKey(key))
            {
                context.m_Blackboards.Add(key, new Blackboard(context.m_Clock));
            }
            return context.m_Blackboards[key];
        }

        private readonly Dictionary<string, Blackboard> m_Blackboards = new Dictionary<string, Blackboard>();

        private readonly Clock m_Clock = new Clock(() => UnityEngine.Random.value);

        private void Update()
        {
            this.m_Clock.Update(Time.deltaTime);
        }
    }
}
#endif