namespace NPBehave
{
    public interface ISharedValueHandle
    {
        void SetTargetBlackboardUseBlackboardValue(ASharedValue blackboardValue, Blackboard blackboard, string key);

        ASharedValue AutoCreateBlackboardValueFromTValue<T>(T value);
        
        bool Compare(ASharedValue lhs, ASharedValue rhs, Operator op);
    }
}