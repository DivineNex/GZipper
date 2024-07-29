using System.IO.Compression;

namespace GZipper
{
    internal static class Zipper
    {
        internal static void Compress(string inputPath, string outputPath)
        {
            var splittedInput = inputPath.Split('.');
            var splittedOutput = outputPath.Split('.');
            splittedOutput[^1] = $".{splittedInput[^1]}.{splittedOutput[^1]}";

            outputPath = string.Concat(splittedOutput);

            using FileStream inputFileStream = File.Open(inputPath, FileMode.Open);
            using FileStream compressedFileStream = File.Create(outputPath);
            using var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);

            inputFileStream.CopyTo(compressor);
        }

        internal static void CompressAsync(string inputPath, string outputPath)
        {

        }

        internal static void Decompress(string inputPath, string outputPath)
        {
            using FileStream inputFileStream = File.Open(inputPath, FileMode.Open);
            using FileStream decompressedFileStream = File.Create(outputPath);
            using var decompressor = new GZipStream(inputFileStream, CompressionMode.Decompress);

            decompressor.CopyTo(decompressedFileStream);
        }

        internal static void DecompressAsync(string inputPath, string outputPath)
        {

        }
    }
}
