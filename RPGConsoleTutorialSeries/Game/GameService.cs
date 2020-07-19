using System;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game.Interfaces;

namespace RPGConsoleTutorialSeries.Game
{
    public class GameService : IGameService
    {
        private IAdventureService adventureService;
        private ICharacterService characterService;
        private Character character;

        public GameService(IAdventureService AdventureService, ICharacterService CharacterService)
        {
            adventureService = AdventureService;
            characterService = CharacterService;
        }

        public bool StartGame(Adventure adventure = null)
        {
            try
            {
                if (adventure == null)
                {
                    adventure = adventureService.GetInitialAdventure();
                }

                Console.Clear();
                Console.WriteLine();

                //Create Title Banner
                for (int i = 0; i <= adventure.Title.Length + 3; i++)
                {
                    Console.Write("*");
                    if (i == adventure.Title.Length + 3)
                    {
                        Console.Write("\n");
                    }
                }
                Console.WriteLine($"| {adventure.Title} |");
                for (int i = 0; i <= adventure.Title.Length + 3; i++)
                {
                    Console.Write("*");
                    if (i == adventure.Title.Length + 3)
                    {
                        Console.Write("\n");
                    }
                }

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\n{adventure.Description.ToUpper()}");

                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Yellow;

                var charactersInRange = characterService.GetCharactersInRange(adventure.MinimumLevel, adventure.MaxLevel);

                if (charactersInRange.Count == 0)
                {
                    Console.WriteLine("Sorry, you do not have any characters in the range level of the adventure you are trying to play.");
                    return false;
                }
                else
                {
                    Console.WriteLine("WHO DOTH WISH TO CHANCE DEATH!?");
                    var characterCount = 0;
                    foreach (var character in charactersInRange)
                    {
                        Console.WriteLine($"#{characterCount} {character.Name} Level - {character.Level} {character.Class}");
                        characterCount++;
                    }
                }
                character = characterService.LoadCharacter(charactersInRange[Convert.ToInt32(Console.ReadLine())].Name);

                Monster myMonster = new Monster(); //Dont need - kill for next level 

            }
            catch (Exception ex)
            {
                Console.WriteLine($"KaBOOM!  Orcs did it again!  Something went wrong! {ex.Message}");
            }
            return true;
        }
    }
}
