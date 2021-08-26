using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    public class BinaryFileProcessor
    {
        public BinaryFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public void Process()
        {
            //byte[] data = File.ReadAllBytes(InputFilePath);

            //byte largest = data.Max();

            //byte[] newData = new byte[data.Length + 1];
            //Array.Copy(data, newData, data.Length);
            ////newData[newData.Length - 1] = largest;

            //// Because we're working with C# 9 in this demo, we can use the array indexing features that were first introduced in C# 8 and 
            //// simplify this indexer here, and this will give us the last element in the array. 
            //newData[^1] = largest;

            //File.WriteAllBytes(OutputFilePath, newData);  

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //using FileStream inputFileStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read);
            //using FileStream outputFileStream = File.OpenWrite(OutputFilePath);

            //// We're using a cnstant here just to improve the readability of the code.
            //const int endOfStream = -1;

            //// Recal that what we want to do in this method is find the largest byte and add that to the end of the file.

            //int largestByte = 0;

            //// Returns the byte, cast to an int, or -1 if the end of the stream has been reached.
            //int currentByte = inputFileStream.ReadByte();
            //while(currentByte != endOfStream)
            //{
            //    outputFileStream.WriteByte((byte)currentByte);

            //    if(currentByte > largestByte)
            //    {
            //        largestByte = currentByte;
            //    }

            //    currentByte = inputFileStream.ReadByte();
            //}
            //outputFileStream.WriteByte((byte)largestByte);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            using FileStream inputFileStream = File.Open(InputFilePath, FileMode.Open, FileAccess.Read);
            using BinaryReader binaryReader = new BinaryReader(inputFileStream); 

            using FileStream outputFileStream = File.OpenWrite(OutputFilePath);
            using BinaryWriter binaryWriter = new BinaryWriter(outputFileStream);

            //byte largest = 0;
            int largest = 0;

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                //byte currentByte = binaryReader.ReadByte();
                int currentByte = binaryReader.ReadByte();

                binaryWriter.Write(currentByte); // writing a .NET int here

                if(currentByte > largest)
                {
                    largest = currentByte;
                }
            }

            binaryWriter.Write(largest); // writing a .NET int here
        }
    }
}
