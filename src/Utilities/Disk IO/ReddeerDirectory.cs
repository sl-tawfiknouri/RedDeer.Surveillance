using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.Disk_IO.Interfaces;

namespace Utilities.Disk_IO
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
    }
}