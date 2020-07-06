namespace Crimson.Assertions
{
    /// <summary>
    /// An exception thrown on a failure. Assertions.Assert._raiseExceptions needs to be set to true.
    /// </summary>
    public class AssertionException : System.Exception
    {
        public AssertionException(string message) : base(message)
        {
            
        }
    }
}