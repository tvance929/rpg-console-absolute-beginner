using RPGConsoleTutorialSeries.Items.Interfaces;

namespace RPGConsoleTutorialSeries.Items.Models
{
    public class Item : IItem
    {
        public ItemType Name;
        public string Description;
        public int ObjectiveNumber;
        public int Weight;
        public int GoldValue;
    }

    public enum ItemType
    {
        Rope,
        Torch,
        HolySymbol,
        Water,
        Food,
        Tinderbox,
        Key,
        Lockpicks
    }
}