using RPGConsoleTutorialSeries.Game;

namespace RPGConsoleTutorialSeries.Adventures.Models
{
    public class Trap
    {
        public TrapType TrapType;
        public Die DamageDie = Die.D4;
    }

    public enum TrapType
    {
        Pit,
        Poison,
        Spike
    }
}
