using System;
using System.Collections.Generic;
using System.Linq;
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

        private Character character;
        private Adventure gameAdventure;
        private bool gameWon = false;
        private string gameWinningDescription;

        public GameService(IAdventureService AdventureService, ICharacterService CharacterService, IMessageHandler MessageHandler)
        {
            adventureService = AdventureService;
            characterService = CharacterService;
            messageHandler = MessageHandler;
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

            var charactersInRange = characterService.GetCharactersInRange(gameAdventure.GUID, gameAdventure.MinimumLevel, gameAdventure.MaxLevel);

            if (charactersInRange.Count == 0)
            {
                messageHandler.Write("Sorry, you do not have any characters in the range level of the adventure you are trying to play.");
                messageHandler.Write("Would you like to:");
                messageHandler.Write("C)reate a new character");
                messageHandler.Write("R)eturn to the Main Menu?");
                var playerDecision = messageHandler.Read().ToLower();
                if (playerDecision == "r")
                {
                    messageHandler.Clear();
                    Program.MakeMainMenu();
                }
                else if (playerDecision == "c")
                {
                    messageHandler.Clear();
                    characterService.CreateCharacter();
                }
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
                        CheckForTraps(room);
                        WriteRoomOptions(room);
                        playerDecision = messageHandler.Read().ToLower();
                        break;
                    case "o":
                        if (room.Chest != null)
                        {
                            OpenChest(room.Chest);
                            if (gameWon)
                            {
                                GameOver();
                            }
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

            messageHandler.Write($"CLANK! A sound of metal falls into place... you TRIPPED a {trap.TrapType} trap!");
            var trapDamage = dice.RollDice(new List<Die>() { trap.DamageDie });
            character.Hitpoints -= trapDamage;
            var hitPoints = character.Hitpoints;
            messageHandler.Write($"YOU WERE DAMAGED FOR {trapDamage} HIT POINTS!  You now have {hitPoints} hit pointss!");
            if (hitPoints < 1)
            {
                messageHandler.Write("AND......you're dead.");
                character.CauseOfDeath = $"Killed by a {trap.TrapType}... it was ugly.";
                character.DiedInAdventure = gameAdventure.Title;
                character.IsAlive = false;
                GameOver();
            }
            messageHandler.Read();
        }

        private void OpenChest(Chest chest)
        {
            if (chest.Lock == null || !chest.Lock.Locked)
            {
                if (chest.Trap != null && !chest.Trap.TrippedOrDisarmed)
                {
                    ProcessTrapMessagesAndDamage(chest.Trap);
                    chest.Trap.TrippedOrDisarmed = true;
                }
                else
                {
                    messageHandler.Write("You open the chest..");
                    if (chest.Gold > 0)
                    {
                        character.Gold += chest.Gold;
                        messageHandler.Write($"Woot! You find {chest.Gold} gold! Your total gold is now {character.Gold}\n");
                        chest.Gold = 0;
                    }

                    if (chest.Treasure != null && chest.Treasure.Count > 0)
                    {
                        messageHandler.Write($"You find {chest.Treasure.Count} items in this chest!  And they are:");

                        foreach (var item in chest.Treasure)
                        {
                            messageHandler.Write(item.Name.ToString());

                            if (item.ObjectiveNumber == gameAdventure.FinalObjective)
                            {
                                gameWon = true;
                                gameWinningDescription = item.Description;
                                character.Gold += gameAdventure.CompletionGoldReward;
                                character.XP += gameAdventure.CompletionXPReward;
                                character.AdventuresPlayed.Add(gameAdventure.GUID);
                            }
                        }
                        messageHandler.Write("\n");

                        character.Inventory.AddRange(chest.Treasure);
                        chest.Treasure = new List<Item>();

                        if (gameWon)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.ForegroundColor = ConsoleColor.White;
                            messageHandler.Write("***************************************************");
                            messageHandler.Write("*  ~~~YOU FOUND THE FINAL OBJECTIVE!  YOU WIN!~~~ *");
                            messageHandler.Write("***************************************************");

                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            messageHandler.Write("YOU FOUND : " + gameWinningDescription);
                            messageHandler.Write("XP Reward = " + gameAdventure.CompletionXPReward);
                            messageHandler.Write("Gold Reward = " + gameAdventure.CompletionGoldReward);
                            messageHandler.Write(character.Name + " now has " + character.XP + " XP and " + character.Gold + " gold.");
                        }
                        return;
                    }

                    if (chest.Gold == 0 && (chest.Treasure == null || chest.Treasure.Count == 0))
                    {
                        messageHandler.Write("The chest is empty... \n");
                    }
                }
            }
            else
            {
                if (TryUnlock(chest.Lock))
                {
                    OpenChest(chest);
                    if (gameWon)
                    {
                        GameOver();
                    }
                }
            }
        }

        private void ExitRoom(Room room, CompassDirection wallLocation)
        {
            if (room.Trap != null && room.Trap.TrippedOrDisarmed == false)
            {
                ProcessTrapMessagesAndDamage(room.Trap);
                room.Trap.TrippedOrDisarmed = true;
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

            if ((exit.Lock == null || !exit.Lock.Locked) || TryUnlock(exit.Lock))
            {
                RoomProcessor(newRoom);
            }
            else
            {
                RoomProcessor(room);
            }
        }

        private bool TryUnlock(Lock theLock)
        {
            if (!theLock.Locked) return true;

            var hasOptions = true;
            var dice = new Dice();

            while (hasOptions)
            {
                if (!theLock.Attempted)
                {
                    messageHandler.Write("Locked!  Would you like to attempt to unlock it? \n" +
                        "K)ey L)ockpick B)ash or W)alk away");
                    var playerDecision = messageHandler.Read().ToLower();
                    switch (playerDecision)
                    {
                        case "k":
                            if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Key && x.ObjectiveNumber == theLock.KeyNumber) != null)
                            {
                                messageHandler.WriteRead("You have the right key!  It unlocks the lock! \n");
                                theLock.Locked = false;
                                return true;
                            }
                            else
                            {
                                messageHandler.Write("You do not have a key for this chest \n");
                                break;
                            }
                        case "l":
                            if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Lockpicks) == null)
                            {
                                messageHandler.Write("You don't have lockpicks! \n");
                                break;
                            }
                            else
                            {
                                var lockpickBonus = 0 + character.Abilities.Dexterity;
                                if (character.Class == CharacterClass.Thief)
                                {
                                    lockpickBonus += 2;
                                }
                                var pickRoll = (dice.RollDice(new List<Die> { Die.D20 }) + lockpickBonus);
                                if (pickRoll > 12)
                                {
                                    messageHandler.WriteRead($"Youe dextrous hands click that lock open! \n" +
                                    $"Your lockpick roll was {pickRoll} and you needed 12! \n");
                                    theLock.Locked = false;
                                    theLock.Attempted = true;
                                    return true;
                                }
                                messageHandler.WriteRead($"Snap! The lock doesnt budge! \n" +
                                $"Your lockpick roll was {pickRoll} and you needed 12! \n");
                                theLock.Attempted = true;
                                break;
                            }
                        case "b":
                            var bashBonus = 0 + character.Abilities.Strength;
                            if (character.Class == CharacterClass.Fighter)
                            {
                                bashBonus += 2;
                            }
                            var bashRoll = (dice.RollDice(new List<Die> { Die.D20 }) + bashBonus);
                            if (bashRoll > 16)
                            {
                                messageHandler.WriteRead($"You muster your strength and BASH that silly lock into submission! \n" +
                                    $"Your bash roll was {bashRoll} and you needed 16! \n");
                                theLock.Locked = false;
                                theLock.Attempted = true;
                                return true;
                            }
                            messageHandler.WriteRead($"Ouch! The lock doesnt budge! \n" +
                                $"Your bash roll was {bashRoll} and you needed 16! \n");
                            theLock.Attempted = true;
                            break;

                        default:
                            return false;
                    }
                }
                else
                {
                    if (character.Inventory.FirstOrDefault(x => x.Name == ItemType.Key && x.ObjectiveNumber == theLock.KeyNumber) != null)
                    {
                        messageHandler.WriteRead("You've tried bashing or picking to no avail BUT you have the right key!  Unlocked! \n");
                        theLock.Locked = false;
                        return true;
                    }
                    else
                    {
                        messageHandler.WriteRead("You cannot try to bash or pick this lock again and you do not currently have a key! \n");
                        return false;
                    }
                }
            }
            return false;
        }

        private void GameOver()
        {
            characterService.SaveCharacter(character);
            character = new Character();
            messageHandler.WriteRead("THY GAME IS OVER SON/MADAM ... PRESSETH ENTER TO RETURNETH TO THINE MAIN MENU");
            messageHandler.Clear();
            Program.MakeMainMenu();
        }
    }
}
