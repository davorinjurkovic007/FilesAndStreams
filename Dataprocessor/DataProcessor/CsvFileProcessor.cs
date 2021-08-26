using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    public class CsvFileProcessor
    {
        private readonly IFileSystem fileSystem;

        public CsvFileProcessor(string inputFilePath, string outputFilePath, IFileSystem fileSystem)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
            this.fileSystem = fileSystem;
        }

        public CsvFileProcessor(string inputFilePath, string outputFilePath) : this(inputFilePath, outputFilePath, new FileSystem())
        {

        }

        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public void Process()
        {
            using StreamReader inputReader = fileSystem.File.OpenText(InputFilePath);

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Comment = '@',
                AllowComments = true,
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true, // default
                HasHeaderRecord = true, // default
                Delimiter = "," // default
                //HeaderValidated = null,
                //MissingFieldFound = null
             };
            using CsvReader csvReader = new CsvReader(inputReader, csvConfiguration);
            csvReader.Context.RegisterClassMap<ProcessOrderMap>();

            //IEnumerable<dynamic> records = csvReader.GetRecords<dynamic>();
            //IEnumerable<Order> records = csvReader.GetRecords<Order>();
            IEnumerable<ProcessedOrder> records = csvReader.GetRecords<ProcessedOrder>();

            //foreach(ProcessedOrder record in records)
            //{
            //    Console.WriteLine($"Order Number: {record.OrderNumber}");
            //    Console.WriteLine($"Customer: {record.Customer}");
            //    //Console.WriteLine($"Description: {record.Description}");
            //    Console.WriteLine($"Amount: {record.Amount}");

            //    //Console.WriteLine(record.OrderNumber);
            //    //Console.WriteLine(record.CustomerNumber);
            //    //Console.WriteLine(record.Description);
            //    //Console.WriteLine(record.Quantity);

            //    //Console.WriteLine(record.Field1);
            //    //Console.WriteLine(record.Field2);
            //    //Console.WriteLine(record.Field3);
            //    //Console.WriteLine(record.Field4);

            //}

            // Create output CSV file
            using StreamWriter output = fileSystem.File.CreateText(OutputFilePath);
            using var csvWriter = new CsvWriter(output, CultureInfo.InvariantCulture);

            //csvWriter.WriteRecords(records);

            csvWriter.WriteHeader<ProcessedOrder>();
            csvWriter.NextRecord();

            var recordsArray = records.ToArray();
            for(int i = 0; i < recordsArray.Length; i++)
            {
                //csvWriter.WriteRecord(recordsArray[i]);
                csvWriter.WriteField(recordsArray[i].OrderNumber);
                csvWriter.WriteField(recordsArray[i].Customer);
                csvWriter.WriteField(recordsArray[i].Amount);

                bool isLastRecord = i == recordsArray.Length - 1;

                if(!isLastRecord)
                {
                    csvWriter.NextRecord();
                }
            }
        }
    }
}
