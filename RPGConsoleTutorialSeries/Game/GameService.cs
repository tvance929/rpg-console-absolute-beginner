using System;
using System.Collections.Generic;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
using RPGConsoleTutorialSeries.Adventures.Models;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game.Interfaces;
using RPGConsoleTutorialSeries.Utilities.Interfaces;

namespace RPGConsoleTutorialSeries.Game
{
    public class GameService : IGameService
    {
        private readonly IAdventureService adventureService;
        private readonly ICharacterService characterService;
        private readonly IMessageHandler messageHandler;

        private Character character;

        public GameService(IAdventureService AdventureService, ICharacterService CharacterService, IMessageHandler MessageHandler)
        {
            adventureService = AdventureService;
            characterService = CharacterService;
            messageHandler = MessageHandler;
        }

        public bool StartGame(Adventure adventure = null)
        {
            if (adventure == null)
            {
                adventure = adventureService.GetInitialAdventure();
            }

            CreateTitleBanner(adventure.Title);

            CreateDescriptionBanner(adventure);

            var charactersInRange = characterService.GetCharactersInRange(adventure.MinimumLevel, adventure.MaxLevel);

            if (charactersInRange.Count == 0)
            {
                messageHandler.Write("Sorry, you do not have any characters in the range level of the adventure you are trying to play.");
                return false;
            }
            else
            {
                messageHandler.Write("WHO DOTH WISH TO CHANCE DEATH!?");
                var characterCount = 0;
                foreach (var character in charactersInRange)
                {
                    messageHandler.Write($"#{characterCount} {character.Name} Level - {character.Level} {character.Class}");
                    characterCount++;
                }
            }
            character = characterService.LoadCharacter(charactersInRange[Convert.ToInt32(messageHandler.Read())].Name);

            var rooms = adventure.Rooms;
            RoomProcessor(rooms[0]);

            return true;
        }

        private void CreateDescriptionBanner(Adventure adventure)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            messageHandler.Write($"\n{adventure.Description.ToUpper()}");

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;

            messageHandler.Write($"\nLevels : {adventure.MinimumLevel} - {adventure.MaxLevel}");
            messageHandler.Write($"\nCompletion Rewards = {adventure.CompletionGoldReward} gold & {adventure.CompletionXPReward} xp");
            messageHandler.Write();
        }

        private void CreateTitleBanner(string title)
        {
            messageHandler.Clear();
            messageHandler.Write();

            //Create Title Banner
            for (int i = 0; i <= title.Length + 3; i++)
            {
                messageHandler.Write("*", false);
                if (i == title.Length + 3)
                {
                    messageHandler.Write("\n");
                }
            }
            messageHandler.Write($"| {title} |");
            for (int i = 0; i <= title.Length + 3; i++)
            {
                messageHandler.Write("*", false);
                if (i == title.Length + 3)
                {
                    messageHandler.Write("\n");
                }
            }
        }

        private void RoomProcessor(Room room)
        {
            RoomDescription(room);
            RoomOptions(room);

        }

        private void RoomDescription(Room room)
        {
            messageHandler.Clear();
            messageHandler.Write("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            messageHandler.Write($"{room.RoomNumber} {room.Description}");

            if (room.Exits.Count == 1)
            {
                messageHandler.Write($"There is an exit in this room on the {room.Exits[0].WallLocation} wall.");
            }
            else
            {
                var exitDescription = "";
                foreach (var exit in room.Exits)
                {
                    exitDescription += $"{exit.WallLocation},";
                }

                messageHandler.Write($"This room has exits on the {exitDescription.Remove(exitDescription.Length - 1)} walls.");
            }

            if (room.Chest != null)
            {
                messageHandler.Write($"There is a chest in the room!");
            }
        }

        private void RoomOptions(Room room)
        {
            messageHandler.Write("WHAT WOULD YOU LIKE TO DO!?");
            messageHandler.Write("----------------------------");
            messageHandler.Write("L)ook for traps");
            messageHandler.Write("Use an Exit:");
            foreach (var exit in room.Exits)
            {
                messageHandler.Write($"({exit.WallLocation.ToString().Substring(0, 1)}){exit.WallLocation.ToString().Substring(1)}");
            }
            if (room.Chest != null)
            {
                messageHandler.Write("O)pen the chest");
                messageHandler.Write("C)heck chest for traps");
            }

            var playerDecision = messageHandler.Read().ToLower();
            var exitRoom = false;

            while(exitRoom == false)
            {
                switch (playerDecision)
                {
                    case "l":
                    case "c":
                        CheckForTraps(room);
                        break;
                    case "o" : 
                        if (room.Chest != null) {
                            OpenChest(room.Chest);
                        }
                        else
                        {
                            messageHandler.Write("Alas, there is NO CHEST in this room!");
                        }
                        break;
                    case "n":
                    case "s":
                    case "e":
                    case "w":
                        ExitRoom(room);
                        break;
                }
            }
        }

        private void CheckForTraps(Room room)
        {
            if (room.Trap != null)
            {
                if (room.Trap.TrippedOrDisarmed)
                {
                    messageHandler.Write("You've already found and disarmed this trap... or tripped it (ouch)");
                    return;
                }

                if (room.Trap.SearchedFor)
                {
                    messageHandler.Write("You've already search for a trap, friend!");
                    return;
                }

                var trapBonus = 0 + character.Abilities.Intelligence;
                if (character.Class == CharacterClass.Thief)
                {
                    trapBonus += 2;
                }

                var dice = new Dice();
                var findTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) + trapBonus;

                if (findTrapRoll < 12)
                {
                    messageHandler.Write("You find NO traps.");
                    room.Trap.SearchedFor = true;
                    return;
                }

                messageHandler.Write("You've found a trap! And are forced to try and disarm...");
                var disarmTrapRoll = dice.RollDice(new List<Die>{ Die.D20 }) + trapBonus;
                
                if (disarmTrapRoll < 11)
                {
                    messageHandler.Write("KABOOM!  You did not disarm the trap you take 4 damage!");                    
                }
                else
                {
                    messageHandler.Write("SHEW!!!  You disarmed the trap!");
                }
                room.Trap.TrippedOrDisarmed = true;
                return;
            }
            messageHandler.Write("You find no traps");
            return;
        }

        private void OpenChest(Chest chest)
        {
            throw new NotImplementedException();
        }

        private void ExitRoom(Room room)
        {
            throw new NotImplementedException();
        }
    }
}
