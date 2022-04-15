using System;
using System.Collections.Generic;
using System.Text;

namespace RPGCombat.Domain
{
    public class Prop
    {
        public Prop()
        {
            Health = 1000;
            IsDestroyed = false;
        }
        public Prop(int health)
        {
            Health = health;
            IsDestroyed = false;
        }

        public int PropId { get; set; }
        public string PropName { get; set; }
        public int Health { get; set; }
        public bool IsDestroyed { get; set; }
    }
}
