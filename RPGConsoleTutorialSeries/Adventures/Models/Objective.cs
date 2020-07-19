namespace RPGConsoleTutorialSeries.Adventures.Models
{
    public class Objective
    {
        public ObjectType ObjectType;
    }

    public enum ObjectType
    {
        MonstersInRoom,
        AllMonsters,
        ItemObtained
    }
}
