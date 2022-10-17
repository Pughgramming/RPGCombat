using System;
using System.Collections.Generic;
using System.Text;

namespace RPGCombat.Domain.DomainEvents
{

    public class AttackProp
    {
        public Character AttackingCharacter { get; }
        public Prop PropToAttack { get; }
        public int Damage { get; }

        public AttackProp(Character attackingCharacter, Prop propToAttack, int damage)
        {
            AttackingCharacter = attackingCharacter;
            PropToAttack = propToAttack;
            Damage = damage;
        }
    }

}
