using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGCombat;
using System.Diagnostics;

namespace RPGTests
{
    [TestClass]
    public class CharacterTests
    {
        [TestMethod]
        public void Created_Character_CreatedProperly()
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
        public void DealtDamage_CharacterAttack_DealsDamageAndDoesntGoUnder0Health()
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
        public void DealtDamage_CharacterAttack_KillsCharacter()
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
        public void Heal_CharacterHeals_HealsOtherCharacterAndDoesntGoOverMax()
        {
            //create characters and other varaibles
            Character character = new Character();
            Character characterTwo = new Character();
            int maxHealth = 1000;
            int moreThanMax = 1001;

            //get the damage 1 will do to 2
            int damageToDeal = character.Attack();
            characterTwo.DealtDamage(damageToDeal);

            var healthAtDamage = characterTwo.Health;

            //heal
            int healthToHeal = character.HealOther();
            characterTwo.Heal(healthToHeal);

            var healthAfterHeal = characterTwo.Health;

            //over heal
            characterTwo.Heal(moreThanMax);

            //Character twos health after heal should not equal the health after damage.
            Assert.AreNotEqual(healthAtDamage, healthAfterHeal, "Character was not healed.");
            //health should never go above 1000
            Assert.AreEqual(maxHealth, character.Health, "Health exceeded max.");
        }
    }
}
