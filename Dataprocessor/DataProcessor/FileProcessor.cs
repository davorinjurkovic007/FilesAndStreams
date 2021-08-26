using System;
using System.IO;
using static System.Console;

namespace DataProcessor
{
    public class FileProcessor
    {
        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompleteDirectoryName = "complete";

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

            if(File.Exists(inProgressFilePath))
            {
                WriteLine($"ERROR: a file with the name {inProgressFilePath} is already being processed");
                return;
            }

            WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);

            // Determine type of file
            string extension = Path.GetExtension(InputFilePath);

            // Move file after processing is complete
            string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompleteDirectoryName);
            Directory.CreateDirectory(completedDirectoryPath);
            WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");
            //File.Move(inProgressFilePath, Path.Combine(completedDirectoryPath, inputFileName));

            string completeFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{extension}";

            // If you got a file path and you just want to change the extension of the file name, you can call the Path.ChangeExtension method
            //completeFileName = Path.ChangeExtension(completeFileName, ".complete");

            var completedFilePath = Path.Combine(completedDirectoryPath, completeFileName);

            switch (extension)
            {
                case ".txt":
                    //ProcessTextFile(inProgressFilePath);
                    var textProcessor = new TextFileProcessor(inProgressFilePath, completedFilePath);
                    textProcessor.Process();
                    break;
                case ".data":
                    var binaryProcessor = new BinaryFileProcessor(inProgressFilePath, completedFilePath);
                    binaryProcessor.Process();
                    break;
                case ".csv":
                    var csvProcessor = new CsvFileProcessor(inProgressFilePath, completedFilePath);
                    csvProcessor.Process();
                    break;
                default:
                    WriteLine($"{extension} is an unsupported file type.");
                    break;
            }

            //// Move file after processing is complete
            //string completedDirectoryPath = Path.Combine(rootDirectoryPath, CompleteDirectoryName);
            //Directory.CreateDirectory(completedDirectoryPath);
            //WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");
            ////File.Move(inProgressFilePath, Path.Combine(completedDirectoryPath, inputFileName));

            //string completeFileName = $"{Path.GetFileNameWithoutExtension(InputFilePath)}-{Guid.NewGuid()}{extension}";

            //// If you got a file path and you just want to change the extension of the file name, you can call the Path.ChangeExtension method
            ////completeFileName = Path.ChangeExtension(completeFileName, ".complete");

            //var completedFilePath = Path.Combine(completedDirectoryPath, completeFileName);

            //File.Move(inProgressFilePath, completedFilePath);

            //// Delete the inProgressDirectory once the processing of the file is complete.
            //string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);
            ////Directory.Delete(inProgressDirectoryPath, true);
            ///
            WriteLine($"Completed processing of {inProgressFilePath}");

            WriteLine($"Deleting {inProgressFilePath}");
            File.Delete(inProgressFilePath);
        }

        //private void ProcessTextFile(string inProgressFilePath)
        //{
        //    WriteLine($"Process text file {inProgressFilePath}");
        //    // Read in and process
        //}
    }
}
