using System;

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
        }

        public int CharacterId { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        public bool Alive { get; set; }
        public int RangeToTarget { get; set; }
        public FighterType FighterType { get; set; }


        public void Attack(Character characterToAttack, int overrideDamage = 0)
        {
            //character cannot attack itself.
            if (characterToAttack == this)
                throw new ApplicationException("Character cannot attack itself!");

            //ensure target is within range
            if(this.RangeToTarget <= this.FighterType.RangeRequiredForAttack)
            {
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

        public void Heal(Character characterToHeal, int healthReceivedOverride = 0)
        {
            //player can only heal itself
            if (characterToHeal != this)
                throw new ApplicationException("Character cannot heal another!");

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
}
