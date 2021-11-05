using System;
using System.Collections.Generic;
using Moq;
using RPGConsoleTutorialSeries.Adventures;
using RPGConsoleTutorialSeries.Adventures.Interfaces;
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
        private Mock<ICombatService> mockCombatService = new Mock<ICombatService>();

        public GameServiceUnitTests()
        {
            gameService = new GameService(mockAdventureService.Object, mockCharacterService.Object, mockMessageHandler.Object, mockCombatService.Object);
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
        public void Method_Should_Throw_Exception_When_CharacterInput_Not_A_Number()
        {
            //Arrange
            var characterList = new List<Character>
            {
                new Character { Name = "Todd" },
                new Character { Name = "Bart" }
            };

            mockAdventureService.Setup(_ => _.GetInitialAdventure()).Returns(new Adventure { Title = "testTitle", Description = "test description" });
            mockCharacterService.Setup(_ => _.GetCharactersInRange(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>())).Returns(characterList);
            mockMessageHandler.Setup(_ => _.Read()).Returns("z");

            //Act //Assert
            Should.Throw<Exception>(() => gameService.StartGame());
        }
    }
}
