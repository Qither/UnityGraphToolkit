using System.Linq;

namespace NPBehave
{
    public class BlackboardQuery : ObservingDecorator
    {
        private readonly string[]          m_Keys;
        private readonly System.Func<bool> m_Query;

        public BlackboardQuery(string[] keys, Stops stopsOnChange, System.Func<bool> query, Node decorate) : base("BlackboardQuery", stopsOnChange, decorate)
        {
            this.m_Keys = keys;
            this.m_Query = query;
        }

        protected override void StartObserving()
        {
            foreach (string key in this.m_Keys)
            {
                this.RootNode.Blackboard.AddObserver(key, this.OnValueChanged);
            }
        }

        protected override void StopObserving()
        {
            foreach (string key in this.m_Keys)
            {
                this.RootNode.Blackboard.RemoveObserver(key, this.OnValueChanged);
            }
        }

        private void OnValueChanged(Blackboard.Type type, object newValue)
        {
            this.Evaluate();
        }

        protected override bool IsConditionMet()
        {
            return this.m_Query();
        }

        public override string ToString()
        {
            string keys = this.m_Keys.Aggregate("", (current, key) => current + (" " + key));
            return this.Name + keys;
        }
    }
}