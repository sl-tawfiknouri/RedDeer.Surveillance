using System.Collections.Generic;

namespace Relay.Disk_IO
{
    public class UploadFileProcessorResult<TCsv, TFrame>
    {
        public UploadFileProcessorResult(
            IReadOnlyCollection<TFrame> successfulReads,
            IReadOnlyCollection<TCsv> unsuccessfulReads)
        {
            SuccessfulReads = successfulReads ?? new List<TFrame>();
            UnsuccessfulReads = unsuccessfulReads ?? new List<TCsv>();
        }

        public IReadOnlyCollection<TFrame> SuccessfulReads { get; }
        public IReadOnlyCollection<TCsv> UnsuccessfulReads { get; }
    }
}
