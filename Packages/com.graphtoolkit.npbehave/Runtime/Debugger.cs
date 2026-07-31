#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
using UnityEngine;
using System.Collections.Generic;

namespace NPBehave
{
    public class Debugger : MonoBehaviour
    {
        public Root BehaviorTree;

        private static Blackboard s_CustomGlobalStats = null;
        public static Blackboard CustomGlobalStats
        {
            get 
            {
                if (s_CustomGlobalStats == null)
                {
                    s_CustomGlobalStats = UnityContext.GetSharedBlackboard("_GlobalStats");;
                }
                return s_CustomGlobalStats;
            }
        }

        private Blackboard m_CustomStats = null;
        public Blackboard CustomStats
        {
            get 
            {
                if (this.m_CustomStats == null)
                {
                    this.m_CustomStats = new Blackboard(CustomGlobalStats, UnityContext.GetClock());
                }
                return this.m_CustomStats;
            }
        }

        public void DebugCounterInc(string key)
        {
            if (!this.CustomStats.IsSet(key))
            {
                this.CustomStats.Set(key, 0);
            }

            this.CustomStats.Set(key, this.CustomStats.Get<int>(key) + 1);
        }

        public void DebugCounterDec(string key)
        {
            if (!this.CustomStats.IsSet(key))
            {
                this.CustomStats.Set(key, 0);
            }

            this.CustomStats.Set(key, this.CustomStats.Get<int>(key) - 1);
        }

        public static void GlobalDebugCounterInc(string key)
        {
            if (!CustomGlobalStats.IsSet(key))
            {
                CustomGlobalStats.Set(key, 0);
            }
            CustomGlobalStats.Set(key, CustomGlobalStats.Get<int>(key) + 1);
        }

        public static void GlobalDebugCounterDec(string key)
        {
            if (!CustomGlobalStats.IsSet(key))
            {
                CustomGlobalStats.Set(key, 0);
            }
            CustomGlobalStats.Set(key, CustomGlobalStats.Get<int>(key) - 1);
        }

    }
}
#endif