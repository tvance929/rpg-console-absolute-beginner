using System;
using System.Collections.Generic;
using System.Text;
using RPGConsoleTutorialSeries.Adventures.Models;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game.Interfaces;
using RPGConsoleTutorialSeries.Utilities.Interfaces;

namespace RPGConsoleTutorialSeries.Game
{
    public class TrapService : ITrapService
    {
        private readonly IMessageHandler messageHandler;

        public TrapService(IMessageHandler MessageHandler)
        {
            messageHandler = MessageHandler;
        }

        public bool CheckForTraps(Trap trap, Character character)
        {
            if (trap != null)
            {
                if (trap.TrippedOrDisarmed)
                {
                    messageHandler.Write("You've already found and disarmed this trap... or tripped it (ouch)");
                    return false;
                }

                if (trap.SearchedFor)
                {
                    messageHandler.Write("You've already search for a trap, friend!");
                    return false;
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
                    return false;
                }
            }
            messageHandler.Write("You find no traps");
            return false;
        }

        public bool DisarmTrap(Trap trap, Character character)
        {
            var trapBonus = 0 + character.Abilities.Intelligence;
            if (character.Class == CharacterClass.Thief)
            {
                trapBonus += 2;
            }

            var dice = new Dice();

            var disarmTrapRoll = dice.RollDice(new List<Die> { Die.D20 }) + trapBonus;

            if (disarmTrapRoll < 11)
            {
                return false;
                //   ProcessTrapMessagesAndDamage(room.Trap);
            }
            else
            {

                return true;
            }
        }

        public int ProcessTrapMessagesAndDamage(Trap trap)
        {
            throw new NotImplementedException();
        }
    }
}
