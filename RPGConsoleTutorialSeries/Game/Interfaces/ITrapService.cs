using RPGConsoleTutorialSeries.Adventures.Models;
using RPGConsoleTutorialSeries.Entities.Models;

namespace RPGConsoleTutorialSeries.Game.Interfaces
{
    public interface ITrapService
    {
        public bool CheckForTraps(Trap trap, Character character);
        public bool DisarmTrap(Trap trap);
        public int ProcessTrapMessagesAndDamage(Trap trap);


    }
}
