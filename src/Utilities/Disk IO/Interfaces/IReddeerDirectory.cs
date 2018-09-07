namespace Utilities.Disk_IO.Interfaces
{
    public interface IReddeerDirectory
    {
        bool Create(string path);
        bool Delete(string path);
    }
}