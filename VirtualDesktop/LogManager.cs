namespace VirtualDesktop
{
    public class LogManager
    {
        private static string LOG_FILE_NAME = "TestLog.txt";

        public static void WriteLogMessageToFile(string message)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(LOG_FILE_NAME , true))
            {
                file.WriteLine(message);
            }
        }
    }
}
