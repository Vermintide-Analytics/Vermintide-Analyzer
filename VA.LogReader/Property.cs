using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class Property
    {
        public PROPERTY Prop { get; set; }
        public float Value { get; set; }

        public Property(PROPERTY property, float value)
        {
            Prop = property;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if(obj is Property other)
            {
                return other.Prop == Prop && other.Value == Value;
            }
            return false;
        }

        public override string ToString() => $"+{Value:f1}{(Prop.IsPercent() ? "%" : "")} {Prop.ForDisplay()}";
    }
}
