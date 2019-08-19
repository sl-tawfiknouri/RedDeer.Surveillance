

// ReSharper disable UnusedMember.Global
namespace Infrastructure.Network.Disk.Interfaces
{
    using System.Collections.Generic;

    public interface IReddeerDirectory
    {
        bool Create(string path);

        bool Delete(string path);

        void DeleteFile(string path);

        bool DirectoryExists(string path);

        IReadOnlyCollection<string> GetFiles(string path, string fileMask);

        void Move(string originPath, string destinationPath);
    }
}