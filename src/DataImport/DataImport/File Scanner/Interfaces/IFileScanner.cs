using System.Threading.Tasks;

namespace DataImport.File_Scanner.Interfaces
{
    public interface IFileScanner
    {
        Task Scan();
    }
}