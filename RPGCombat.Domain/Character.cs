using RPGCombat.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;


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

            //subscribe to domain events so handlers are called
            Bus.Subscribe<HealCharacter>(Heal);
            Bus.Subscribe<AttackCharacter>(Attack);
        }

        public int CharacterId { get; set; }
        public int Level { get; set; }
        public int Range { get; set; }
        public List<Faction> Factions { get; set; }

        private void Heal(HealCharacter healEvent)
        {
            //check if alive
            if (!Alive) 
                return;

            //check if we aren't healing ourself, that the other character is in our faction
            if (!HealingSelf(healEvent) 
                && !IsSameFaction(healEvent.HealingCharacter, healEvent.CharacterToHeal)) 
                return;

            //add health 
            Health += healEvent.HealthGain;

            //ensure we don't go over max health
            if (Health > MaxHealth) 
                Health = MaxHealth;
        }

        private bool HealingSelf(HealCharacter healEvent)
        {
            //check if healing self
            return healEvent.HealingCharacter.CharacterId == healEvent.CharacterToHeal.CharacterId;
        }

        private void Attack(AttackCharacter attackEvent)
        {
            //check if:
            // 1. I'm attacker
            // 2. I'm not being attacked
            // 3. I'm in the same faction as character being attacked
            // 4. Character is in range
            if (IsAttackingSelf(attackEvent)) 
                return;
            if (IsSameFaction(attackEvent.AttackingCharacter, attackEvent.CharacterToAttack)) 
                return;
            if (IsInRange(attackEvent)) 
                return;

            //if rule checks pass, then determine the damage reduction or multiplier
            Health -= DetermineDamage(attackEvent);

            //Die if health falls to zero
            if (Health <= 0) Die();
        }

        private bool IsAttackingSelf(AttackCharacter @event)
        {
            return @event.AttackingCharacter.CharacterId == @event.CharacterToAttack.CharacterId;
        }

        private bool IsSameFaction(Character characterOne, Character characterTwo)
        {
            var boolean = characterOne.Factions.Select(x => x.Name).Intersect(characterTwo.Factions.Select(y => y.Name)).Any();
            return boolean;
        }

        private static bool IsInRange(AttackCharacter @event)
        {
            //check if given range from event is within the attacking characters range.
            return @event.Range > @event.AttackingCharacter.Range;
        }

        private int DetermineDamage(AttackCharacter @event)
        {
            // if character being attacked is 5 levels or more higher than attacker / half damage.
            if ((Level - @event.AttackingCharacter.Level) >= 5) 
                return @event.Damage / 2;

            //if the attacking characters level is 5+ higher than attacked double damage.
            if ((@event.AttackingCharacter.Level - Level) >= 5) 
                return @event.Damage * 2;

            return @event.Damage;
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
    }

    internal static class IdGenerator
    {

        private static int nextId = 0;

        public static int Next()
        {
            return nextId++;
        }
    }

    public class MeleeCharacter : Character
    {
        private MeleeCharacter()
        {
            Range = 2;
        }

        public static Character Create()
        {
            return new MeleeCharacter();
        }
    }

    public class RangedCharacter : Character
    {
        private RangedCharacter()
        {
            Range = 20;
        }

        public static Character Create()
        {
            return new RangedCharacter();
        }
    }
}
