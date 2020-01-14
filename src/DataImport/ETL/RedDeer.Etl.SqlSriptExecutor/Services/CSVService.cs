using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using RedDeer.Etl.SqlSriptExecutor.Services.Interfaces;
using System;
using System.IO;
using System.Text;

namespace RedDeer.Etl.SqlSriptExecutor.Services
{
    public class CSVService 
        : ICSVService
    {
        private const string CRLF = "\r\n";
        private const string NewLine = "\\n";

        private readonly ILogger<CSVService> _logger;

        public CSVService(ILogger<CSVService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public int ReplaceNewLines(string fileName, Stream sourceStream, Stream destinationStream)
        {
            sourceStream.Seek(0, SeekOrigin.Begin);
            destinationStream.Seek(0, SeekOrigin.Begin);

            var crlfCounter = 0;

            var configuration = new Configuration
            {
                BadDataFound = (ctx) =>
                {
                    _logger.LogWarning($"BadDataFound error in file '{fileName}' (Row:'{ctx.RawRow}', ColumnIndex: '{ctx.ColumnCount}', Field:'{ctx.Field}', RawRecord: '{ctx.RawRecord}').");
                }
            };

            var streamReader = new StreamReader(sourceStream);
            var csvReader = new CsvReader(streamReader, configuration);

            var streamWriter = new StreamWriter(destinationStream, encoding: Encoding.UTF8, 1024, true);
            var csvWriter = new CsvWriter(streamWriter, configuration, leaveOpen: true);

            while (csvReader.Read())
            {
                for (int i = 0; i < csvReader.Context.Record.Length; i++)
                {
                    var fieldValue = csvReader.GetField(i);

                    if (fieldValue.Contains(CRLF))
                    {
                        crlfCounter++;

                        var newValue = fieldValue.Replace(CRLF, NewLine);
                        csvWriter.WriteField(newValue);
                    }
                    else
                    {
                        csvWriter.WriteField(fieldValue);
                    }
                }

                csvWriter.Flush();
                csvWriter.NextRecord();
            }

            csvReader.Dispose();
            csvWriter.Dispose();

            streamReader.Dispose();
            streamWriter.Dispose();

            return crlfCounter;
        }
    }
}
