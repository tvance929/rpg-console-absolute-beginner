namespace RPGConsoleTutorialSeries.Utilities.Interfaces
{
    public interface IMessageHandler
    {
        public void Write(string message = "", bool withLine = true);

        public string Read();

        /// <summary>
        /// Used to clear the screen
        /// </summary>
        public void Clear();
    }
}
