namespace NPBehave
{

    public class Cooldown : Decorator
    {
        private readonly bool  m_StartAfterDecorate;
        private readonly bool  m_ResetOnFailure;
        private readonly bool  m_FailOnCooldown;
        private readonly float m_CooldownTime;
        private readonly float m_RandomVariation;
        private          bool  m_IsReady = true;

        /// <summary>
        /// The Cooldown decorator ensures that the branch can not be started twice within the given cooldown time.
        /// 
        /// The decorator can start the cooldown timer right away or wait until the child stopps, you can control this behavior with the
        /// `startAfterDecorate` parameter.
        /// 
        /// The default behavior in case the cooldown timer is active and this node is started again is, that the decorator waits until
        /// the cooldown is reached and then executes the underlying node.
        /// You can change this behavior with the `failOnCooldown` parameter to make the decorator immediately fail instead.
        /// 
        /// </summary>
        /// <param name="cooldownTime">time until next execution</param>
        /// <param name="randomVariation">random variation added to the cooldown time</param>
        /// <param name="startAfterDecorate">If set to <c>true</c> the cooldown timer is started from the point after the decorate has been started, else it will be started right away.</param>
        /// <param name="resetOnFailure">If set to <c>true</c> the timer will be reset in case the underlying node fails.</param>
        /// <param name="failOnCooldown">If currently on cooldown and this parameter is set to <c>true</c>, the decorator will immmediately fail instead of waiting for the cooldown.</param>
        /// <param name="decorate">Decorate node.</param>
        public Cooldown(float cooldownTime, float randomVariation, bool startAfterDecorate, bool resetOnFailure, bool failOnCooldown, Node decorate) : base("TimeCooldown", decorate)
        {
            this.m_StartAfterDecorate = startAfterDecorate;
            this.m_CooldownTime = cooldownTime;
            this.m_ResetOnFailure = resetOnFailure;
            this.m_RandomVariation = randomVariation;
            this.m_FailOnCooldown = failOnCooldown;
            Log.AssertIsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, bool startAfterDecorate, bool resetOnFailure, bool failOnCooldown, Node decorate) : base("TimeCooldown", decorate)
        {
            this.m_StartAfterDecorate = startAfterDecorate;
            this.m_CooldownTime = cooldownTime;
            this.m_RandomVariation = cooldownTime * 0.1f;
            this.m_ResetOnFailure = resetOnFailure;
            this.m_FailOnCooldown = failOnCooldown;
            Log.AssertIsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, float randomVariation, bool startAfterDecorate, bool resetOnFailure, Node decorate) : base("TimeCooldown", decorate)
        {
            this.m_StartAfterDecorate = startAfterDecorate;
            this.m_CooldownTime = cooldownTime;
            this.m_ResetOnFailure = resetOnFailure;
            this.m_RandomVariation = randomVariation;
            Log.AssertIsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, bool startAfterDecorate, bool resetOnFailure, Node decorate) : base("TimeCooldown", decorate)
        {
            this.m_StartAfterDecorate = startAfterDecorate;
            this.m_CooldownTime = cooldownTime;
            this.m_RandomVariation = cooldownTime * 0.1f;
            this.m_ResetOnFailure = resetOnFailure;
            Log.AssertIsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, float randomVariation, Node decorate) : base("TimeCooldown", decorate)
        {
            this.m_StartAfterDecorate = false;
            this.m_CooldownTime = cooldownTime;
            this.m_ResetOnFailure = false;
            this.m_RandomVariation = randomVariation;
            Log.AssertIsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        public Cooldown(float cooldownTime, Node decorate) : base("TimeCooldown", decorate)
        {
            this.m_StartAfterDecorate = false;
            this.m_CooldownTime = cooldownTime;
            this.m_ResetOnFailure = false;
            this.m_RandomVariation = cooldownTime * 0.1f;
            Log.AssertIsTrue(cooldownTime > 0f, "cooldownTime has to be set");
        }

        protected override void DoStart()
        {
            if (this.m_IsReady)
            {
                this.m_IsReady = false;
                if (!this.m_StartAfterDecorate)
                {
                    this.Clock.AddTimer(this.m_CooldownTime, this.m_RandomVariation, 0, this.TimeoutReached);
                }

                this.Decorate.Start();
            }
            else
            {
                if (this.m_FailOnCooldown)
                {
                    this.Stopped(false);
                }
            }
        }

        protected override void DoStop()
        {
            if (this.Decorate.IsActive)
            {
                this.m_IsReady = true;
                this.Clock.RemoveTimer(this.TimeoutReached);
                this.Decorate.Stop();
            }
            else
            {
                this.m_IsReady = true;
                this.Clock.RemoveTimer(this.TimeoutReached);
                this.Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            if (this.m_ResetOnFailure && !result)
            {
                this.m_IsReady = true;
                this.Clock.RemoveTimer(this.TimeoutReached);
            }
            else if (this.m_StartAfterDecorate)
            {
                this.Clock.AddTimer(this.m_CooldownTime, this.m_RandomVariation, 0, this.TimeoutReached);
            }

            this.Stopped(result);
        }

        private void TimeoutReached()
        {
            if (this.IsActive && !this.Decorate.IsActive)
            {
                this.Clock.AddTimer(this.m_CooldownTime, this.m_RandomVariation, 0, this.TimeoutReached);
                this.Decorate.Start();
            }
            else
            {
                this.m_IsReady = true;
            }
        }
    }
}