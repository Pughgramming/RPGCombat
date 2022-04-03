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

        public int Health { get; set; }
        public int Level { get; set; }
        public bool Alive { get; set; }

        public void DealtDamage(int damageDealt)
        {
            //ensure we don't go under 0
            this.Health = Math.Max(this.Health - damageDealt, 0);

            //if health reached 0 : dies
            if (this.Health == 0)
            {
                this.Alive = false;
            }
        }

        /// <summary>
        /// Returns damage to deal.
        /// </summary>
        /// <returns></returns>
        public int Attack()
        {
            Random random = new Random();
            return random.Next(10, 100);
        }

        public void Heal(int healthReceived)
        {
            if (!this.Alive)
            {
                throw new ApplicationException("This player is dead and cannot be healed.");
            }

            //ensure we don't go over 1000
            this.Health = Math.Min(this.Health + healthReceived, 1000);

        }

        public int HealOther()
        {
            Random random = new Random();
            return random.Next(100, 500);
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
