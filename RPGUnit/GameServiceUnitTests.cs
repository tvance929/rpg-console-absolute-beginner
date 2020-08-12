using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game;
using Shouldly;
using Xunit;

namespace RPGUnit
{
    public class GameServiceUnitTests
    {
        private GameService gameService;

        private Mock<IAdventureService> mockAdventureService = new Mock<IAdventureService>();
        private Mock<ICharacterService> mockCharacterService = new Mock<ICharacterService>();

        public GameServiceUnitTests()
        {
            gameService = new GameService(mockAdventureService.Object, mockCharacterService.Object);
        }

        [Fact]
        public void GetSuccount()
        {
            //Arrange  //Act //Assert
            gameService.StartGame().ShouldBe(true);
        }

        [Fact]
        public void GetsdfdfSuccount()
        {
            //Arrange
            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Throws(new Exception());

            //Act //Assert
            Should.Throw<Exception>(() => gameService.StartGame());
        }

        [Fact]
        public void sdfg()
        {
            //Arrange
            var characterList = new List<Character>
            {
                new Character { Name = "TestCharacter" }
            };
            Console.SetIn(new StringReader("0"));

            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "test", Description = "testDescription" });
            mockCharacterService.Setup(_ => _.GetCharactersInRange(It.IsAny<int>(), It.IsAny<int>())).Returns(characterList);

            //Act //Assert
            Should.Throw<Exception>(() => gameService.StartGame());
        }
    }
}