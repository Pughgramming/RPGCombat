using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGCombat;
using System;
using System.Diagnostics;
using System.Linq;
using RPGCombat.Domain;

namespace RPGTests
{
    [TestClass]
    public class CharacterTests
    {
        #region Create
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
        #endregion

        #region Attack/Damage
        [TestMethod]
        public void DealtDamage_CharacterAttack_DealsDamage()
        {
            //create characters
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 1;
            character.FighterType = new FighterType() { Type = "Melee", RangeRequiredForAttack = 2 };
            int maxHealth = 1000;

            //attack
            character.Attack(characterTwo);

            //2's health should below 1000 now
            Assert.AreNotEqual(maxHealth, characterTwo.Health, "Damage was not dealt.");

        }

        [TestMethod]
        public void DealtDamage_CharacterAttack_KillsCharacterAndDoesntGoUnderZeroHealth()
        {
            //create character and other variables
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 1;
            character.FighterType = new FighterType() { Type = "Melee", RangeRequiredForAttack = 2 };

            int damage = 1001;
            int healthAtZero = 0;
            bool isAlive = false;

            //kill them
            character.Attack(characterTwo, damage);

            Assert.AreEqual(isAlive, characterTwo.Alive, "Character didn't die.");
            //health should never go below zero
            Assert.AreEqual(healthAtZero, characterTwo.Health, "Damage was below 0.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_CharacterCannotAttackItself()
        {
            //create characters
            Character character = new Character();

            //assert an exception is thrown when attack is called on itself
            Assert.ThrowsException<ApplicationException>(() => character.Attack(character), "Exception was not thrown.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_AttackerDamageDoubledIfFiveLevelsAbove()
        {
            //create varaibles and set levels
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 1;
            character.FighterType = new FighterType() { Type = "Melee", RangeRequiredForAttack = 2 };
            character.Level = 11;
            var healthAfterDamage = 800;

            //attack. Should double damage, so should be from 1000 - 200.
            character.Attack(characterTwo);

            //assert
            Assert.AreEqual(healthAfterDamage, characterTwo.Health, "Damage didn't double");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_AttackerDamageHalvedIfFiveLevelsBelow()
        {
            //create varaibles and set levels
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 1;
            character.FighterType = new FighterType() { Type = "Melee", RangeRequiredForAttack = 2 };
            characterTwo.Level = 11;
            var healthAfterDamage = 950;

            //attack. Should double damage, so should be from 1000 - 200.
            character.Attack(characterTwo);

            //assert
            Assert.AreEqual(healthAfterDamage, characterTwo.Health, "Damage didn't double");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_MeleeFighterIsInRangeForAttack()
        {
            //create varaibles and set levels
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 1;
            character.FighterType = new FighterType() { Type = "Melee", RangeRequiredForAttack = 2 };
            int maxHealth = 1000;

            //attack
            character.Attack(characterTwo);

            //assert target was hit
            Assert.AreNotEqual(maxHealth, characterTwo.Health, "Character was not attacked.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_RangedFighterIsInRangeForAttack()
        {
            //create varaibles and set levels
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 14;
            character.FighterType = new FighterType() { Type = "Ranged", RangeRequiredForAttack = 20 };
            int maxHealth = 1000;

            //attack
            character.Attack(characterTwo);

            //assert target was hit
            Assert.AreNotEqual(maxHealth, characterTwo.Health, "Character was not attacked.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_MeleeFighterIsNotInRangeForAttack()
        {
            //create varaibles and set levels
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 14;
            character.FighterType = new FighterType() { Type = "Melee", RangeRequiredForAttack = 2 };
            int maxHealth = 1000;

            //attack
            character.Attack(characterTwo);

            //assert target was not hit
            Assert.AreEqual(maxHealth, characterTwo.Health, "Character was attacked.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_RangedFighterIsNotInRangeForAttack()
        {
            //create varaibles and set levels
            Character character = new Character();
            Character characterTwo = new Character();
            character.RangeToTarget = 30;
            character.FighterType = new FighterType() { Type = "Ranged", RangeRequiredForAttack = 25 };
            int maxHealth = 1000;

            //attack
            character.Attack(characterTwo);

            //assert target was not hit
            Assert.AreEqual(maxHealth, characterTwo.Health, "Character was attacked.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_CharacterCannotAttackAlly()
        {
            //create characters
            Character character = new Character();
            Character characterTwo = new Character();

            character.JoinFaction("Hawks");
            characterTwo.JoinFaction("Hawks");

            //assert an exception is thrown when attacking an ally
            Assert.ThrowsException<ApplicationException>(() => character.Attack(character), "Exception was not thrown.");
        }

        [TestMethod]
        public void Attack_CharacterAttacks_CharacterAttacksProp()
        {
            //create actors
            Character character = new Character();
            Prop prop = new Prop() { PropName = "Tree" };
            int maxPropHealth = 1000;
            int overrideDamage = 1000;
            bool isTrue = true;

            //attack prop
            character.Attack(prop, overrideDamage);

            //prop is attacked and destroyed
            Assert.AreNotEqual(maxPropHealth, prop.Health, "Prop was not attacked");
            Assert.AreEqual(true, prop.IsDestroyed, "Prop was not destroyed");
        }
        #endregion

        #region Heal
        [TestMethod]
        public void Heal_CharacterHeals_HealsCharacterAndDoesntGoOverMax()
        {
            //create characters and other varaibles
            Character character = new Character();
            int maxHealth = 1000;
            int moreThanMax = 1001;
            int damage = 100;

            //set character health
            character.Health = damage;

            var healthAfterDamage = character.Health;

            //overheal
            character.Heal(character, moreThanMax);

            //assert character healed and didn't go over max
            Assert.AreNotEqual(healthAfterDamage, character.Health, "Character was not healed");
            Assert.AreEqual(maxHealth, character.Health, "Character was overhealed");
        }

        [TestMethod]
        public void Heal_CharacterHeals_CharacterCannotHealAnotherWhenNotInFaction()
        {
            Character character = new Character();
            Character characterTwo = new Character();

            //assert an exception is thrown when heal is called on another when characters don't have factions
            Assert.ThrowsException<ApplicationException>(() => character.Heal(characterTwo), "Exception was not thrown.");
        }

        [TestMethod]
        public void Heal_CharacterHeals_CharacterCannotHealAnotherOutSideOfFaction()
        {
            Character character = new Character();
            Character characterTwo = new Character();
            character.JoinFaction("Seals");
            characterTwo.JoinFaction("Hawks");

            //assert an exception is thrown when heal is called on another when characters aren't in the same faction
            Assert.ThrowsException<ApplicationException>(() => character.Heal(characterTwo), "Exception was not thrown.");
        }
        #endregion
    }
}
