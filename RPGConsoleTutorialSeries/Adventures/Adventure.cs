using System;
using System.Collections.Generic;
using RPGConsoleTutorialSeries.Adventures.Models;

namespace RPGConsoleTutorialSeries.Adventures
{
    public class Adventure
    {
        public Guid GUID;
        public string Title;
        public string Description;
        public int CompletionXPReward;
        public int CompletionGoldReward;
        public int FinalObjective;
        public int MaxLevel;
        public int MinimumLevel;
        public List<Room> Rooms;

        public Adventure()
        {

        }
    }
}
