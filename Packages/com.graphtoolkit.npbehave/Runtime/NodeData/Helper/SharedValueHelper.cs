using System;
using System.Collections.Generic;

namespace NPBehave
{
    public static class SharedValueHelper
    {
        private static ISharedValueHandle s_SharedValueHandle;
        
        public static void RegisterSharedValueHandle(ISharedValueHandle sharedValueHandle)
        {
            s_SharedValueHandle = sharedValueHandle;
        }
        
        /// <summary>
        /// 通过ANPBBValue来设置目标黑板值
        /// </summary>
        /// <param name="blackboardValue"></param>
        /// <param name="blackboard"></param>
        /// <param name="key"></param>
        public static void SetTargetBlackboardUseBlackboardValue(ASharedValue blackboardValue, Blackboard blackboard, string key)
        {
            // 这里只能用这个ToString()来做判断，直接获取Name的话是简略版本的
            switch (blackboardValue.ValueType.ToString())
            {
                case "System.String":
                    SharedString sharedString = blackboardValue as SharedString;
                    if (sharedString != null)
                    {
                        blackboard.Set(key, sharedString.GetValue());
                    }
                    break;
                case "System.Single":
                    SharedFloat sharedFloat = blackboardValue as SharedFloat;
                    if (sharedFloat != null)
                    {
                        blackboard.Set(key, sharedFloat.GetValue());
                    }
                    break;
                case "System.Int32":
                    SharedInt sharedInt = blackboardValue as SharedInt;
                    if (sharedInt != null)
                    {
                        blackboard.Set(key, sharedInt.GetValue());
                    }
                    break;
                case "System.Int64":
                    SharedLong sharedLong = blackboardValue as SharedLong;
                    if (sharedLong != null)
                    {
                        blackboard.Set(key, sharedLong.GetValue());
                    }
                    break;
                case "System.UInt32":
                    SharedUInt sharedUInt = blackboardValue as SharedUInt;
                    if (sharedUInt != null)
                    {
                        blackboard.Set(key, sharedUInt.GetValue());
                    }
                    break;
                case "System.Boolean":
                    SharedBool sharedBool = blackboardValue as SharedBool;
                    if (sharedBool != null)
                    {
                        blackboard.Set(key, sharedBool.GetValue());
                    }
                    break;
                case "System.Collections.Generic.List`1[System.Int64]":
                    SharedListLong sharedListLong = blackboardValue as SharedListLong;
                    if (sharedListLong != null)
                    {
                        blackboard.Set(key, sharedListLong.GetValue());
                    }
                    break;
                default:
                    if (s_SharedValueHandle != null)
                    {
                        s_SharedValueHandle.SetTargetBlackboardUseBlackboardValue(blackboardValue, blackboard, key);
                        return;
                    }
                    break;
            }
            
            Log.Error("SetTargetBlackboardUseBlackboardValue: " + blackboardValue.ValueType.ToString() + " not support!");
        }

        /// <summary>
        /// 自动从T创建一个NP_BBValue
        /// </summary>
        public static ASharedValue AutoCreateBlackboardValueFromTValue<T>(T value)
        {
            string valueType = typeof(T).ToString();

            switch (valueType)
            {
                case "System.String":
                    SharedString sharedString = new SharedString();
                    return SetValue(sharedString, value);
                case "System.Single":
                    SharedFloat sharedFloat = new SharedFloat();
                    return SetValue(sharedFloat, value);
                case "System.Int32":
                    SharedInt sharedInt = new SharedInt();
                    return SetValue(sharedInt, value);
                case "System.Int64":
                    SharedLong sharedLong = new SharedLong();
                    return SetValue(sharedLong, value);
                case "System.UInt32":
                    SharedUInt sharedUInt = new SharedUInt();
                    return SetValue(sharedUInt, value);
                case "System.Boolean":
                    SharedBool sharedBool = new SharedBool();
                    return SetValue(sharedBool, value);
                case "System.Collections.Generic.List`1[System.Int64]":
                    SharedListLong sharedListLong = new SharedListLong();
                    return SetValue(sharedListLong, value);
                default:
                    if (s_SharedValueHandle != null)
                    {
                        return s_SharedValueHandle.AutoCreateBlackboardValueFromTValue(value);
                    }
                    break;
            }

            Log.Error("AutoCreateBlackboardValueFromTValue: " + valueType + " not support!");
            return null;
        }

        public static ASharedValue SetValue<T>(ASharedValue sharedValue, T v)
        {
            if (sharedValue is ASharedValueBase<T> sharedValueBase)
            {
                sharedValueBase.SetValueFrom(v);
                return sharedValueBase;
            }
            
            return null;
        }

        /// <summary>
        /// 从blackBoardValue中拷贝数据到self
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sharedValue"></param>
        public static void SetValueFrom(in ASharedValue self, ASharedValue sharedValue)
        {
            if (sharedValue == null)
            {
                Log.Error($"blackBoardValue is null");
                return;
            }

            if (self.ValueType != sharedValue.ValueType)
            {
                Log.Error($"Values cannot be copied from the blackboard because they are of different value types, self：{self.ValueType} blackBoardValue: {sharedValue.ValueType}");
            }

            self.SetValueFrom(sharedValue);
        }

        public static bool Compare(ASharedValue lhs, ASharedValue rhs, Operator op)
        {
            switch (op)
            {
                case Operator.IsSet: return true;
                case Operator.IsEqual:
                {
                    switch (lhs)
                    {
                        case SharedBool sharedValue:
                            return sharedValue == rhs as SharedBool;
                        case SharedFloat sharedValue:
                            return sharedValue == rhs as SharedFloat;
                        case SharedInt sharedValue:
                            return sharedValue == rhs as SharedInt;
                        case SharedUInt sharedValue:
                            return sharedValue == rhs as SharedUInt;
                        case SharedString sharedValue:
                            return sharedValue == rhs as SharedString;
                        case SharedLong sharedValue:
                            return sharedValue == rhs as SharedLong;
                        case SharedListLong sharedValue:
                            return sharedValue == rhs as SharedListLong;
                        default:
                            return SharedValueCompare(lhs, rhs, op);
                    }
                }
                case Operator.IsNotEqual:
                {
                    switch (lhs)
                    {
                        case SharedBool sharedValue:
                            return sharedValue != rhs as SharedBool;
                        case SharedFloat sharedValue:
                            return sharedValue != rhs as SharedFloat;
                        case SharedInt sharedValue:
                            return sharedValue != rhs as SharedInt;
                        case SharedUInt sharedValue:
                            return sharedValue != rhs as SharedUInt;
                        case SharedString sharedValue:
                            return sharedValue != rhs as SharedString;
                        case SharedLong sharedValue:
                            return sharedValue != rhs as SharedLong;
                        case SharedListLong sharedValue:
                            return sharedValue != rhs as SharedListLong;
                        default:
                            return SharedValueCompare(lhs, rhs, op);
                    }
                }

                case Operator.IsGreaterOrEqual:
                {
                    switch (lhs)
                    {
                        case SharedBool sharedValue:
                            return (rhs as SharedBool) >= sharedValue;
                        case SharedFloat sharedValue:
                            return (rhs as SharedFloat) >= sharedValue;
                        case SharedInt sharedValue:
                            return (rhs as SharedInt) >= sharedValue;
                        case SharedUInt sharedValue:
                            return (rhs as SharedUInt) >= sharedValue;
                        case SharedString sharedValue:
                            return (rhs as SharedString) >= sharedValue;
                        case SharedLong sharedValue:
                            return (rhs as SharedLong) >= sharedValue;
                        case SharedListLong sharedValue:
                            return (rhs as SharedListLong) >= sharedValue;
                        default:
                            return SharedValueCompare(lhs, rhs, op);
                    }
                }

                case Operator.IsGreater:
                {
                    switch (lhs)
                    {
                        case SharedBool sharedValue:
                            return (rhs as SharedBool) > sharedValue;
                        case SharedFloat sharedValue:
                            return (rhs as SharedFloat) > sharedValue;
                        case SharedInt sharedValue:
                            return (rhs as SharedInt) > sharedValue;
                        case SharedUInt sharedValue:
                            return (rhs as SharedUInt) > sharedValue;
                        case SharedString sharedValue:
                            return (rhs as SharedString) > sharedValue;
                        case SharedLong sharedValue:
                            return (rhs as SharedLong) > sharedValue;
                        case SharedListLong sharedValue:
                            return (rhs as SharedListLong) > sharedValue;
                        default:
                            return SharedValueCompare(lhs, rhs, op);
                    }
                }

                case Operator.IsSmallerOrEqual:
                    switch (lhs)
                    {
                        case SharedBool sharedValue:
                            return (rhs as SharedBool) <= sharedValue;
                        case SharedFloat sharedValue:
                            return (rhs as SharedFloat) <= sharedValue;
                        case SharedInt sharedValue:
                            return (rhs as SharedInt) <= sharedValue;
                        case SharedUInt sharedValue:
                            return (rhs as SharedUInt) <= sharedValue;
                        case SharedString sharedValue:
                            return (rhs as SharedString) <= sharedValue;
                        case SharedLong sharedValue:
                            return (rhs as SharedLong) <= sharedValue;
                        case SharedListLong sharedValue:
                            return (rhs as SharedListLong) <= sharedValue;
                        default:
                            return SharedValueCompare(lhs, rhs, op);
                    }
                case Operator.IsSmaller:
                    switch (lhs)
                    {
                        case SharedBool sharedValue:
                            return (rhs as SharedBool) < sharedValue;
                        case SharedFloat sharedValue:
                            return (rhs as SharedFloat) < sharedValue;
                        case SharedInt sharedValue:
                            return (rhs as SharedInt) < sharedValue;
                        case SharedUInt sharedValue:
                            return (rhs as SharedUInt) < sharedValue;
                        case SharedString sharedValue:
                            return (rhs as SharedString) < sharedValue;
                        case SharedLong sharedValue:
                            return (rhs as SharedLong) < sharedValue;
                        case SharedListLong sharedValue:
                            return (rhs as SharedListLong) < sharedValue;
                        default:
                            return SharedValueCompare(lhs, rhs, op);
                    }

                case Operator.IsNotSet:
                case Operator.AlwaysTrue:
                default: return false;
            }
        }
        
        private static bool SharedValueCompare(ASharedValue lhs, ASharedValue rhs, Operator op)
        {
            if (s_SharedValueHandle != null)
            {
                return s_SharedValueHandle.Compare(lhs, rhs, op);
            }
            Log.Error($"{lhs.GetType()} is not supported");
            return false;
        }
    }
}