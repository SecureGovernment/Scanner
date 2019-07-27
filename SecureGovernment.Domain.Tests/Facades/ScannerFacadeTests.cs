using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Facades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureGovernment.Domain.Tests.Facades
{
    [TestClass]
    public class ScannerFacadeTests
    {
        [TestMethod]
        public void Test_ScannerFacade_ConnectToTarget_ValidConnection()
        {
            // Arrange
            var facade = new ScannerFacade();

            // Act
            var workerTask = facade.ConnectToTarget("alexgebhard.com");
            var result = workerTask;

            // Assert
        }
    }
}
