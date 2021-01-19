using System;
using System.Collections.Generic;
using RPGConsoleTutorialSeries.Items.Interfaces;

namespace RPGConsoleTutorialSeries.Entities.Models
{
    public class Character : Entity
    {
        public string Name;
        public Abilities Abilities = new Abilities();
        public int XP = 0;
        public string Background;
        public int InventoryWeight;
        public List<Guid> AdventuresPlayed = new List<Guid>();    
        public CharacterClass Class;
        public string CauseOfDeath;
        public string DiedInAdventure;
    }

    public class Abilities
    {
        public int Strength;
        public int Dexterity;
        public int Intelligence;
        public int Wisdom;    
    }

    public enum CharacterClass
    {
        Fighter,
        Thief,
        MagicUser,
        Healer
    }
}
