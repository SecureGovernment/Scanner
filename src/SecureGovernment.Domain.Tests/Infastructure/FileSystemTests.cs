using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Infastructure;
using System;
using System.IO;

namespace SecureGovernment.Domain.Tests.Infastructure
{
    [TestClass]
    public class FileSystemTests
    {
        [TestMethod]
        public void Test_FileSystem_Exists_FileDoesNotExist()
        {
            //Arrange
            var fileSystem = new FileSystem();

            //Act
            var exists = fileSystem.Exists("DOESNOTEXIST");

            //Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void Test_FileSystem_Exists_FileExists()
        {
            //Arrange
            var fileSystem = new FileSystem();

            //Act
            var exists = fileSystem.Exists(Path.Combine(Environment.CurrentDirectory, "SecureGovernment.Domain.dll"));

            //Assert
            Assert.IsTrue(exists);
        }
    }
}
