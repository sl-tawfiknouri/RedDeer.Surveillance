using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Etl.SqlSriptExecutor.Services;
using System.IO;
using System.Text;

namespace RedDeer.Etl.SqlSriptExecutor.Tests.Services
{
    public class CSVServiceUnitTests
    {
        private CSVService _csvService;

        [SetUp]
        public void SetUp()
        {
            _csvService = new CSVService(new NullLogger<CSVService>());
        }

        [Test]
        public void ReplaceNewLines_WhenContainsEmbeddedNewLineInsideQuotes_ShouldReplace()
        {
            var csvLines = new string[] 
            { 
                "H1,H2,H3",
                "L2V1,\"L2\r\nV2\",L2V3",
                "L3V1,\"L3\r\nV2\",L3V3",
                "L4V1,L4V2,L4V3"
            };

            var csv = string.Join("\r\n", csvLines);

            var sourceStream = new MemoryStream();
            sourceStream.Write(Encoding.UTF8.GetBytes(csv));

            var destinationStream = new MemoryStream();
            var crlfCounter = _csvService.ReplaceNewLines("sample-test.csv", sourceStream, destinationStream);

            crlfCounter.Should().Be(2);

            destinationStream.Seek(0, SeekOrigin.Begin);
            var bytes = destinationStream.ToArray();
            
            var actual = Encoding.UTF8.GetString(bytes);

            var expectedCsvLines = new string[]
            {
                "H1,H2,H3",
                "L2V1,L2\\nV2,L2V3",
                "L3V1,L3\\nV2,L3V3",
                "L4V1,L4V2,L4V3"
            };


            // added by CSVHelper library
            var info = Encoding.UTF8.GetString(new byte[] { 239, 187, 191 });

            var expectedCsv = info + string.Join("\r\n", expectedCsvLines) + "\r\n";

            actual.Should().Be(expectedCsv);
            actual.Length.Should().Be(expectedCsv.Length);
        }
    }
}
