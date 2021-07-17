using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VA.LogReader
{
    public class TalentTree
    {
        [TalentPosition(1, 1)]
        public bool R1C1 { get; private set; }
        [TalentPosition(1, 2)]
        public bool R1C2 { get; private set; }
        [TalentPosition(1, 3)]
        public bool R1C3 { get; private set; }

        [TalentPosition(2, 1)]
        public bool R2C1 { get; private set; }
        [TalentPosition(2, 2)]
        public bool R2C2 { get; private set; }
        [TalentPosition(2, 3)]
        public bool R2C3 { get; private set; }

        [TalentPosition(3, 1)]
        public bool R3C1 { get; private set; }
        [TalentPosition(3, 2)]
        public bool R3C2 { get; private set; }
        [TalentPosition(3, 3)]
        public bool R3C3 { get; private set; }

        [TalentPosition(4, 1)]
        public bool R4C1 { get; private set; }
        [TalentPosition(4, 2)]
        public bool R4C2 { get; private set; }
        [TalentPosition(4, 3)]
        public bool R4C3 { get; private set; }

        [TalentPosition(5, 1)]
        public bool R5C1 { get; private set; }
        [TalentPosition(5, 2)]
        public bool R5C2 { get; private set; }
        [TalentPosition(5, 3)]
        public bool R5C3 { get; private set; }

        [TalentPosition(6, 1)]
        public bool R6C1 { get; private set; }
        [TalentPosition(6, 2)]
        public bool R6C2 { get; private set; }
        [TalentPosition(6, 3)]
        public bool R6C3 { get; private set; }

        public TalentTree(Talent_Tree talentEvent)
        {
            R1C1 = talentEvent.R1C1;
            R1C2 = talentEvent.R1C2;
            R1C3 = talentEvent.R1C3;

            R2C1 = talentEvent.R2C1;
            R2C2 = talentEvent.R2C2;
            R2C3 = talentEvent.R2C3;

            R3C1 = talentEvent.R3C1;
            R3C2 = talentEvent.R3C2;
            R3C3 = talentEvent.R3C3;

            R4C1 = talentEvent.R4C1;
            R4C2 = talentEvent.R4C2;
            R4C3 = talentEvent.R4C3;

            R5C1 = talentEvent.R5C1;
            R5C2 = talentEvent.R5C2;
            R5C3 = talentEvent.R5C3;

            R6C1 = talentEvent.R6C1;
            R6C2 = talentEvent.R6C2;
            R6C3 = talentEvent.R6C3;
        }

        public IEnumerable<TalentPosition> AllocatedTalents()
        {
            foreach (var prop in typeof(TalentTree).GetProperties())
            {
                if ((bool)prop.GetValue(this))
                {
                    yield return ((TalentPositionAttribute)Attribute.GetCustomAttribute(prop, typeof(TalentPositionAttribute))).Position;
                }
            }
        }

        public IEnumerable<TalentPosition> AddedTalents(TalentTree old)
        {
            foreach(var prop in typeof(TalentTree).GetProperties())
            {
                bool currentState = (bool)prop.GetValue(this);
                bool oldState = (bool)prop.GetValue(old);
                if (currentState && !oldState)
                {
                    yield return ((TalentPositionAttribute)Attribute.GetCustomAttribute(prop, typeof(TalentPositionAttribute))).Position;
                }
            }
        }
    }

    public struct TalentPosition
    {
        public int Row;
        public int Column;

        public TalentPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

    public class TalentPositionAttribute : Attribute
    {
        public TalentPosition Position { get; }
        public TalentPositionAttribute(int row, int column)
        {
            Position = new TalentPosition(row, column);
        }
    }
}
