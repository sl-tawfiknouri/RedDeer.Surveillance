using System;
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
    }
}
