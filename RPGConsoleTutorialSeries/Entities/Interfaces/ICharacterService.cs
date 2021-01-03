using System;
using System.Collections.Generic;
using RPGConsoleTutorialSeries.Entities.Models;

namespace RPGConsoleTutorialSeries.Entities.Interfaces
{
    public interface ICharacterService
    {
        public Character LoadCharacter(string name);

        public bool SaveCharacter(Character character);

        public List<Character> GetCharactersInRange(Guid adventureGUID, int minLevel = 0, int maxLevel = 20);
    }
}
