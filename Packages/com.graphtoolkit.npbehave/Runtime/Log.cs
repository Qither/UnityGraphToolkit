namespace NPBehave
{
    public static class Log
    {
        public static void Info(string content)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Debug.Log(content);
#else
            System.Console.WriteLine("NPBehave Info：" + content);
#endif
        }
        
        public static void Error(string content)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Debug.LogError(content, null);
#else
            System.Console.WriteLine("NPBehave Error：" + content);
#endif
        }

        public static void AssertIsTrue(bool condition, string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.IsTrue(condition, content);
#else 
            if (!condition)
            {
                System.Console.WriteLine("NPBehave AssertIsTrue：" + content);
            }
#endif
        }

        public static void AssertIsFalse(bool condition, string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.IsFalse(condition, content);
#else 
            if (condition)
            {
                System.Console.WriteLine("NPBehave AssertIsFalse：" + content);
            }
#endif
        }
        
        public static void AssertAreEqual(Node.State expected, Node.State actual ,string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.AreEqual(expected, actual, content);
#else 
            if (expected != actual)
            {
                System.Console.WriteLine("NPBehave AssertAreEqual：" + content);
            }
#endif
        }
        
        public static void AssertAreEqual(object expected, object actual ,string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.AreEqual(expected, actual, content);
#else 
            if (expected != actual)
            {
                System.Console.WriteLine("NPBehave AssertAreEqual：" + content);
            }
#endif
        }
        
        public static void AssertAreNotEqual(Node.State expected, Node.State actual, string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual, content);
#else
            if (expected == actual)
            {
                System.Console.WriteLine("NPBehave AssertAreNotEqual：" + content);
            }
#endif
        }

        public static void AssertAreNotEqual(object expected, object actual, string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.AreNotEqual(expected, actual, content);
#else
            if (expected == actual)
            {
                System.Console.WriteLine("NPBehave AssertAreNotEqual：" + content);
            }
#endif
        }

        public static void AssertIsNotNull(object value, string content = null)
        {
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
            UnityEngine.Assertions.Assert.IsNotNull(value, content);
#else
            if (value == null)
            {
                System.Console.WriteLine("NPBehave AssertIsNotNull：" + content);
            }
#endif
        }
    }
}