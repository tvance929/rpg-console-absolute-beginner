using System;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Entities;
using RPGConsoleTutorialSeries.Game;

namespace RPGConsoleTutorialSeries
{
    class Program
    {
        private static AdventureService adventureService = new AdventureService();
        private static CharacterService characterService = new CharacterService();
        private static GameService gameService = new GameService(adventureService, characterService);
        static void Main(string[] args)
        {
            MakeTitle();
            MakeMainMenu();
        }

        private static void MakeTitle()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("***************************************************");
            Console.WriteLine("*                                                 *");
            Console.WriteLine("*      ┌─┐┌─┐┌┐┌┌─┐┌─┐┬  ┌─┐  ┌─┐┬─┐┌─┐┬ ┬┬       *");
            Console.WriteLine("*      │  │ ││││└─┐│ ││  ├┤   │  ├┬┘├─┤││││       *");
            Console.WriteLine("*      └─┘└─┘┘└┘└─┘└─┘┴─┘└─┘  └─┘┴└─┴ ┴└┴┘┴─┘     *");
            Console.WriteLine("*                                                 *");
            Console.WriteLine("***************************************************\n\n");
        }

        private static void MakeMainMenu()
        {
            MakeMenuOptions();           
            var inputValid = false;

            while (!inputValid)
            {
                switch (Console.ReadLine().ToUpper())
                {
                    case "S":
                        gameService.StartGame();
                        inputValid = true;
                        break;
                    case "C":
                        CreateCharacter();
                        inputValid = true;
                        break;
                    case "L":
                        LoadGame();
                        inputValid = true;
                        break;
                    default:
                        Console.WriteLine("\nUm.... you didnt pick the right letter!!??!?");
                        MakeMenuOptions();
                        inputValid = false;
                        break;
                }               
            }
        }

        private static void MakeMenuOptions()
        {
            Console.WriteLine("(S)tart a new game");
            Console.WriteLine("(L)oad a game");
            Console.WriteLine("(C)reate new character");
        }

        private static void LoadGame()
        {
            Console.WriteLine("Load a game, great job!");
        }       

        private static void CreateCharacter()
        {
            Console.WriteLine("You are creating a character, good job!");
        }
    }
}
