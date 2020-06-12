using System;
using System.IO;
using Newtonsoft.Json;
using RPGConsoleTutorialSeries.Adventures.Interfaces;

namespace RPGConsoleTutorialSeries.Adventures
{
    public class AdventureService : IAdventureService
    {
        public Adventure GetInitialAdventure()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}adventures";
            var initialAdventure = new Adventure();

            if (File.Exists($"{basePath}\\initial.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var initialJsonFile = directory.GetFiles("initial.json");

                using (StreamReader fi = File.OpenText(initialJsonFile[0].FullName))
                {
                    initialAdventure = JsonConvert.DeserializeObject<Adventure>(fi.ReadToEnd());
                }
            }

            return initialAdventure;
        }
    }
}
