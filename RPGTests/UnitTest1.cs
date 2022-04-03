using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGCombat;

namespace RPGTests
{
    [TestClass]
    public class CharacterTests
    {
        [TestMethod]
        public void Character_Created_Properly()
        {
            //Create character and variables to test
            Character character = new Character();

            int healthExpected = 1000;
            int levelExpected = 1;
            bool isAlive = true;

            Assert.AreEqual(healthExpected, character.Health, "Health not created properly");
            Assert.AreEqual(levelExpected, character.Level, "Level not created properly");
            Assert.AreEqual(isAlive, character.Alive, "Character is dead on start..");
        }

        [TestMethod]
        public void Character_DealsDamage()
        {
            //create characters
            Character character = new Character();
            Character characterTwo = new Character();
            int maxHealth = 1000;
            int zeroHealth = 0;

            //get the damage 1 will do to 2
            int damageToDeal = character.Attack();
            characterTwo.DealtDamage(damageToDeal);

            //get damage over 1000 for character 1 to go to 0
            int damageToDealOver = 1001;
            character.DealtDamage(damageToDealOver);

            //2's health should below 1000 now
            Assert.AreNotEqual(maxHealth, characterTwo.Health, "Damage was not dealt.");
            //health should never go below zero
            Assert.AreEqual(zeroHealth, character.Health, "Damage was below 0.");
        }

        [TestMethod]
        public void Character_Dies()
        {
            //create character and other variables
            Character character = new Character();
            int damage = 1000;
            bool isAlive = false;

            //kill them
            character.DealtDamage(damage);

            Assert.AreEqual(isAlive, character.Alive, "Character didn't die.");
        }

        [TestMethod]
        public void Character_Heals()
        {

        }
    }
}
