namespace GZipper
{
    internal class GZipper
    {
        static void Main(string[] args)
        {
            var inputString = Console.ReadLine();
            args = inputString.Split(' ');

            string mode = args[0];
            string inputFilePath = args[1];
            string outputFilePath = args[2];

            try
            {
                if (mode.Equals("compress", StringComparison.OrdinalIgnoreCase))
                    Zipper.Compress(inputFilePath, outputFilePath);
                else if (mode.Equals("decompress", StringComparison.OrdinalIgnoreCase))
                    Zipper.Decompress(inputFilePath, outputFilePath);
                else
                    Logger.Print($"Неизвестный режим. Используйте либо 'compress', либо 'decompress'", LogMessageType.Error);
            }
            catch (Exception ex)
            {
                Logger.Print($"Ошибка: {ex.Message}", LogMessageType.Error);
            }
        }
    }
}
