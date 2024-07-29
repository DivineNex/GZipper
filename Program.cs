namespace GZipper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputString = Console.ReadLine();

            ///////////TEST
            //var dateTimeTicks = DateTime.Now.Ticks;

            //Zipper.Compress("C:\\Users\\nexte\\Desktop\\28.mp3", $"C:\\Users\\nexte\\Desktop\\COMPRESSED{dateTimeTicks}.gz");
            //Zipper.Decompress($"C:\\Users\\nexte\\Desktop\\COMPRESSED{dateTimeTicks}.mp3.gz", "C:\\Users\\nexte\\Desktop\\DECOMPESSED.mp3");
            ///////////TEST

            if (!string.IsNullOrEmpty(inputString))
            {
                var splittedInput = inputString.Split(' ').ToList();

                if (splittedInput.Count > 0)
                {
                    if (splittedInput[0].ToLower() == "compress")
                        Zipper.Compress(splittedInput[1], splittedInput[2]);
                    else if (splittedInput[0].ToLower() == "decompress")
                        Zipper.Decompress(splittedInput[1], splittedInput[2]);
                }
            }
        }
    }
}
