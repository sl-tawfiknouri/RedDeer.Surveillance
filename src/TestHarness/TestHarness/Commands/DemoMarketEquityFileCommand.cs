namespace TestHarness.Commands
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using TestHarness.Commands.Interfaces;
    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Factory.Interfaces;

    public class DemoMarketEquityFileCommand : ICommand
    {
        public static string FileDirectory = "PlayMarketFiles";

        private readonly IAppFactory _appFactory;

        private IEquityDataGenerator _fileProcessor;

        public DemoMarketEquityFileCommand(IAppFactory appFactory)
        {
            this._appFactory = appFactory ?? throw new ArgumentNullException(nameof(appFactory));
        }

        public bool Handles(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return false;

            if (command.ToLower().Contains("stop demo equity market file")) return true;

            if (!command.ToLower().Contains("run demo equity market file")) return false;

            var fileSegment = Regex.Replace(
                command,
                "run demo equity market file ",
                string.Empty,
                RegexOptions.IgnoreCase);
            fileSegment = fileSegment.Trim();

            if (string.IsNullOrWhiteSpace(fileSegment)) return false;

            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            if (!File.Exists(playFileFullPath)) return false;

            var fileExtension = playFileFullPath.Split('.').Reverse().FirstOrDefault();

            if (string.Equals(fileExtension, "csv", StringComparison.InvariantCultureIgnoreCase)) return true;

            return false;
        }

        public void Run(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            var hasRunDemoMarketEquityFileCommandSegment = command.ToLower().Contains("run demo equity market file");
            if (hasRunDemoMarketEquityFileCommandSegment) this.RunDemo(command);

            var hasStopDemoMarketEquityFileCommandSegment = command.ToLower().Contains("stop demo equity market file");
            if (hasStopDemoMarketEquityFileCommandSegment) this.StopDemo();
        }

        private string GetEquityFilePath(string command)
        {
            var fileSegment = Regex.Replace(
                command,
                "run demo equity market file ",
                string.Empty,
                RegexOptions.IgnoreCase);
            fileSegment = fileSegment.Trim();
            var playFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), FileDirectory);
            var playFileFullPath = Path.Combine(playFileDirectory, fileSegment);

            return playFileFullPath;
        }

        private void RunDemo(string command)
        {
            var console = this._appFactory.Console;
            var equityStream = this._appFactory.StockExchangeStreamFactory.CreateDisplayable(console);
            var filePath = this.GetEquityFilePath(command);
            this._fileProcessor = this._appFactory.EquitiesFileDataImportProcessFactory.Create(filePath);

            this._fileProcessor.InitiateWalk(equityStream);
        }

        private void StopDemo()
        {
            this._fileProcessor?.TerminateWalk();
        }
    }
}