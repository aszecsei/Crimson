namespace Crimson.Assertions
{
    public static class Assert
    {
        private static void HandleAssertion(bool condition, string message)
        {
            if (!condition)
            {
                Utils.LogError(message);
                
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    throw new AssertionException(message);
                }
            }
        }

        public static void AreApproximatelyEqual(float expected, float actual)
        {
            AreApproximatelyEqual(expected, actual, Mathf.EPSILON, System.Environment.StackTrace);
        }

        public static void AreApproximatelyEqual(float expected, float actual, string message)
        {
            AreApproximatelyEqual(expected, actual, Mathf.EPSILON, message);
        }

        public static void AreApproximatelyEqual(float expected, float actual, float tolerance)
        {
            AreApproximatelyEqual(expected, actual, tolerance, System.Environment.StackTrace);
        }

        public static void AreApproximatelyEqual(float expected, float actual, float tolerance, string message)
        {
            HandleAssertion(Mathf.Approximately(expected, actual, tolerance), message);
        }

        public static void IsFalse(bool condition)
        {
            HandleAssertion(!condition, System.Environment.StackTrace);
        }
        
        public static void IsFalse(bool condition, string message)
        {
            HandleAssertion(!condition, message);
        }

        public static void IsNotNull(object? obj, string message)
        {
            HandleAssertion(obj != null, message);
        }
    }
}