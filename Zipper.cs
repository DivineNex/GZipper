using System.Collections.Concurrent;
using System.IO.Compression;

namespace GZipper
{
    internal static class Zipper
    {
        private const int BLOCK_SIZE = 1024 * 1024;
        private static readonly ConcurrentQueue<(long index, byte[] data)> _compressedBlocksQueue = new ConcurrentQueue<(long, byte[])>();
        private static readonly ManualResetEventSlim _dataAvailable = new ManualResetEventSlim(false);
        private static readonly ManualResetEventSlim _stopRequested = new ManualResetEventSlim(false);
        private static Task _writerTask;

        internal static void Compress(string inputFilePath, string outputFilePath)
        {

            StartWriter(outputFilePath);

            using FileStream inputFileStream = File.OpenRead(inputFilePath);

            Parallel.For(0, (inputFileStream.Length + BLOCK_SIZE - 1) / BLOCK_SIZE, i =>
            {
                byte[] buffer = new byte[BLOCK_SIZE];
                int bytesRead;

                lock (inputFileStream)
                {
                    inputFileStream.Seek(i * BLOCK_SIZE, SeekOrigin.Begin);
                    bytesRead = inputFileStream.Read(buffer, 0, BLOCK_SIZE);
                }

                if (bytesRead > 0)
                {
                    byte[] compressedData = CompressBlock(buffer, bytesRead);
                    _compressedBlocksQueue.Enqueue((i, compressedData));
                    _dataAvailable.Set();
                }
            });

            StopWriter();
        }

        private static byte[] CompressBlock(byte[] data, int count)
        {
            using var inputStream = new MemoryStream(data, 0, count);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);
            inputStream.CopyTo(gzipStream);
            gzipStream.Flush();
            return outputStream.ToArray();
        }

        private static void StartWriter(string outputPath)
        {
            _writerTask = Task.Run(() =>
            {
                using FileStream outputFileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                long nextBlockIndex = 0;

                while (true)
                {
                    _dataAvailable.Wait();
                    _dataAvailable.Reset();

                    while (_compressedBlocksQueue.TryDequeue(out var item))
                    {
                        if (item.index == nextBlockIndex)
                        {
                            // Записываем длину блока (4 байта)
                            byte[] lengthBuffer = BitConverter.GetBytes(item.data.Length);
                            outputFileStream.Write(lengthBuffer, 0, lengthBuffer.Length);

                            outputFileStream.Write(item.data, 0, item.data.Length);
                            nextBlockIndex++;
                        }
                        else
                        {
                            _compressedBlocksQueue.Enqueue(item);
                        }
                    }

                    if (_stopRequested.IsSet)
                    {
                        break;
                    }
                }
            });
        }

        private static void StopWriter()
        {
            _stopRequested.Set();
            _dataAvailable.Set();
            _writerTask.Wait();
        }

        internal static void Decompress(string inputFilePath, string outputFilePath)
        {
            using FileStream inputFileStream = File.OpenRead(inputFilePath);
            using FileStream outputFileStream = File.Create(outputFilePath);

            while (inputFileStream.Position < inputFileStream.Length)
            {
                byte[] lengthBuffer = new byte[4];
                inputFileStream.Read(lengthBuffer, 0, 4);
                int blockSize = BitConverter.ToInt32(lengthBuffer, 0);

                byte[] buffer = new byte[blockSize];
                inputFileStream.Read(buffer, 0, blockSize);

                byte[] decompressedData = DecompressBlock(buffer);
                outputFileStream.Write(decompressedData, 0, decompressedData.Length);
            }
        }

        private static byte[] DecompressBlock(byte[] data)
        {
            using var inputStream = new MemoryStream(data);
            using var outputStream = new MemoryStream();
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            gzipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}
