namespace GZipper
{
    internal enum LogMessageType
    {
        Info,
        Warning,
        Error
    }

    internal static class Logger
    {
        public static void Print(string message, LogMessageType messageType)
        {
            switch (messageType)
            {
                case LogMessageType.Info:
                    Console.WriteLine(message);
                    break;
                case LogMessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogMessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}
