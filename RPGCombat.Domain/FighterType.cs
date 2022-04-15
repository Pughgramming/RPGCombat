using System;
using System.Collections.Generic;
using System.Text;

namespace RPGCombat.Domain
{
    public class FighterType
    {
        public int FighterTypeId { get; set; }
        public string Type { get; set; }
        public int RangeRequiredForAttack { get; set; }
    }
}
