using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGCombat;
using System;
using System.Diagnostics;
using System.Linq;
using RPGCombat.Domain;
using RPGCombat.Domain.DomainEvents;

namespace RPGTests
{
    [TestClass]
    public class CharacterTests
    {
        public const int MaxHealth = 1000;

        #region Create
        [TestMethod]
        public void Character_Create_Properly()
        {
            //Create character and variables to test
            Bus.Clear();
            Character character = new Character();

            int levelExpected = 1;
            bool isAlive = true;

            Assert.AreEqual(MaxHealth, character.Health, "Health not created properly");
            Assert.AreEqual(levelExpected, character.Level, "Level not created properly");
            Assert.AreEqual(isAlive, character.Alive, "Character is dead on start..");
        }
        #endregion

        #region Attack/Damage
        [TestMethod]
        public void Character_Attack_DealsDamage()
        {
            //create characters
            Character character = new Character();
            var rangedCharacter = RangedCharacter.Create();

            //attack
            Bus.Raise(new AttackCharacter(rangedCharacter, character, 150, 10));
            //2's health should below 1000 now
            Assert.AreNotEqual(MaxHealth, character.Health, "Damage was not dealt.");

        }

        [TestMethod]
        public void Character_Attack_KillsCharacterAndDoesntGoUnderZeroHealth()
        {
            //create character and other variables
            Character character = new Character();
            var rangedFighter = RangedCharacter.Create();
            int healthAtZero = 0;

            //kill them
            Bus.Raise(new AttackCharacter(rangedFighter, character, 1001, 10));

            Assert.IsFalse(character.Alive, "Character didn't die.");
            //health should never go below zero
            Assert.AreEqual(healthAtZero, character.Health, "Damage was below 0.");
        }

        [TestMethod]
        public void Character_Attack_CannotAttackSelf()
        {
            //create characters
            Character character = new Character() { Range = 1};

            //attack
            Bus.Raise(new AttackCharacter(character, character, 1000, 1));

            //assert an exception is thrown when attack is called on itself
            Assert.AreEqual(MaxHealth, character.Health, "Damaged itself.");
        }

        [TestMethod]
        public void Character_Attack_DamageDoubledIfFiveLevelsAbove()
        {
            //create varaibles and set levels
            var rangedCharacter = RangedCharacter.Create();
            Character characterTwo = new Character() { Range = 1};
            rangedCharacter.Level = 11;
            var healthAfterDamage = 800;

            //attack. Should double damage, so should be from 1000 - 200.
            Bus.Raise(new AttackCharacter(rangedCharacter, characterTwo, 100, 5));

            //assert
            Assert.AreEqual(healthAfterDamage, characterTwo.Health, "Damage didn't double");
        }

        [TestMethod]
        public void Character_Attack_HalvedIfFiveLevelsBelow()
        {
            //create varaibles and set levels
            var rangedCharacter = RangedCharacter.Create();
            Character characterTwo = new Character() { Range = 1 };
            characterTwo.Level = 11;
            var healthAfterDamage = 950;

            //attack. Should halve damage, so should be from 1000 - 50.
            Bus.Raise(new AttackCharacter(rangedCharacter, characterTwo, 100, 5));

            //assert
            Assert.AreEqual(healthAfterDamage, characterTwo.Health, "Damage didn't halve");
        }

        [TestMethod]
        public void MeleeCharacter_Attack_WhenInRangeForAttack()
        {
            //create varaibles and set levels
            var meleeCharacter = MeleeCharacter.Create();
            Character characterTwo = new Character();

            //attack
            Bus.Raise(new AttackCharacter(meleeCharacter, characterTwo, 100, 2));

            //assert target was hit
            Assert.AreNotEqual(MaxHealth, characterTwo.Health, "Character was not attacked.");
        }

        [TestMethod]
        public void RangedCharacter_Attack_WhenInRangeForAttack()
        {
            //create varaibles and set levels
            var rangedCharacter = RangedCharacter.Create();
            Character characterTwo = new Character();

            //attack
            Bus.Raise(new AttackCharacter(rangedCharacter, characterTwo, 100, 20));

            //assert target was hit
            Assert.AreNotEqual(MaxHealth, characterTwo.Health, "Character was not attacked.");
        }

        [TestMethod]
        public void MeleeCharacter_Attack_FailsWhenNotInRange()
        {
            //create varaibles and set levels
            var meleeCharacter = MeleeCharacter.Create();
            Character characterTwo = new Character();

            //attack
            Bus.Raise(new AttackCharacter(meleeCharacter, characterTwo, 100, 3));

            //assert target was not hit
            Assert.AreEqual(MaxHealth, characterTwo.Health, "Character was attacked.");
        }

        [TestMethod]
        public void RangedCharacter_Attack_FailsWhenNotInRange()
        {
            //create varaibles and set levels
            var rangedCharacter = RangedCharacter.Create();
            Character characterTwo = new Character();

            //attack
            Bus.Raise(new AttackCharacter(rangedCharacter, characterTwo, 100, 21));

            //assert target was not hit
            Assert.AreEqual(MaxHealth, characterTwo.Health, "Character was attacked.");
        }

        [TestMethod]
        public void Character_Attack_CannotAttackAlly()
        {
            //create characters
            Character character = new Character() { Range = 1};
            Character characterTwo = new Character() { Range = 1};
            character.JoinFaction("Hawks");
            characterTwo.JoinFaction("Hawks");

            //action
            Bus.Raise(new AttackCharacter(character, characterTwo, 100, 1));

            //assert an exception is thrown when attacking an ally
            Assert.AreEqual(MaxHealth, characterTwo.Health, "Character two was attacked.");
        }

        [TestMethod]
        public void Character_Attacks_PropAndPropIsDestroyed()
        {
            //create actors
            Character character = new Character();
            Prop prop = new Prop() { PropName = "Tree" };
            int maxPropHealth = 2000;

            //attack prop
            Bus.Raise(new AttackProp(character, prop, 2000));

            //prop is attacked and destroyed
            Assert.AreNotEqual(maxPropHealth, prop.Health, "Prop was not attacked");
            Assert.IsFalse(prop.Alive, "Prop was not destroyed");
        }

        #endregion

        #region Heal
        [TestMethod]
        public void Character_Heals_AndDoesntGoOverMax()
        {
            //create characters and other varaibles
            Character character = new Character();

            //set character health
            character.Health = 900;

            var healthAfterDamage = character.Health;

            //overheal
            Bus.Raise(new HealCharacter(character, character, 101));

            //assert character healed and didn't go over max
            Assert.AreNotEqual(healthAfterDamage, character.Health, "Character was not healed");
            Assert.AreEqual(MaxHealth, character.Health, "Character was overhealed");
        }

        [TestMethod]
        public void Character_Heals_AnotherCharacterInFaction()
        {
            //create characters and other varaibles
            Character character = new Character();
            Character characterTwo = new Character();
            character.JoinFaction("Hawks");
            characterTwo.JoinFaction("Hawks");

            //set character health
            character.Health = 900;

            var healthAfterDamage = character.Health;

            //heal
            Bus.Raise(new HealCharacter(character, character, 100));

            //assert character healed and didn't go over max
            Assert.AreNotEqual(healthAfterDamage, character.Health, "Character was not healed");
        }

        [TestMethod]
        public void Character_Heal_CannotHealAnotherWhenNotInFaction()
        {
            //setup
            Character character = new Character();
            Character characterTwo = new Character();
            characterTwo.Health = 500;

            //heal
            Bus.Raise(new HealCharacter(character, characterTwo, 50));

            //assert an exception is thrown when heal is called on another when characters don't have factions
            Assert.AreEqual(500, characterTwo.Health, "Character was healed.");
        }

        [TestMethod]
        public void Character_Heal_CannotHealAnotherOutsideOfFaction()
        {
            //create characters and other varaibles
            Character character = new Character();
            Character characterTwo = new Character();
            character.JoinFaction("Seals");
            characterTwo.JoinFaction("Hawks");

            //set character health
            characterTwo.Health = 900;

            //heal
            Bus.Raise(new HealCharacter(character, characterTwo, 100));

            //assert character healed and didn't go over max
            Assert.AreEqual(900, characterTwo.Health, "Character was not healed");
        }
        #endregion
    }
}
