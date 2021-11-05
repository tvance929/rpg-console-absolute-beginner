using System.Collections.Generic;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game.Interfaces;
using RPGConsoleTutorialSeries.Items.Models;
using RPGConsoleTutorialSeries.Utilities.Interfaces;

namespace RPGConsoleTutorialSeries.Game
{
    public class CombatService : ICombatService
    {
        private readonly IMessageHandler messageHandler;

        public CombatService(IMessageHandler MessageHandler)
        {
            messageHandler = MessageHandler;
        }

        public void RunCombat(ref Character character, List<Monster> monsters)
        {
            //Monster list
            var monsterDescription = "You face off against : ";
            foreach (var monster in monsters)
            {
                monsterDescription += $" -- {monster.MonsterType}";
            }

            messageHandler.Write(monsterDescription, true);

            //Initiative 
            var dice = new Dice();
            var d20 = new List<Die> { Die.D20 };
            var charDamageDie = new List<Die> { (Die)character.Attack.BaseDie };

            messageHandler.WriteRead("Hit a Key to roll for Initiative!");

            var charInitiative = dice.RollDice(new List<Die> { Die.D20 });
            var monsterInitiative = dice.RollDice(new List<Die> { Die.D20 });

            messageHandler.WriteRead($"You rolled {charInitiative} and the monsters rolled {monsterInitiative}");

            while (charInitiative == monsterInitiative)
            {
                messageHandler.Write($"Thats a tie, lets roll again!", true);

                charInitiative = dice.RollDice(d20);
                monsterInitiative = dice.RollDice(d20);

                messageHandler.Write($"You rolled {charInitiative} and the monsters rolled {monsterInitiative}");
            }

            var monstersAreAlive = true;
            //var monsterGold = 0;
            //var monsterInventory = new List<Item>();

            var charactersTurn = (charInitiative > monsterInitiative);

            while (character.IsAlive && monstersAreAlive)
            {
                if (charactersTurn)
                {
                    //Character Attack
                    messageHandler.WriteRead($"Hit a Key to Attack the {monsters[0].MonsterType}!\n================");

                    var attackToHitMonster = dice.RollDice(d20);
                    messageHandler.Write($"You rolled a {attackToHitMonster} against the monster's armor class of {monsters[0].ArmorClass}", true);
                    
                    if (attackToHitMonster >= monsters[0].ArmorClass)
                    {
                        var damage = dice.RollDice(charDamageDie);
                        messageHandler.Write($"You swung and hit the {monsters[0].MonsterType} for {damage} damage!", true);

                        monsters[0].Hitpoints -= damage;
                        if (monsters[0].Hitpoints < 1)
                        {
                            messageHandler.Write($"You have killed the {monsters[0].MonsterType}");

                            if (monsters[0].Gold > 0)
                            {
                                messageHandler.Write($"It had {monsters[0].Gold} GOLD!");
                                character.Gold += monsters[0].Gold;
                            }

                            if (monsters[0].Inventory.Count > 0)
                            {
                                messageHandler.Write($"It also has some inventory!  You get:");

                                foreach(Item item in monsters[0].Inventory)
                                {
                                    messageHandler.Write(item.Description);
                                }

                                character.Inventory.AddRange(monsters[0].Inventory);
                            }                          

                            monsters.RemoveAt(0);
                            if (monsters.Count < 1)
                            {
                                monstersAreAlive = false;
                            }
                        }
                    }
                    else
                    {
                        messageHandler.Write("Swing and a miss!!", true);
                    }

                    charactersTurn = false;
                }
                else //Monsters Turn
                {
                    messageHandler.Write($"================\nThe {monsters[0].MonsterType} attacks!");

                    var attackToHitCharacter = dice.RollDice(d20);
                    messageHandler.Write($"The monster rolls a {attackToHitCharacter} and your AC is {character.ArmorClass}", true);
                    if (attackToHitCharacter >= character.ArmorClass)
                    {
                        messageHandler.Write("The monster hits!", true);
                        var damage = dice.RollDice(new List<Die> { (Die)monsters[0].Attack.BaseDie });

                        messageHandler.Write($"It hits you for {damage}!\n", true);

                        character.Hitpoints -= damage;
                        if (character.Hitpoints < 1)
                        {
                            messageHandler.Write($"You have died.... at the hands of a viscious {monsters[0].MonsterType}");

                            character.CauseOfDeath = $"Combat with {monsters[0].MonsterType}";
                            character.IsAlive = false;
                        }
                    }
                    else
                    {
                        messageHandler.Write("Swing and a miss!!", true);
                    }
                    charactersTurn = true;
                }
            }
        }
    }
}
