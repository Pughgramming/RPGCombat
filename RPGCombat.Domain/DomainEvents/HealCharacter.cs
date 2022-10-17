using System;
using System.Collections.Generic;
using System.Text;

namespace RPGCombat.Domain.DomainEvents
{
    public class HealCharacter
    {
        public Character HealingCharacter { get; }
        public Character CharacterToHeal { get; }
        public int HealthGain { get; }

        public HealCharacter(Character healingCharacter, Character characterToHeal, int healthGain)
        {
            HealingCharacter = healingCharacter;
            CharacterToHeal = characterToHeal;
            HealthGain = healthGain;
        }
    }
}
