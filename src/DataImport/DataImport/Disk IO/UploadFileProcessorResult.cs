namespace DataImport.Disk_IO
{
    using System.Collections.Generic;

    public class UploadFileProcessorResult<TCsv, TFrame>
    {
        public UploadFileProcessorResult(
            IReadOnlyCollection<TFrame> successfulReads,
            IReadOnlyCollection<TCsv> unsuccessfulReads)
        {
            this.SuccessfulReads = successfulReads ?? new List<TFrame>();
            this.UnsuccessfulReads = unsuccessfulReads ?? new List<TCsv>();
        }

        public IReadOnlyCollection<TFrame> SuccessfulReads { get; }

        public IReadOnlyCollection<TCsv> UnsuccessfulReads { get; }
    }
}