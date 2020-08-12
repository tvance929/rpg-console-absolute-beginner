using System;
using RPGConsoleTutorialSeries.Utilities.Interfaces;

namespace RPGConsoleTutorialSeries.Utilities
{
    public class ConsoleMessageHandler : IMessageHandler
    {
        public string Read()
        {
            return Console.ReadLine();
        }

        public void Write(string message = "", bool withLine = true)
        {
            if (withLine)
            {
                Console.WriteLine(message);
            }
            else
            {
                Console.Write(message);
            }
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
