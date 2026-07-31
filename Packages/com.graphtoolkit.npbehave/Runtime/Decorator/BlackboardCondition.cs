namespace NPBehave
{
    public class BlackboardCondition : ObservingDecorator
    {
        public string Key { get; }

        public ASharedValue Value { get; }

        public Operator Operator { get; }

        public BlackboardCondition(string key, Operator op, ASharedValue value, Stops stopsOnChange, Node decorate) : base("BlackboardCondition", stopsOnChange, decorate)
        {
            this.Operator = op;
            this.Key = key;
            this.Value = value;
            this.StopsOnChange = stopsOnChange;
        }
        
        public BlackboardCondition(string key, Operator op, Stops stopsOnChange, Node decorate) : base("BlackboardCondition", stopsOnChange, decorate)
        {
            this.Operator = op;
            this.Key = key;
            this.StopsOnChange = stopsOnChange;
        }


        protected override void StartObserving()
        {
            this.RootNode.Blackboard.AddObserver(this.Key, this.OnValueChanged);
        }

        protected override void StopObserving()
        {
            this.RootNode.Blackboard.RemoveObserver(this.Key, this.OnValueChanged);
        }

        private void OnValueChanged(Blackboard.Type type, ASharedValue newValue)
        {
            this.Evaluate();
        }

        protected override bool IsConditionMet()
        {
            if (this.Operator == Operator.AlwaysTrue)
            {
                return true;
            }

            if (!this.RootNode.Blackboard.IsSet(this.Key))
            {
                return this.Operator == Operator.IsNotSet;
            }

            ASharedValue sharedValue = this.RootNode.Blackboard.Get(this.Key);

            return SharedValueHelper.Compare(this.Value, sharedValue, this.Operator);
        }

        public override string ToString()
        {
            return "(" + this.Operator + ") " + this.Key + " ? " + this.Value;
        }
    }
}