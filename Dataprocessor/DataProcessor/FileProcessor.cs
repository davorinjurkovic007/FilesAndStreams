using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    public class FileProcessor
    {
        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompleteDirectoryname = "complete";

        public string InputFilePath { get; }

        public FileProcessor(string filePath)
        {
            InputFilePath = filePath;
        }

        public void Process()
        {
            WriteLine($"Begin process of {InputFilePath}");

            // Check if file exists
            if(!File.Exists(InputFilePath))
            {
                WriteLine($"ERROR: file {InputFilePath} does not exist.");
                return;
            }

            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            WriteLine($"Root data path is {rootDirectoryPath}");

            // Check if backup dir exists
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);

            //if(!Directory.Exists(backupDirectoryPath))
            //{
                //WriteLine($"Creating {backupDirectoryPath}");
                WriteLine($"Attempting to create {backupDirectoryPath}");
                Directory.CreateDirectory(backupDirectoryPath);
            //}

            // Copy file to backup dir
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            WriteLine($"Copying {InputFilePath} to {backupFilePath}");

            // Third option true - file will be overwrite
            File.Copy(InputFilePath, backupFilePath, true);

            // Move to in progress dir
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            string inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
        }
    }
}
