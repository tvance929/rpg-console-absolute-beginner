using System;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
using RPGConsoleTutorialSeries.Entities.Interfaces;

namespace RPGConsoleTutorialSeries.Game
{
    public class GameService
    {
        private IAdventureService adventureService;
        private ICharacterService characterService;

        public GameService(IAdventureService AdventureService, ICharacterService CharacterService)
        {
            adventureService = AdventureService;
            characterService = CharacterService;
        }

        public void StartGame()
        {
            var initialAdventure = adventureService.GetInitialAdventure();
            var initialCharacter = characterService.LoadInitialCharacter();

            Console.WriteLine($"Adventure : {initialAdventure.Title}");
            Console.WriteLine($"Description : {initialAdventure.Description}");
            Console.WriteLine($"Character Name : {initialCharacter.Name}");
            Console.WriteLine($"Level : {initialCharacter.Level}");
        }
    }
}
