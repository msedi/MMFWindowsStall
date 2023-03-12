using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace MemoryMappedFileTest
{
    class Program
    {
        // Size of a single block in bytes
        const int SizeOfBlock = 30000000;

        // Number of blocks.
        const int NoOfBlocks = 5000;

        // Name of the test file.
        const string FileName = "C:\\MMF\\mm.dat";

        static unsafe void Main(string[] args)
        {
            // Create the temporary which is deleted on close.
            using var stream = new FileStream(FileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose);

            // Create the file mapping and the view
            using var mm = MemoryMappedFile.CreateFromFile(stream, null, NoOfBlocks * (long)SizeOfBlock, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, false);
            using var va = mm.CreateViewAccessor();

            // Get the absolute start address
            var StartAddress = va.SafeMemoryMappedViewHandle.DangerousGetHandle();

            // Create some empty block data we can write.
            var BlockData = new byte[SizeOfBlock];

            // Run through the blocks and write 0s in it.
            for (int blockNo = 0; blockNo < NoOfBlocks; blockNo++)
            {
                // Get the address of the block
                byte* blockAddress = (byte*)StartAddress + checked(blockNo * (long)SizeOfBlock);

                // Work with spans
                Span<byte> blockSpan = new(blockAddress, SizeOfBlock);
                BlockData.CopyTo(blockSpan);

                // Print the blockNo.
                Console.WriteLine(blockNo);
            };

            Console.WriteLine("Done");
        }
    }
}
