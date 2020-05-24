using System;
using System.Collections.Generic;

namespace Crimson
{
    public class BitTag
    {
        internal static int TotalTags;
        internal static BitTag[] byID = new BitTag[32];

        private static readonly Dictionary<string, BitTag> byName =
            new Dictionary<string, BitTag>(StringComparer.OrdinalIgnoreCase);

        public BitTag(string name)
        {
#if DEBUG
            if (TotalTags >= 32) throw new Exception("Maximum tag limit of 32 exceeded!");

            if (byName.ContainsKey(name)) throw new Exception("Two tags defined with the same name: '" + name + "'!");
#endif

            ID = TotalTags;
            Value = 1 << TotalTags;
            Name = name;

            byID[ID] = this;
            byName[name] = this;

            TotalTags++;
        }

        public int ID { get; }
        public int Value { get; }
        public string Name { get; }

        public static BitTag Get(string name)
        {
#if DEBUG
            if (!byName.ContainsKey(name)) throw new Exception("No tag with the name '" + name + "' has been defined!");
#endif
            return byName[name];
        }

        public static implicit operator int(BitTag tag)
        {
            return tag.Value;
        }
    }
}