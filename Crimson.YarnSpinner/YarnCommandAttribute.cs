using System;

namespace Crimson.YarnSpinner
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class YarnCommandAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the command, as it exists in Yarn.
        /// </summary>
        /// <remarks>
        /// This value does not have to be the same as the name of the
        /// method. For example, you could have a method named "`WalkToPoint`",
        /// and expose it to Yarn as a command named "`walk_to_point`".
        /// </remarks>
        public string CommandString { get; set; }

        public YarnCommandAttribute(string commandString) => CommandString = commandString;
    }
}