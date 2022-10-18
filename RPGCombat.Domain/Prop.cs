using RPGCombat.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPGCombat.Domain
{
    public class Prop : Targetable
    {
        public Prop()
        {
            Health = 1000;
            Alive = false;

            //Subscribe to attack event
            Bus.Subscribe<AttackProp>(Attack);
        }
        public Prop(int health)
        {
            Health = health;
            Alive = false;

            //Subscribe to attack event
            Bus.Subscribe<AttackProp>(Attack);
        }

        public int PropId { get; set; }
        public string PropName { get; set; }

        private void Attack(AttackProp attackObjectEvent)
        {
            Health -= attackObjectEvent.Damage;
            //if health reaches 0 destroy object
            if (Health <= 0) Destroy();
        }

        private void Destroy()
        {
            Health = 0;
            Alive = false;

            //unsubscribe from attack event for this prop, since it's destroyed.
            Bus.Unsubscribe<AttackProp>(Attack);
        }
    }
}
