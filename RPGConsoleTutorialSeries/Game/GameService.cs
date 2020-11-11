using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
using RPGConsoleTutorialSeries.Adventures.Models;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game.Interfaces;
using RPGConsoleTutorialSeries.Items.Models;
using RPGConsoleTutorialSeries.Utilities.Interfaces;

namespace RPGConsoleTutorialSeries.Game
{
    public class GameService : IGameService
    {
        private readonly IAdventureService adventureService;
        private readonly ICharacterService characterService;
        private readonly IMessageHandler messageHandler;
        private readonly ITrapService trapService;

        private Character character;
        private Adventure gameAdventure;

        public GameService(IAdventureService AdventureService, 
            ICharacterService CharacterService, 
            IMessageHandler MessageHandler,
            ITrapService TrapService)
        {
            adventureService = AdventureService;
            characterService = CharacterService;
            messageHandler = MessageHandler;
            trapService = TrapService;
        }

        public bool StartGame(Adventure adventure = null)
        {
            gameAdventure = adventure;
            if (gameAdventure == null)
            {
                gameAdventure = adventureService.GetInitialAdventure();
            }

            CreateTitleBanner(gameAdventure.Title);

            CreateDescriptionBanner(gameAdventure);

            var charactersInRange = characterService.GetCharactersInRange(gameAdventure.MinimumLevel, gameAdventure.MaxLevel);

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

            var rooms = gameAdventure.Rooms;
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
            WriteRoomOptions(room);

            var playerDecision = messageHandler.Read().ToLower();
            var exitRoom = false;

            while (exitRoom == false)
            {
                switch (playerDecision)
                {
                    case "l":
                    case "c":
                        var foundTrap = trapService.CheckForTraps(room.Trap, character);
                        room.Trap.SearchedFor = true;
                        if (foundTrap)
                        {
                            messageHandler.Write("You've found a trap! And are forced to try and disarm...");
                            //Make Disarm Option
                            var disarmedTrap = 
                            messageHandler.Write("SHEW!!!  You disarmed the trap!");
                        }

                        CheckForTraps(room);
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "o":
                        if (room.Chest != null)
                        {
                            OpenChest(room.Chest);
                            WriteRoomOptions(room);
                            playerDecision = messageHandler.Read().ToLower();
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
                        var wallLocation = CompassDirection.North;
                        if (playerDecision == "s") wallLocation = CompassDirection.South;
                        else if (playerDecision == "w") wallLocation = CompassDirection.West;
                        else if (playerDecision == "e") wallLocation = CompassDirection.East;

                        if (room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation) != null)
                        {
                            ExitRoom(room, wallLocation);
                        }
                        else
                        {
                            Console.WriteLine("\n Um... that's a wall friend....\n");
                        }

                        break;
                }
            }
        }

        private void WriteRoomOptions(Room room)
        {
            messageHandler.Write("WHAT WOULD YOU LIKE TO DO!?");
            messageHandler.Write("----------------------------");
            messageHandler.Write("L)ook for traps");
            if (room.Chest != null)
            {
                messageHandler.Write("O)pen the chest");
                messageHandler.Write("C)heck chest for traps");
            }
            messageHandler.Write("Use an Exit:");
            foreach (var exit in room.Exits)
            {
                messageHandler.Write($"({exit.WallLocation.ToString().Substring(0, 1)}){exit.WallLocation.ToString().Substring(1)}");
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
                var disarmTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) + trapBonus;

                if (disarmTrapRoll < 11)
                {
                    ProcessTrapMessagesAndDamage(room.Trap);
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

        private void ProcessTrapMessagesAndDamage(Trap trap)
        {
            var dice = new Dice();

            messageHandler.Write($"CLANK!  A sound of metal falls into place... you TRIPPED a {trap.TrapType} trap!");
            var trapDamage = dice.RollDice(new List<Die>() { trap.DamageDie });
            var hitPoints = character.Hitpoints - trapDamage;
            messageHandler.Write($"YOU WERE DAMAGED FOR {trapDamage} HIT POINTS!  You now have {hitPoints} hit pointss!");
            if (hitPoints < 1)
            {
                messageHandler.Write("AND......you're dead.");
                //IMPLEMENT END OF GAME DEATH HERE
            }
            messageHandler.Read();
        }

        private void OpenChest(Chest chest)
        {
            if (!chest.Locked)
            {
                if (chest.Trap != null)
                {
                    if (!chest.Trap.TrippedOrDisarmed)
                    {
                        ProcessTrapMessagesAndDamage(chest.Trap);
                    }
                }
                else
                {
                    if (chest.Gold > 0)
                    {
                        character.Gold += chest.Gold;
                        chest.Gold = 0;
                        messageHandler.Write($"Woot! You find {chest.Gold} gold! Your total gold is now {character.Gold}");
                    }

                    if (chest.Treasure != null)
                    {
                        messageHandler.Write($"You find {chest.Treasure.Count} items in this chest!  And they are:");

                        if (character.Inventory == null)
                        {
                            character.Inventory = new List<Items.Interfaces.IItem>();
                        }

                        foreach (var item in chest.Treasure)
                        {
                            messageHandler.Write(item.Name.ToString());
                        }
                        character.Inventory.AddRange(chest.Treasure);
                        chest.Treasure = new List<Item>();
                    }

                    if (chest.Gold > 0 && chest.Treasure != null)
                    {
                        messageHandler.Write("you find NOTHING in this stinking dirty chest!!!");
                    }
                }
            }
            else
            {
                messageHandler.Write("The chest is locked!  Would you like to attempt to unlock it? Y or N");
                var playerDecision = messageHandler.Read().ToLower();
                switch (playerDecision)
                {
                    case "y":
                        throw new NotImplementedException("We havent implemented unlocking Chests");
                    default:
                        return;
                }
            }
        }

        private void ExitRoom(Room room, CompassDirection wallLocation)
        {
            if (room.Trap != null && room.Trap.TrippedOrDisarmed == false)
            {
                ProcessTrapMessagesAndDamage(room.Trap);
                //IF NOT DEAD - keep going.
            }

            var exit = room.Exits.FirstOrDefault(x => x.WallLocation == wallLocation);

            if (exit == null)
            {
                throw new Exception("this room doesnt have that exception");
            }

            var newRoom = gameAdventure.Rooms.FirstOrDefault(x => x.RoomNumber == exit.LeadsToRoomNumber);

            if (newRoom == null)
            {
                throw new Exception("The room that this previous room was supposed to lead too does not exist!?  Dragons?  Or maybe a bad author!!!");
            }

            RoomProcessor(newRoom);
        }
    }
}
