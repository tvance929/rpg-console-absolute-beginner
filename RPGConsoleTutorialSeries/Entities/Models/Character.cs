using System;
using System.Collections.Generic;
using RPGConsoleTutorialSeries.Items.Interfaces;

namespace RPGConsoleTutorialSeries.Entities.Models
{
    public class Character : Entity
    {
        public string Name;
        public Abilities Abilities;
        public int XP;
        public string Background;
        public int InventoryWeight;
        public List<Guid> AdventuresPlayed;    
        public CharacterClass Class;
        public string CauseOfDeath;
        public string DiedInAdventure;
    }

    public class Abilities
    {
        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Wisdom;
        public int Charisma;        
    }

    public enum CharacterClass
    {
        Fighter,
        Thief,
        MagicUser,
        Healer
    }
}
