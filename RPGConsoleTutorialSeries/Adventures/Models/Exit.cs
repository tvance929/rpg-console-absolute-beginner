namespace RPGConsoleTutorialSeries.Adventures.Models
{
    public class Exit
    {
        public Lock Lock;
        public CompassDirection WallLocation;
        public Riddle Riddle;
        public int LeadsToRoomNumber;
    }

    public enum CompassDirection
    {
        North,
        East,
        South,
        West
    }
}