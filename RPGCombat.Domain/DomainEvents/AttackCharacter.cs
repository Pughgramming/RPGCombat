using System;
using System.Collections.Generic;
using System.Text;

namespace RPGCombat.Domain.DomainEvents
{
    public class AttackCharacter
    {
        public Character AttackingCharacter { get; }
        public Character CharacterToAttack { get; }
        public int Damage { get; }
        public int Range { get; }

        public AttackCharacter(Character character, Character characterToAttack, int damage, int range)
        {
            AttackingCharacter = character;
            CharacterToAttack = characterToAttack;
            Damage = damage;
            Range = range;
        }
    }
}
