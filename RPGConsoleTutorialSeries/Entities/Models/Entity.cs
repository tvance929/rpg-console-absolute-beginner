namespace RPGConsoleTutorialSeries.Entities.Models
{
    public abstract class Entity
    {
        public int Hitpoints = 0;
        public Attack Attack;
    }

    public class Attack
    {
        public int BaseDie;
        public int BonusDamage;
    }
}
