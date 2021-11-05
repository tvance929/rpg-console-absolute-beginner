using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Utilities;
using RPGConsoleTutorialSeries.Utilities.Interfaces;
using RPGConsoleTutorialSeries.Items.Models;

namespace RPGConsoleTutorialSeries.Entities
{
    public class CharacterService : ICharacterService
    {
        private readonly IMessageHandler messageHandler;

        public CharacterService(IMessageHandler MessageHandler)
        {
            messageHandler = MessageHandler;
        }

        public Character LoadCharacter(string name)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var character = new Character();

            if (File.Exists($"{basePath}\\{name}.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var characterJsonFile = directory.GetFiles($"{name}.json");

                using (StreamReader fi = File.OpenText(characterJsonFile[0].FullName))
                {
                    character = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                }
            }
            else
            {
                throw new Exception("Character not found.");
            }

            return character;
        }

        public List<Character> GetCharactersInRange(Guid adventureGUID, int minLevel = 0, int maxLevel = 20)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var charactersInRange = new List<Character>();

            try
            {
                var directory = new DirectoryInfo(basePath);
                foreach (var file in directory.GetFiles($"*.json"))
                {
                    using (StreamReader fi = File.OpenText(file.FullName))
                    {
                        var potentialCharacterInRange = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                        if (potentialCharacterInRange.IsAlive &&
                            (potentialCharacterInRange.Level >= minLevel && potentialCharacterInRange.Level <= maxLevel) &&
                            !potentialCharacterInRange.AdventuresPlayed.Contains(adventureGUID))
                        {
                            charactersInRange.Add(potentialCharacterInRange);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"OH NOZE! Goblins!!! {ex.Message}");
            }

            return charactersInRange;
        }

        public bool SaveCharacter(Character character)
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";

            File.WriteAllText($"{basePath}\\{character.Name}.json", JsonConvert.SerializeObject(character));
            return true;
        }

        public void CreateCharacter()
        {
            messageHandler.Clear();
            messageHandler.Write("***********************************");
            messageHandler.Write("*     CHARACTER CREATION          *");
            messageHandler.Write("***********************************\n\n");

            var newCharacter = new Character
            {
                Inventory = new List<Item>()
            };

            messageHandler.Write("Adventurers Name Please : ", true);
            newCharacter.Name = messageHandler.Read();

            messageHandler.Write("\nClass?  F)ighter, T)hief, M)agicUser, H)ealer", true);
            switch (messageHandler.Read().ToLower())
            {
                case "f":
                    newCharacter.Class = CharacterClass.Fighter;
                    newCharacter.Hitpoints = 8;
                    newCharacter.ArmorClass = 12;
                    newCharacter.Abilities.Strength = 1;
                    newCharacter.Attack = new Attack { BaseDie = 8, BonusDamage = 0 };
                    break;
                case "t":
                    newCharacter.Class = CharacterClass.Thief;
                    newCharacter.Hitpoints = 4;
                    newCharacter.ArmorClass = 8;
                    newCharacter.Abilities.Dexterity = 1;
                    newCharacter.Attack = new Attack { BaseDie = 4, BonusDamage = 0 };
                    break;
                case "m":
                    newCharacter.Class = CharacterClass.MagicUser;
                    newCharacter.Hitpoints = 4;
                    newCharacter.ArmorClass = 8;
                    newCharacter.Abilities.Intelligence = 1;
                    newCharacter.Attack = new Attack { BaseDie = 4, BonusDamage = 0 };
                    break;
                case "h":
                    newCharacter.Class = CharacterClass.Healer;
                    newCharacter.Hitpoints = 6;
                    newCharacter.ArmorClass = 10;
                    newCharacter.Abilities.Wisdom = 1;
                    newCharacter.Attack = new Attack { BaseDie = 6, BonusDamage = 0 };
                    break;
            }

            messageHandler.Write("\nBackground : ", true);
            newCharacter.Background = messageHandler.Read();

            newCharacter.Abilities = SetAbilities(newCharacter);

            messageHandler.Clear();
            messageHandler.Write("\n====================================\n\n");
            messageHandler.Write("HERE IS THINE CHARACTER", true); ;
            DisplayCharacter(newCharacter);
            messageHandler.Write("\nWHAT BE YOUR CHOICE?  S)ave or R)edo");
            var playerChoice = messageHandler.Read().ToLower();

            if (playerChoice == "r")
            {
                CreateCharacter();
            }
            else if (playerChoice == "s")
            {
                if (SaveCharacter(newCharacter))
                {
                    messageHandler.Clear();
                    Program.MakeMainMenu();
                }
            }
        }

        private Abilities SetAbilities(Character character)
        {
            var abilityPoints = 3;

            WriteAbilities(character.Abilities);

            while (abilityPoints != 0)
            {
                messageHandler.Write($"You have {abilityPoints} ability points to add to your Character ");
                messageHandler.Write("Add to S)tr, D)ex, I)nt, W)is ?");

                switch (messageHandler.Read().ToLower())
                {
                    case "s":
                        character.Abilities.Strength += 1;
                        abilityPoints -= 1;
                        WriteAbilities(character.Abilities);
                        break;
                    case "d":
                        character.Abilities.Dexterity += 1;
                        abilityPoints -= 1;
                        WriteAbilities(character.Abilities);
                        break;
                    case "i":
                        character.Abilities.Intelligence += 1;
                        abilityPoints -= 1;
                        WriteAbilities(character.Abilities);
                        break;
                    case "w":
                        character.Abilities.Wisdom += 1;
                        abilityPoints -= 1;
                        WriteAbilities(character.Abilities);
                        break;
                }
            }
            return character.Abilities;
        }

        private void WriteAbilities(Abilities abilities)
        {
            messageHandler.Write("\nABILITIES ");
            messageHandler.Write("###################");
            messageHandler.Write($"Strength        {abilities.Strength}");
            messageHandler.Write($"Dexterity       {abilities.Dexterity}");
            messageHandler.Write($"Intelligence    {abilities.Intelligence}");
            messageHandler.Write($"Wisdom          {abilities.Wisdom}");
            messageHandler.Write("###################", true);
        }


        private void DisplayCharacter(Character character)
        {
            messageHandler.Write($"\n\n*****************************");
            messageHandler.Write($"NAME: {character.Name.ToUpper()}");
            messageHandler.Write($"BACKGROUND:\n {character.Background}");
            messageHandler.Write($"LEVEL: {character.Level}");
            messageHandler.Write($"CLASS: {character.Class}");
            WriteAbilities(character.Abilities);
            messageHandler.Write("############################", true);
        }
    }
}
