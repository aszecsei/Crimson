using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Crimson.AI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AITag : System.Attribute
    {
        /// <summary>
        /// The full name of the tag.
        /// </summary>
        public string Tag { get; set; }

        public AITag(string tag)
        {
            Tag = tag;
        }
    }
}