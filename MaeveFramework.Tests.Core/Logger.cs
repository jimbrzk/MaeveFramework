using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaeveFramework.Tests.Core
{
    [TestClass]
    public class Logger
    {
        [TestMethod]
        public void TestDebug()
        {
            MaeveFramework.Logger.LoggingManager.UseDebug();
            var logger = MaeveFramework.Logger.LoggingManager.GetLogger("TEST");
            Assert.IsTrue(logger != null, "Failed to create logger");

            logger.Info(new Exception("TEST"));
            logger.Info("TEST");
            logger.Info(new Exception("TEST"), "TEST");

            logger.Warn(new Exception("TEST"));
            logger.Warn("TEST");
            logger.Warn(new Exception("TEST"), "TEST");

            logger.Error(new Exception("TEST"));
            logger.Error("TEST");
            logger.Error(new Exception("TEST"), "TEST");

            logger.Debug(new Exception("TEST"));
            logger.Debug("TEST");
            logger.Debug(new Exception("TEST"), "TEST");

            logger.Trace(new Exception("TEST"));
            logger.Trace("TEST");
            logger.Trace(new Exception("TEST"), "TEST");
        }

        [TestMethod]
        public void TestConsole()
        {
            MaeveFramework.Logger.LoggingManager.UseConsole();
            var logger = MaeveFramework.Logger.LoggingManager.GetLogger("TEST");
            Assert.IsTrue(logger != null, "Failed to create logger");

            logger.Info(new Exception("TEST"));
            logger.Info("TEST");
            logger.Info(new Exception("TEST"), "TEST");

            logger.Warn(new Exception("TEST"));
            logger.Warn("TEST");
            logger.Warn(new Exception("TEST"), "TEST");

            logger.Error(new Exception("TEST"));
            logger.Error("TEST");
            logger.Error(new Exception("TEST"), "TEST");

            logger.Debug(new Exception("TEST"));
            logger.Debug("TEST");
            logger.Debug(new Exception("TEST"), "TEST");

            logger.Trace(new Exception("TEST"));
            logger.Trace("TEST");
            logger.Trace(new Exception("TEST"), "TEST");
        }

        [TestMethod]
        public void TestNLog()
        {
            MaeveFramework.Logger.LoggingManager.UseNLog();
            var logger = MaeveFramework.Logger.LoggingManager.GetLogger("TEST");
            Assert.IsTrue(logger != null, "Failed to create logger");

            logger.Info(new Exception("TEST"));
            logger.Info("TEST");
            logger.Info(new Exception("TEST"), "TEST");

            logger.Warn(new Exception("TEST"));
            logger.Warn("TEST");
            logger.Warn(new Exception("TEST"), "TEST");

            logger.Error(new Exception("TEST"));
            logger.Error("TEST");
            logger.Error(new Exception("TEST"), "TEST");

            logger.Debug(new Exception("TEST"));
            logger.Debug("TEST");
            logger.Debug(new Exception("TEST"), "TEST");

            logger.Trace(new Exception("TEST"));
            logger.Trace("TEST");
            logger.Trace(new Exception("TEST"), "TEST");
        }
    }
}
