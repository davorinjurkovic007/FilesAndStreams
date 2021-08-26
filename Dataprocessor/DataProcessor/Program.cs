using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using static System.Console;

namespace DataProcessor
{
    class Program
    {
        //private static ConcurrentDictionary<string, string> FilesToProcess = new ConcurrentDictionary<string, string>();

        private static MemoryCache FilesToProcess = MemoryCache.Default;

        static void Main(string[] args)
        {
            WriteLine("Parsing command line options");

            //// Commnad line validatio omitted for brevity

            //var command = args[0];

            //if(command == "--file")
            //{
            //    var filePath = args[1];
            //    WriteLine($"Single file {filePath} selected");
            //    ProcessSingleFile(filePath);
            //}
            //else if(command == "--dir")
            //{
            //    var directoryPath = args[1];
            //    var fileType = args[2];
            //    ProcessDirectory(directoryPath, fileType);
            //}
            //else
            //{
            //    WriteLine("Invalid command line options");
            //}

            //WriteLine("Press enter to quit.");
            //ReadLine();

            var directoryToWatch = args[0];

            if(!Directory.Exists(directoryToWatch))
            {
                WriteLine($"ERROR: {directoryToWatch} does not exist");
                ReadLine();
            }
            else
            {
                WriteLine($"Watching directory {directoryToWatch} for changes");

                ProcessExistingFiles(directoryToWatch);

                using (var inputFileWatcher = new FileSystemWatcher(directoryToWatch))
                {
                    //using (var timer = new Timer(ProcessFiles, null, 0, 1000))
                    //{

                        inputFileWatcher.IncludeSubdirectories = false;
                        inputFileWatcher.InternalBufferSize = 32768; // 32 KB
                                                                     // If we just wanted change notification for a specific file type, we could specify the Filter property.
                                                                     // So, for example, we could limit this to files that have a .txt extension. Now we'll only get notifications for .txt files
                                                                     //inputFileWatcher.Filter = "*.txt";
                        inputFileWatcher.Filter = "*.*"; // this is the default
                        inputFileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

                        inputFileWatcher.Created += FileCreated;
                        inputFileWatcher.Changed += FileChanged;
                        inputFileWatcher.Deleted += FileDeleted;
                        inputFileWatcher.Renamed += FileRenamed;
                        inputFileWatcher.Error += WatcherError;

                        inputFileWatcher.EnableRaisingEvents = true;

                        WriteLine("Press enter to quit.");
                        ReadLine();
                    //}
                }
            }
        }

        private static void FileCreated(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File created: {e.Name} - type: {e.ChangeType}");

            //ProcessSingleFile(e.FullPath);
            //FilesToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);
        }

        private static void FileChanged(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File changed: {e.Name} - type: {e.ChangeType}");

            //ProcessSingleFile(e.FullPath);
            //FilesToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);
        }

        private static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            WriteLine($"* File deleted: {e.Name} - type: {e.ChangeType}");
        }

        private static void FileRenamed(object sender, RenamedEventArgs e)
        {
            WriteLine($"* File renamed: {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }

        private static void WatcherError(object sender, ErrorEventArgs e)
        {
            WriteLine($"ERROR: file system watching may to longer be active: {e.GetException()}");
        }

        private static void ProcessDirectory(string directoryPath, string fileType)
        {
            // string[] allFiles = Directory.GetFiles(directoryPath);

            switch(fileType)
            {
                case "TEXT":
                    string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
                    foreach(var textFilePath in textFiles)
                    {
                        var fileProcessor = new FileProcessor(textFilePath);
                        fileProcessor.Process();
                    }
                    break;
                default:
                    WriteLine($"ERROR: {fileType} is not supported");
                    return;
            }
        }

        private static void ProcessSingleFile(string filePath)
        {
            var fileProcessor = new FileProcessor(filePath);
            fileProcessor.Process();
        }

        //private static void ProcessFiles(object stateInfo)
        //{
        //    foreach(var fileName in FilesToProcess.Keys)
        //    {
        //        if(FilesToProcess.TryRemove(fileName, out _))
        //        {
        //            var fileProcessor = new FileProcessor(fileName);
        //            fileProcessor.Process();
        //        }
        //    }
        //}

        private static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);

            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                // Effectively, what this is going to do is if the CacheItem hasn't been updated for 2 seconds, the ProcessFile methos will be called
                SlidingExpiration = TimeSpan.FromSeconds(2)
            };

            FilesToProcess.Add(item, policy);
        }

        private static void ProcessFile(CacheEntryRemovedArguments arguments)
        {
            WriteLine($"* Cache item removed: {arguments.CacheItem.Key} becouse {arguments.RemovedReason}");

            if(arguments.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(arguments.CacheItem.Key);
                fileProcessor.Process();
            }
            else
            {
                WriteLine($"WARNING: {arguments.CacheItem.Key} was removed unexpectedly and may not be processed because {arguments.RemovedReason}");
            }
        }

        private static void ProcessExistingFiles(string directoryToWatch)
        {
            WriteLine($"Checking {directoryToWatch} for existing files");

            foreach(var filePath in Directory.EnumerateFiles(directoryToWatch))
            {
                WriteLine($"   - Found {filePath}");
                AddToCache(filePath);
            }
        }
    }
}
