using System.Collections.Generic;

namespace NPBehave
{
    public class Clock
    {
        private readonly List<System.Action>              m_UpdateObservers = new List<System.Action>();
        private readonly Dictionary<System.Action, Timer> m_Timers          = new Dictionary<System.Action, Timer>();
        private readonly HashSet<System.Action>           m_RemoveObservers = new HashSet<System.Action>();
        private readonly HashSet<System.Action>           m_AddObservers    = new HashSet<System.Action>();
        private readonly HashSet<System.Action>           m_RemoveTimers    = new HashSet<System.Action>();
        private readonly Dictionary<System.Action, Timer> m_AddTimers       = new Dictionary<System.Action, Timer>();
        private          bool                             m_IsInUpdate;

        public delegate float RandomDelegate();

        private readonly RandomDelegate m_Random;

        public float Random => this.m_Random();

        public Clock(RandomDelegate random)
        {
            this.m_Random = random;
        }
        
        class Timer
        {
            public double ScheduledTime;
            public int Repeat;
            public bool Used;
            public double Delay;
            public float RandomVariance;
            public RandomDelegate Random;

            public void ScheduleAbsoluteTime(double elapsedTime)
            {
                this.ScheduledTime = elapsedTime + this.Delay - this.RandomVariance * 0.5f + this.RandomVariance * this.Random();
            }
        }

        private double m_ElapsedTime;

        private readonly List<Timer> m_TimerPool             = new List<Timer>();
        private          int         m_CurrentTimerPoolIndex;

        /// <summary>Register a timer function</summary>
        /// <param name="time">time in milliseconds</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float time, int repeat, System.Action action)
        {
            this.AddTimer(time, 0f, repeat, action);
        }

        /// <summary>Register a timer function with random variance</summary>
        /// <param name="delay">time in milliseconds</param>
        /// <param name="randomVariance">deviate from time on a random basis</param>
        /// <param name="repeat">number of times to repeat, set to -1 to repeat until unregistered.</param>
        /// <param name="action">method to invoke</param>
        public void AddTimer(float delay, float randomVariance, int repeat, System.Action action)
        {
            
            Timer timer;

            if (!this.m_IsInUpdate)
            {
                if (!this.m_Timers.ContainsKey(action))
                {
                    this.m_Timers[action] = this.GetTimerFromPool();
                }
                timer = this.m_Timers[action];
            }
            else
            {
                if (!this.m_AddTimers.ContainsKey(action))
                {
                    this.m_AddTimers[action] = this.GetTimerFromPool();
                }
                timer = this.m_AddTimers [action];

                if (this.m_RemoveTimers.Contains(action))
                {
                    this.m_RemoveTimers.Remove(action);
                }
            }

            Log.AssertIsTrue(timer.Used);
            timer.Delay = delay;
            timer.RandomVariance = randomVariance;
            timer.Repeat = repeat;
            timer.Random = this.m_Random;
            timer.ScheduleAbsoluteTime(this.m_ElapsedTime);
        }

        public void RemoveTimer(System.Action action)
        {
            if (!this.m_IsInUpdate)
            {
                if (this.m_Timers.ContainsKey(action))
                {
                    this.m_Timers[action].Used = false;
                    this.m_Timers.Remove(action);
                }
            }
            else
            {
                if (this.m_Timers.ContainsKey(action))
                {
                    this.m_RemoveTimers.Add(action);
                }
                if (this.m_AddTimers.ContainsKey(action))
                {
                    Log.AssertIsTrue(this.m_AddTimers[action].Used);
                    this.m_AddTimers[action].Used = false;
                    this.m_AddTimers.Remove(action);
                }
            }
        }

        public bool HasTimer(System.Action action)
        {
            if (!this.m_IsInUpdate)
            {
                return this.m_Timers.ContainsKey(action);
            }
            else
            {
                if (this.m_RemoveTimers.Contains(action))
                {
                    return false;
                }
                else if (this.m_AddTimers.ContainsKey(action))
                {
                    return true;
                }
                else
                {
                    return this.m_Timers.ContainsKey(action);
                }
            }
        }

        /// <summary>Register a function that is called every frame</summary>
        /// <param name="action">function to invoke</param>
        public void AddUpdateObserver(System.Action action)
        {
            if (!this.m_IsInUpdate)
            {
                this.m_UpdateObservers.Add(action);
            }
            else
            {
                if (!this.m_UpdateObservers.Contains(action))
                {
                    this.m_AddObservers.Add(action);
                }
                if (this.m_RemoveObservers.Contains(action))
                {
                    this.m_RemoveObservers.Remove(action);
                }
            }
        }

        public void RemoveUpdateObserver(System.Action action)
        {
            if (!this.m_IsInUpdate)
            {
                this.m_UpdateObservers.Remove(action);
            }
            else
            {
                if (this.m_UpdateObservers.Contains(action))
                {
                    this.m_RemoveObservers.Add(action);
                }
                if (this.m_AddObservers.Contains(action))
                {
                    this.m_AddObservers.Remove(action);
                }
            }
        }

        public bool HasUpdateObserver(System.Action action)
        {
            if (!this.m_IsInUpdate)
            {
                return this.m_UpdateObservers.Contains(action);
            }
            else
            {
                if (this.m_RemoveObservers.Contains(action))
                {
                    return false;
                }
                else if (this.m_AddObservers.Contains(action))
                {
                    return true;
                }
                else
                {
                    return this.m_UpdateObservers.Contains(action);
                }
            }
        }

        public void Update(float deltaTime)
        {
            this.m_ElapsedTime += deltaTime;

            this.m_IsInUpdate = true;

            foreach (System.Action action in this.m_UpdateObservers)
            {
                if (!this.m_RemoveObservers.Contains(action))
                {
                    action.Invoke();
                }
            }

            Dictionary<System.Action, Timer>.KeyCollection keys = this.m_Timers.Keys;
            foreach (System.Action callback in keys)
            {
                if (this.m_RemoveTimers.Contains(callback))
                {
                    continue;
                }

                Timer timer = this.m_Timers[callback];
                if (timer.ScheduledTime <= this.m_ElapsedTime)
                {
                    if (timer.Repeat == 0)
                    {
                        this.RemoveTimer(callback);
                    }
                    else if (timer.Repeat >= 0)
                    {
                        timer.Repeat--;
                    }
                    callback.Invoke();
                    timer.ScheduleAbsoluteTime(this.m_ElapsedTime);
                }
            }

            foreach (System.Action action in this.m_AddObservers)
            {
                this.m_UpdateObservers.Add(action);
            }
            foreach (System.Action action in this.m_RemoveObservers)
            {
                this.m_UpdateObservers.Remove(action);
            }
            foreach (System.Action action in this.m_AddTimers.Keys)
            {
                if (this.m_Timers.ContainsKey(action))
                {
                    Log.AssertAreNotEqual(this.m_Timers[action], this.m_AddTimers[action]);
                    this.m_Timers[action].Used = false;
                }
                Log.AssertIsTrue(this.m_AddTimers[action].Used);
                this.m_Timers[action] = this.m_AddTimers[action];
            }
            foreach (System.Action action in this.m_RemoveTimers)
            {
                Log.AssertIsTrue(this.m_Timers[action].Used);
                this.m_Timers[action].Used = false;
                this.m_Timers.Remove(action);
            }
            this.m_AddObservers.Clear();
            this.m_RemoveObservers.Clear();
            this.m_AddTimers.Clear();
            this.m_RemoveTimers.Clear();

            this.m_IsInUpdate = false;
        }

        public int NumUpdateObservers => this.m_UpdateObservers.Count;

        public int NumTimers => this.m_Timers.Count;

        public double ElapsedTime => this.m_ElapsedTime;

        private Timer GetTimerFromPool()
        {
            int   i     = 0;
            int   l     = this.m_TimerPool.Count;
            Timer timer = null;
            while (i < l)
            {
                int timerIndex = (i + this.m_CurrentTimerPoolIndex) % l;
                if (!this.m_TimerPool[timerIndex].Used)
                {
                    this.m_CurrentTimerPoolIndex = timerIndex;
                    timer = this.m_TimerPool[timerIndex];
                    break;
                }
                i++;
            }

            if (timer == null)
            {
                timer                        = new Timer();
                this.m_CurrentTimerPoolIndex = 0;
                this.m_TimerPool.Add(timer);
            }

            timer.Used = true;
            return timer;
        }

        public int DebugPoolSize => this.m_TimerPool.Count;
    }
}