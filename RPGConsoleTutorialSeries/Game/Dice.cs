using System;
using System.Collections.Generic;

namespace RPGConsoleTutorialSeries.Game
{
    public class Dice
    {
        public int RollDice(List<Die> DiceToRoll)
        {
            var randomRoller = new Random();
            var total = 0;
            foreach(var die in DiceToRoll)
            {
                total += randomRoller.Next(1, (int)die);
            }
            return total;
        }
    }

    public enum Die
    {
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D20 = 20,
        D100 = 100
    }
}
