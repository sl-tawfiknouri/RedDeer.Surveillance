namespace DataImport.File_Scanner.Interfaces
{
    using System.Threading.Tasks;

    public interface IFileScanner
    {
        Task Scan();
    }
}