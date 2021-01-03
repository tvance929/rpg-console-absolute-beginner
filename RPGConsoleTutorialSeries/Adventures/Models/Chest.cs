using System.Collections.Generic;
using RPGConsoleTutorialSeries.Items.Models;

namespace RPGConsoleTutorialSeries.Adventures.Models
{
    public class Chest
    {
        public Lock Lock;
        public Trap Trap;
        public List<Item> Treasure;
        public int Gold;
    }
}