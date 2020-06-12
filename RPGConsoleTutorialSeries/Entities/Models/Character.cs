using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using RPGConsoleTutorialSeries.Items.Interfaces;

namespace RPGConsoleTutorialSeries.Entities.Models
{
    public class Character
    {
        public string Name;
        public int Level;
        public Abilities Abilities;
        public int Gold;
        public string Background;
        public int InventoryWeight;
        public List<string> AdventuresPlayed;
        public bool IsAlive;
        public int ArmorClass;
        public List<IItem> Inventory;
        public int HitPoints;
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
}
