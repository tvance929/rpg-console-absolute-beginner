namespace RPGConsoleTutorialSeries.Utilities.Interfaces
{
    public interface IMessageHandler
    {
        public void Write(string message = "", bool withLine = true);

        public string Read();

        /// <summary>
        /// Write a message on a line and then read the line
        /// </summary>
        /// <param name="message"></param>
        public void WriteRead(string message);

        /// <summary>
        /// Used to clear the screen
        /// </summary>
        public void Clear();
    }
}