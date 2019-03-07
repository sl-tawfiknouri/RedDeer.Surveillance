using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.Network.Disk.Interfaces;

namespace Infrastructure.Network.Disk
{
    public class ReddeerDirectory : IReddeerDirectory
    {
        public bool Create(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var invalidCharacters = Path.GetInvalidPathChars();
            if (path.ToCharArray().Any(pa => invalidCharacters.Contains(pa)))
            {
                throw new ArgumentException();
            }

            try
            {
                Directory.CreateDirectory(path);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var invalidCharacters = Path.GetInvalidPathChars();
            if (path.ToCharArray().Any(pa => invalidCharacters.Contains(pa)))
            {
                throw new ArgumentException();
            }

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public IReadOnlyCollection<string> GetFiles(string path, string fileMask)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return new List<string>();
            }

            var invalidCharacters = Path.GetInvalidPathChars();
            if (path.ToCharArray().Any(pa => invalidCharacters.Contains(pa)))
            {
                throw new ArgumentException();
            }

            if (!Directory.Exists(path))
            {
                return new List<string>();
            }

            if (!string.IsNullOrWhiteSpace(fileMask))
            {
                return Directory.GetFiles(path, fileMask);
            }
            else
            {
                return Directory.GetFiles(path);
            }
        }

        public void Move(string originPath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(originPath)
                || string.IsNullOrWhiteSpace(destinationPath))
            {
                return;
            }

            var invalidCharacters = Path.GetInvalidPathChars();
            if (originPath.ToCharArray().Any(pa => invalidCharacters.Contains(pa)))
            {
                throw new ArgumentException();
            }

            if (destinationPath.ToCharArray().Any(pa => invalidCharacters.Contains(pa)))
            {
                throw new ArgumentException();
            }

            if (!File.Exists(originPath))
            {
                return;
            }

            if (File.Exists(destinationPath))
            {
                var fileName = Path.GetFileName(destinationPath);
                var newFileName = $"{Guid.NewGuid()}-{fileName}";
                destinationPath = destinationPath.Replace(fileName, newFileName);
            }

            File.Move(originPath, destinationPath);
        }

        public void DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
    }
}