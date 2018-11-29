namespace Relay.Disk_IO.Interfaces
{
    public interface IBaseUploadFileProcessor<TCsv, TFrame>
    {
        UploadFileProcessorResult<TCsv, TFrame> Process(string path);
    }
}