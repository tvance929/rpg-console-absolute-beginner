using System;
using System.IO;
using Newtonsoft.Json;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;

namespace RPGConsoleTutorialSeries.Entities
{
    public class CharacterService : ICharacterService
    {
        public Character LoadInitialCharacter()
        {
            var basePath = $"{AppDomain.CurrentDomain.BaseDirectory}characters";
            var initialCharacter = new Character();

            if (File.Exists($"{basePath}\\conan.json"))
            {
                var directory = new DirectoryInfo(basePath);
                var initialJsonFile = directory.GetFiles("conan.json");

                using (StreamReader fi = File.OpenText(initialJsonFile[0].FullName))
                {
                    initialCharacter = JsonConvert.DeserializeObject<Character>(fi.ReadToEnd());
                }
            }

            return initialCharacter;
        }
    }
}
