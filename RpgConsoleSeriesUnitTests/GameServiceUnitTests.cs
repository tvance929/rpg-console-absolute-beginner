using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json.Serialization;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
using RPGConsoleTutorialSeries.Adventures.Models;
using RPGConsoleTutorialSeries.Entities.Interfaces;
using RPGConsoleTutorialSeries.Entities.Models;
using RPGConsoleTutorialSeries.Game;
using RPGConsoleTutorialSeries.Game.Interfaces;
using RPGConsoleTutorialSeries.Utilities.Interfaces;
using Shouldly;
using Xunit;

namespace RpgConsoleSeriesUnitTests
{
    public class GameServiceUnitTests
    {
        private GameService gameService;

        private Mock<IAdventureService> mockAdventureService = new Mock<IAdventureService>();
        private Mock<ICharacterService> mockCharacterService = new Mock<ICharacterService>();
        private Mock<IMessageHandler> mockMessageHandler = new Mock<IMessageHandler>();
        private Mock<ITrapService> mockTrapService = new Mock<ITrapService>();

        public GameServiceUnitTests()
        {
            gameService = new GameService(mockAdventureService.Object, 
                mockCharacterService.Object, mockMessageHandler.Object, mockTrapService.Object);
        }

        [Fact]
        public void Method_Should_Return_Exception_When_Service_Throws_Exception()
        {
            //Arrange
            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Throws(new Exception());
            
            //Act //Assert
            Should.Throw<Exception>(() => gameService.StartGame());
        }

        [Fact]
        public void Method_Should_Return_False_When_No_Characters_In_Range()
        {
            //Arrange
            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "testTitle", Description = "test description" });
            mockCharacterService.Setup(_ => _.GetCharactersInRange(It.IsAny<int>(), It.IsAny<int>())).Returns(new List<Character>());

            //Act
            var methodReturn = gameService.StartGame();

            //Assert
            methodReturn.ShouldBeFalse();
        }

        [Fact]
        public void Method_Should_Return_True_When_There_Are_Characters_In_Range()
        {
            //Arrange
            var characterList = new List<Character>
            {
                new Character { Name = "Todd" },
                new Character { Name = "Bart" }
            };

            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "testTitle", Description = "test description", Rooms = new List<Room> { new Room() } });
            mockCharacterService.Setup(_ => _.GetCharactersInRange(It.IsAny<int>(), It.IsAny<int>())).Returns(characterList);
            mockMessageHandler.Setup(_ => _.Read()).Returns("0");

            //Act
            var methodReturn = gameService.StartGame();

            //Assert
            methodReturn.ShouldBeTrue();
        }

        [Fact]
        public void Method_Should_Throw_Exception_When_CharacterInput_Not_A_Number()
        {
            //Arrange
            var characterList = new List<Character>
            {
                new Character { Name = "Todd" },
                new Character { Name = "Bart" }
            };

            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "testTitle", Description = "test description" });
            mockCharacterService.Setup(_ => _.GetCharactersInRange(It.IsAny<int>(), It.IsAny<int>())).Returns(characterList);
            mockMessageHandler.Setup(_ => _.Read()).Returns("z");

            //Act //Assert
            Should.Throw<Exception>(() => gameService.StartGame());
        }
    }
}
