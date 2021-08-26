using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    public class TextFileProcessor
    {
        public TextFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public void Process()
        {
            //// Using read all text
            ////string originalText = File.ReadAllText(InputFilePath);
            ////string processedText = originalText.ToUpperInvariant();
            ////File.WriteAllText(OutputFilePath, processedText);

            //// Using read all lines
            //string[] lines = File.ReadAllLines(InputFilePath);
            //lines[1] = lines[1].ToUpperInvariant(); // Assumes there is a line 2 in the file

            //try
            //{
            //    File.WriteAllLines(OutputFilePath, lines);
            //}
            //catch(Exception ex)
            //{
            //    // Retry
            //    // Log
            //    // throw
            //    throw;
            //}

            //using var inputFileStream = new FileStream(InputFilePath, FileMode.Open);
            //using var inputStreamReader = new StreamReader(inputFileStream);
            using StreamReader inputStreamReader = File.OpenText(InputFilePath);

            //using var outputFileStream = new FileStream(OutputFilePath, FileMode.CreateNew);
            //using var outputStreamWriter = new StreamWriter(outputFileStream);
            using var outputStreamWriter = new StreamWriter(OutputFilePath);

            var currentLineNumber = 1;
            while(!inputStreamReader.EndOfStream)
            {
                string inputLine = inputStreamReader.ReadLine();

                if(currentLineNumber == 2)
                {
                    inputLine = inputLine.ToUpperInvariant();
                }
                //string processedLine = inputLine.ToUpperInvariant();

                bool isLastLine = inputStreamReader.EndOfStream;

                if (isLastLine)
                {
                    //outputStreamWriter.Write(processedLine);
                    outputStreamWriter.Write(inputLine);
                }
                else
                {
                    //outputStreamWriter.WriteLine(processedLine);
                    outputStreamWriter.WriteLine(inputLine);
                }

                currentLineNumber++;
            }
        }
    }
}
