using System;
using System.Collections.Generic;
using System.Linq;
using RPGCombat.Domain.DomainEvents;

namespace RPGCombat.Domain
{
    /// <summary>
    /// Refactored to add domain event / subscriber system to clean up code.
    /// Allows for much smoother rule checking and testing.
    /// Also makes for much more readable code.
    /// </summary>
    public class Character : Targetable
    {
        private const int MaxHealth = 1000;

        public Character()
        {
            CharacterId = IdGenerator.Next();
            Health = 1000;
            Level = 1;
            Alive = true;
            Factions = new List<Faction>();

            Bus.Subscribe<HealCharacter>(Heal);
            Bus.Subscribe<AttackCharacter>(Attack);
        }

        public int CharacterId { get; set; }
        public int Level { get; set; }
        public int Range { get; set; }
        public List<Faction> Factions { get; set; }

        private void Heal(HealCharacter healCharacterEvent)
        {
            if (!Alive) return;
            if (!HealingSelf(healCharacterEvent) && !IsSameFaction(healCharacterEvent.HealingCharacter)) return;
            Health += healCharacterEvent.HealthGain;
            if (Health > MaxHealth) Health = MaxHealth;
        }

        private bool HealingSelf(HealCharacter healCharacterEvent)
        {
            return healCharacterEvent.HealingCharacter.CharacterId == CharacterId && healCharacterEvent.CharacterToHeal.CharacterId == CharacterId;
        }

        private void Attack(AttackCharacter attackCharacterEvent)
        {
            if (IsAttacker(attackCharacterEvent.AttackingCharacter)) return;
            if (IsNotAttacked(attackCharacterEvent.CharacterToAttack)) return;
            if (IsSameFaction(attackCharacterEvent.AttackingCharacter)) return;
            if (IsInRange(attackCharacterEvent)) return;
            Health -= DamageReduction(attackCharacterEvent);
            if (Health <= 0) Die();
        }

        private bool IsAttacker(Character attacker)
        {
            return attacker.CharacterId == CharacterId;
        }

        private bool IsNotAttacked(Character attacked)
        {
            return attacked.CharacterId != CharacterId;
        }

        private bool IsSameFaction(Character attacker)
        {
            return Factions.Any(attacker.IsInFaction);
        }

        private bool IsInFaction(Faction faction)
        {
            return Factions.Contains(faction);
        }

        private static bool IsInRange(AttackCharacter attackCharacterEvent)
        {
            return attackCharacterEvent.Range > attackCharacterEvent.AttackingCharacter.Range;
        }

        private int DamageReduction(AttackCharacter attackCharacterEvent)
        {
            if ((Level - attackCharacterEvent.AttackingCharacter.Level) >= 5) return attackCharacterEvent.Damage / 2;
            if ((attackCharacterEvent.AttackingCharacter.Level - Level) >= 5) return attackCharacterEvent.Damage * 2;
            return attackCharacterEvent.Damage;
        }

        private void Die()
        {
            Alive = false;
            Health = 0;

            //unsubscribe from events on death
            Bus.Unsubscribe<AttackCharacter>(Attack);
        }

        public void JoinFaction(string factionName)
        {
            this.Factions.Add(new Faction()
            {
                Name = factionName
            });
        }

        public void LeaveFaction(string factionName)
        {
            var factionToLeave = this.Factions.Where(x => x.Name == factionName).FirstOrDefault();

            if (factionToLeave != null)
            {
                this.Factions.Remove(factionToLeave);
            }
            else
            {
                throw new ApplicationException("Faction: " + factionName + " does not exist.");
            }

        }
        public void LevelUp()
        {
            Level++;
        }

    }

    internal static class IdGenerator
    {

        private static int nextId = 0;

        public static int Next()
        {
            return nextId++;
        }
    }

    public class MeleeFighter : Character
    {
        private MeleeFighter()
        {
            Range = 2;
        }

        public static Character Create()
        {
            return new MeleeFighter();
        }
    }

    public class RangedFighter : Character
    {
        private RangedFighter()
        {
            Range = 20;
        }

        public static Character Create()
        {
            return new RangedFighter();
        }
    }
}
