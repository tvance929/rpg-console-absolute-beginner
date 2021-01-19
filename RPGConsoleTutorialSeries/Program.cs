using System;
using System.Media;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Entities;
using RPGConsoleTutorialSeries.Game;
using RPGConsoleTutorialSeries.Utilities;

namespace RPGConsoleTutorialSeries
{
    class Program
    {
        private static readonly AdventureService adventureService = new AdventureService();
        private static readonly ConsoleMessageHandler consoleMessageHandler = new ConsoleMessageHandler();
        private static readonly CharacterService characterService = new CharacterService(consoleMessageHandler);
        private static GameService gameService = new GameService(adventureService, characterService, consoleMessageHandler);

        static void Main(string[] args)
        {
            MakeTitle();
            using (SoundPlayer player = new SoundPlayer($"{AppDomain.CurrentDomain.BaseDirectory}/sounds/shortIntro.wav"))
            {
                player.Play();
                MakeMainMenu(player);
            }
                
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

        public static void MakeMainMenu(SoundPlayer player = null)
        {
            MakeMenuOptions();
            var inputValid = false;
            try
            {
                while (!inputValid)
                {
                    switch (Console.ReadLine().ToUpper())
                    {
                        case "S":
                            if (player != null) player.Stop();
                            gameService.StartGame();
                            inputValid = true;
                            break;
                        case "C":
                            characterService.CreateCharacter();
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
            catch (Exception ex)
            {
                Console.WriteLine($"KaBOOM!  Orcs did it again!  Something went wrong! {ex.Message}");
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
    }
}
