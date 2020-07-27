using RPGConsoleTutorialSeries.Adventures;

namespace RPGConsoleTutorialSeries.Game.Interfaces
{
    public interface IGameService
    {
        bool StartGame(Adventure adventure = null);
    }
}