using System.Collections.Generic;
using RPGConsoleTutorialSeries.Entities.Models;

namespace RPGConsoleTutorialSeries.Entities.Interfaces
{
    public interface ICharacterService
    {
        public Character LoadCharacter(string name);

        public List<Character> GetCharactersInRange(int minLevel = 0, int maxLevel = 20);
    }
}
