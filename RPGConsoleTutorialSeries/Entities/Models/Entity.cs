using System.Collections.Generic;
using RPGConsoleTutorialSeries.Items.Models;

namespace RPGConsoleTutorialSeries.Entities.Models
{
    public abstract class Entity
    {
        public int Hitpoints = 0;
        public Attack Attack;
        public int Gold;
        public int Level;
        public bool IsAlive;
        public int ArmorClass;
        public List<Item> Inventory;
    }

    public class Attack
    {
        public int BaseDie;
        public int BonusDamage;
    }
}
