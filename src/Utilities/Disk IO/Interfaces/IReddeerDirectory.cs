﻿using System.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace Infrastructure.Network.Disk_IO.Interfaces
{
    public interface IReddeerDirectory
    {
        bool Create(string path);
        bool Delete(string path);
        void DeleteFile(string path);
        IReadOnlyCollection<string> GetFiles(string path, string fileMask);
        void Move(string originPath, string destinationPath);
        bool DirectoryExists(string path);
    }
}