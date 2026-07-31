using System;
using System.Collections.Generic;

namespace NPBehave
{
    public class Blackboard
    {
        public enum Type
        {
            Add,
            Remove,
            Change
        }
        private struct Notification
        {
            public readonly string     Key;
            public readonly Type       Type;
            public readonly ASharedValue Value;
            public Notification(string key, Type type, ASharedValue value)
            {
                this.Key = key;
                this.Type = type;
                this.Value = value;
            }
        }

        private          bool                                               m_IsNotifying;
        private readonly Clock                                              m_Clock;
        private readonly Dictionary<string, ASharedValue>                     m_Data         = new Dictionary<string, ASharedValue>();
        private readonly Dictionary<string, List<Action<Type, ASharedValue>>> m_Observers    = new Dictionary<string, List<Action<Type, ASharedValue>>>();
        private readonly Dictionary<string, List<Action<Type, ASharedValue>>> m_AddObservers = new Dictionary<string, List<Action<Type, ASharedValue>>>();
        private readonly Dictionary<string, List<Action<Type, ASharedValue>>> m_RemoveObservers =
            new Dictionary<string, List<Action<Type, ASharedValue>>>();
        private readonly List<Notification>  m_Notifications         = new List<Notification>();
        private readonly List<Notification>  m_NotificationsDispatch = new List<Notification>();
        private readonly Blackboard          m_ParentBlackboard;
        private readonly HashSet<Blackboard> m_Children = new HashSet<Blackboard>();

        public Blackboard(Blackboard parent, Clock clock)
        {
            this.m_Clock = clock;
            this.m_ParentBlackboard = parent;
        }
        public Blackboard(Clock clock)
        {
            this.m_ParentBlackboard = null;
            this.m_Clock = clock;
        }
        
        public Blackboard(Clock clock, NPRootNodeData.SharedValueDictionary blackboardData)
        {
            this.m_ParentBlackboard = null;
            this.m_Clock = clock;
            foreach (KeyValuePair<string, ASharedValue> keyValuePair in blackboardData)
            {
                this.m_Data.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public void Enable()
        {
            this.m_ParentBlackboard?.m_Children.Add(this);
        }

        public void Disable()
        {
            this.m_ParentBlackboard?.m_Children.Remove(this);
            this.m_Clock?.RemoveTimer(this.NotifyObservers);
        }

        public void Set<T>(string key, T value) where T : notnull
        {
            if (this.m_ParentBlackboard != null && this.m_ParentBlackboard.IsSet(key))
            {
                this.m_ParentBlackboard.Set(key, value);
            }
            else
            {
                if (!this.m_Data.ContainsKey(key))
                {
                    ASharedValue sharedValue = SharedValueHelper.AutoCreateBlackboardValueFromTValue(value);
                    this.m_Data.Add(key, sharedValue);
                    this.m_Notifications.Add(new Notification(key, Type.Add, sharedValue));
                    this.m_Clock.AddTimer(0f, 0, this.NotifyObservers);
                }
                else
                {
                    ASharedValueBase<T> sharedValueBase = this.m_Data[key] as ASharedValueBase<T>;
                    if ((sharedValueBase == null && value != null) ||
                        (sharedValueBase != null && !sharedValueBase.GetValue().Equals(value)))
                    {
                        if (sharedValueBase != null)
                        {
                            sharedValueBase.SetValueFrom(value);
                            this.m_Notifications.Add(new Notification(key, Type.Change, sharedValueBase));
                        }

                        this.m_Clock.AddTimer(0f, 0, this.NotifyObservers);
                    }
                }
            }
        }

        public void Unset(string key)
        {
            if (!this.m_Data.ContainsKey(key)) return;
            this.m_Data.Remove(key);
            this.m_Notifications.Add(new Notification(key, Type.Remove, null));
            this.m_Clock.AddTimer(0f, 0, this.NotifyObservers);
        }

        public T Get<T>(string key)
        {
            ASharedValue result = this.Get(key);
            if (result == null)
            {
                return default;
            }

            ASharedValueBase<T> finalResult = result as ASharedValueBase<T>;
            if (finalResult == null)
            {
                Log.Error($"get blackboard value filed, Key:{key}, Type:{typeof(ASharedValueBase<T>)}");
                return default;
            }
            else
            {
                return finalResult.GetValue();
            }
        }

        public ASharedValue Get(string key)
        {
            return this.m_Data.ContainsKey(key) ? this.m_Data[key] : this.m_ParentBlackboard?.Get(key);
        }

        public bool IsSet(string key)
        {
            return this.m_Data.ContainsKey(key) || (this.m_ParentBlackboard != null && this.m_ParentBlackboard.IsSet(key));
        }

        public void AddObserver(string key, Action<Type, ASharedValue> observer)
        {
            List<Action<Type, ASharedValue>> observers = GetObserverList(this.m_Observers, key);
            if (!this.m_IsNotifying)
            {
                if (!observers.Contains(observer))
                {
                    observers.Add(observer);
                }
            }
            else
            {
                if (!observers.Contains(observer))
                {
                    List<Action<Type, ASharedValue>> addObservers = GetObserverList(this.m_AddObservers, key);
                    if (!addObservers.Contains(observer))
                    {
                        addObservers.Add(observer);
                    }
                }

                List<Action<Type, ASharedValue>> removeObservers = GetObserverList(this.m_RemoveObservers, key);
                if (removeObservers.Contains(observer))
                {
                    removeObservers.Remove(observer);
                }
            }
        }

        public void RemoveObserver(string key, Action<Type, ASharedValue> observer)
        {
            List<Action<Type, ASharedValue>> observers = GetObserverList(this.m_Observers, key);
            if (!this.m_IsNotifying)
            {
                if (observers.Contains(observer))
                {
                    observers.Remove(observer);
                }
            }
            else
            {
                List<Action<Type, ASharedValue>> removeObservers = GetObserverList(this.m_RemoveObservers, key);
                if (!removeObservers.Contains(observer))
                {
                    if (observers.Contains(observer))
                    {
                        removeObservers.Add(observer);
                    }
                }

                List<Action<Type, ASharedValue>> addObservers = GetObserverList(this.m_AddObservers, key);
                if (addObservers.Contains(observer))
                {
                    addObservers.Remove(observer);
                }
            }
        }


#if UNITY_EDITOR
        public List<string> Keys
        {
            get
            {
                if (this.m_ParentBlackboard != null)
                {
                    List<string> keys = this.m_ParentBlackboard.Keys;
                    keys.AddRange(this.m_Data.Keys);
                    return keys;
                }
                else
                {
                    return new List<string>(this.m_Data.Keys);
                }
            }
        }

        public int NumObservers
        {
            get
            {
                int count = 0;
                foreach (string key in this.m_Observers.Keys)
                {
                    count += this.m_Observers[key].Count;
                }
                return count;
            }
        }
#endif


        private void NotifyObservers()
        {
            if (this.m_Notifications.Count == 0)
            {
                return;
            }

            this.m_NotificationsDispatch.Clear();
            this.m_NotificationsDispatch.AddRange(this.m_Notifications);
            foreach (Blackboard child in this.m_Children)
            {
                child.m_Notifications.AddRange(this.m_Notifications);
                child.m_Clock.AddTimer(0f, 0, child.NotifyObservers);
            }
            this.m_Notifications.Clear();

            this.m_IsNotifying = true;
            foreach (Notification notification in this.m_NotificationsDispatch)
            {
                if (!this.m_Observers.ContainsKey(notification.Key))
                {
                    Log.Info("1 do not notify for key:" + notification.Key + " value: " + notification.Value);
                    continue;
                }

                List<Action<Type, ASharedValue>> observers = GetObserverList(this.m_Observers, notification.Key);
                foreach (Action<Type, ASharedValue> observer in observers)
                {
                    if (this.m_RemoveObservers.ContainsKey(notification.Key) && this.m_RemoveObservers[notification.Key].Contains(observer))
                    {
                        continue;
                    }
                    observer(notification.Type, notification.Value);
                }
            }

            foreach (string key in this.m_AddObservers.Keys)
            {
                GetObserverList(this.m_Observers, key).AddRange(this.m_AddObservers[key]);
            }
            foreach (string key in this.m_RemoveObservers.Keys)
            {
                foreach (Action<Type, ASharedValue> action in this.m_RemoveObservers[key])
                {
                    GetObserverList(this.m_Observers, key).Remove(action);
                }
            }
            this.m_AddObservers.Clear();
            this.m_RemoveObservers.Clear();

            this.m_IsNotifying = false;
        }

        private static List<Action<Type, ASharedValue>> GetObserverList(Dictionary<string, List<Action<Type, ASharedValue>>> target, string key)
        {
            List<Action<Type, ASharedValue>> observers;
            if (target.ContainsKey(key))
            {
                observers = target[key];
            }
            else
            {
                observers = new List<Action<Type, ASharedValue>>();
                target[key] = observers;
            }
            return observers;
        }
    }
}
