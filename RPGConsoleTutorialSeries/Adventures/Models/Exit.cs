namespace RPGConsoleTutorialSeries.Adventures.Models
{
    public class Exit
    {
        public bool Locked = false;
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
