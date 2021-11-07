using System.Collections.Generic;
using System.Linq;

namespace VA.LogReader
{
    public class ItemDetails
    {
        public HashSet<TRAIT> Traits { get; set; } = new HashSet<TRAIT>();
        public List<Property> Properties { get; set; } = new List<Property>();

        public override bool Equals(object obj)
        {
            if(obj is ItemDetails other)
            {
                if (!Traits.IsSubsetOf(other.Traits) || !other.Traits.IsSubsetOf(Traits)) return false;

                if (Properties.Count != other.Properties.Count) return false;
                foreach(var prop in Properties)
                {
                    if(!other.Properties.Any(otherProp => otherProp.Equals(prop)))
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }
    }
}
