using System;
using System.Collections.Generic;
using System.Linq;

namespace RPGCombat
{
    //creating class here for ease.
    public class Character
    {
        public Character()
        {
            Health = 1000;
            Level = 1;
            Alive = true;
            Factions = new List<Faction>();
        }

        public int CharacterId { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        public bool Alive { get; set; }
        public int RangeToTarget { get; set; }
        public FighterType FighterType { get; set; }
        public List<Faction> Factions { get; set; }

        public void Attack(Character characterToAttack, int overrideDamage = 0)
        {
            //character cannot attack itself.
            if (characterToAttack == this)
                throw new ApplicationException("Character cannot attack itself!");

            //ensure target is within range
            if (this.RangeToTarget <= this.FighterType.RangeRequiredForAttack)
            {
                //ensure is not an ally
                foreach(var fac in this.Factions)
                {
                    if (characterToAttack.Factions.Contains(fac))
                    {
                        throw new ApplicationException("Cannot attack an ally!");
                    }
                }

                var damage = 0;
                if (overrideDamage != 0)
                {
                    //subtract damage from health of the character
                    characterToAttack.Health = Math.Max(characterToAttack.Health - overrideDamage, 0);
                }
                else
                {
                    //get damage
                    damage = 100;

                    //if level of defender is 5 or more we half the damage.
                    //if the defender is 5 or more levels under we double it.
                    if (characterToAttack.Level >= this.Level + 5)
                    {
                        damage /= 2;
                    }
                    else if (characterToAttack.Level <= this.Level - 5)
                    {
                        damage *= 2;
                    }

                    //subtract damage from health of the character
                    characterToAttack.Health = Math.Max(characterToAttack.Health - damage, 0);
                }

                //if health reached 0 : dies
                if (characterToAttack.Health == 0)
                {
                    characterToAttack.Alive = false;
                }
            }
        }

        public void Attack(Prop propToAttack, int damage = 0)
        {
            if(damage == 0)
            {
                damage = 100;
            }

            //deal damage
            propToAttack.Health = Math.Max(propToAttack.Health - damage, 0);

            //if props health drops to 0 : destroyed
            if (propToAttack.Health == 0)
            {
                propToAttack.IsDestroyed = true;
            }
        }

        public void Heal(Character characterToHeal, int healthReceivedOverride = 0)
        {
            //player can only heal itself or another ally
            if (characterToHeal != this)
            {
                //if character to heal or the character healing isn't in a faction, throw exception
                if(characterToHeal.Factions.Count() == 0 || this.Factions.Count() == 0)
                    throw new ApplicationException("You can only heal characters in your faction!");

                //check to see if character to heal is in healers faction.
                foreach (var fac in this.Factions)
                {
                    if (!characterToHeal.Factions.Contains(fac))
                    {
                        throw new ApplicationException("You can only heal characters in your faction!");
                    }
                }
            }

            if (!this.Alive)
            {
                throw new ApplicationException("This player is dead and cannot be healed.");
            }

            if (healthReceivedOverride != 0)
            {
                //ensure we don't go over 1000
                this.Health = Math.Min(this.Health + healthReceivedOverride, 1000);
            }
            else
            {
                //get health to heal
                int health = 100;

                //ensure we don't go over 1000
                this.Health = Math.Min(this.Health + health, 1000);
            }

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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class FighterType
    {
        public int FighterTypeId { get; set; }
        public string Type { get; set; }
        public int RangeRequiredForAttack { get; set; }
    }

    public class Faction
    {
        public int FactionId { get; set; }
        public string Name { get; set; }
    }

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
