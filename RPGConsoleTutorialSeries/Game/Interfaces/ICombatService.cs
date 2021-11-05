using System.Collections.Generic;
using RPGConsoleTutorialSeries.Entities.Models;

namespace RPGConsoleTutorialSeries.Game.Interfaces
{
    public interface ICombatService
    {
        void RunCombat(ref Character character, List<Monster> monsters);
    }
}
