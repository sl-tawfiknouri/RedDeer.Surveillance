using System;
using System.IO;
using NUnit.Framework;
using Utilities.Disk_IO;

namespace Utilities.Tests.Disk_IO
{
    [TestFixture]
    public class ReddeerDirectoryTests
    {
        [Test]
        public void Create_DoesNotThrow_NullPath()
        {
            var directory = new ReddeerDirectory();

            var result = directory.Create(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Create_DoesNotThrow_EmptyPath()
        {
            var directory = new ReddeerDirectory();

            var result = directory.Create(string.Empty);

            Assert.IsFalse(result);
        }

        [Test]
        [Explicit]
        [Ignore("Windows only test")]
        public void Create_DoesThrowWhenArgumentHas_NonPathValue()
        {
            var directory = new ReddeerDirectory();

            Assert.Throws<ArgumentException>(() => directory.Create("er?:<!'/\b\aj4jh,a"));
        }

        [Test]
        public void Delete_DoesNotThrow_NullPath()
        {
            var directory = new ReddeerDirectory();

            var result = directory.Delete(null);

            Assert.IsFalse(result);
        }

        [Test]
        public void Delete_DoesNotThrow_EmptyPath()
        {
            var directory = new ReddeerDirectory();

            var result = directory.Delete(string.Empty);

            Assert.IsFalse(result);
        }

        [Test]
        [Explicit]
        [Ignore("Windows only test")]
        public void Delete_DoesThrowWhenArgumentHas_NonPathValue()
        {
            var directory = new ReddeerDirectory();

            Assert.Throws<ArgumentException>(() => directory.Delete("er?:<!'/\b\aj4jh,a"));
        }

        [Test]
        [Explicit]
        public void Create_And_Delete()
        {
            var directory = new ReddeerDirectory();

            var path = Path.Combine(Directory.GetCurrentDirectory(), "test subfolder");
            directory.Create(path);

            Assert.IsTrue(Directory.Exists(path));

            directory.Delete(path);

            Assert.IsFalse(Directory.Exists(path));
        }

        [Test]
        public void GetFiles_DoesNotThrow_NullPath()
        {
            var directory = new ReddeerDirectory();

            var result = directory.GetFiles(null, null);

            Assert.IsEmpty(result);
        }

        [Test]
        public void GetFiles_DoesNotThrow_EmptyPath()
        {
            var directory = new ReddeerDirectory();

            var result = directory.GetFiles(string.Empty, string.Empty);

            Assert.IsEmpty(result);
        }

        [Test]
        [Explicit]
        [Ignore("Windows only test")]
        public void GetFiles_DoesThrowWhenArgumentHas_NonPathValue()
        {
            var directory = new ReddeerDirectory();

            Assert.Throws<ArgumentException>(() => directory.GetFiles("er?:<!'/\b\aj4jh,a", string.Empty));
        }

        [Test]
        public void Exists_ReturnsTrueWhenFolder_Exists()
        {
            var directory = new ReddeerDirectory();
            var currentDirectory = Directory.GetCurrentDirectory();

            var exists = directory.DirectoryExists(currentDirectory);

            Assert.IsTrue(exists);
        }

        [Test]
        public void Exists_ReturnsFalseWhenFolder_DoesNotExist()
        {
            var directory = new ReddeerDirectory();
            var targetDirectory = "C:/aepjgaeijrg4j/aerjga34i91/argjag.aerg";

            var exists = directory.DirectoryExists(targetDirectory);

            Assert.IsFalse(exists);
        }
    }
}
