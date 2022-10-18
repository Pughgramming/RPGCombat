using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGCombat;
using System;
using System.Diagnostics;
using System.Linq;
using RPGCombat.Domain;

namespace RPGCombat.Tests
{
    [TestClass]
    public class FactionTests
    {
        #region Join/Leave
        [TestMethod]
        public void Faction_Join_CharacterCanJoinFaction()
        {
            //create characters
            Character character = new Character();
            string factionName = "Hawks";

            //join faction
            character.JoinFaction(factionName);

            var characterFactionName = character.Factions.Where(x => x.Name == factionName).Select(x => x.Name).FirstOrDefault();
            //assert an exception is thrown when attacking an ally
            Assert.AreEqual(factionName, characterFactionName, "Faction was not joined");
        }

        [TestMethod]
        public void Faction_Leave_CharacterCanLeaveFaction()
        {
            //create characters
            Character character = new Character();
            string factionName = "Hawks";

            //join faction
            character.JoinFaction(factionName);
            //leave faction
            character.LeaveFaction(factionName);

            var characterFactionName = character.Factions.Where(x => x.Name == factionName).Select(x => x.Name).FirstOrDefault();
            //assert an exception is thrown when attacking an ally
            Assert.AreEqual(null, characterFactionName, "Faction was not left");
        }
        #endregion
    }
}
