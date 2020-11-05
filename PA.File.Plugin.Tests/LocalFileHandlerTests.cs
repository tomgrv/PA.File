// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PA.Converters.Extensions;
using PA.Plugin.FileHandler;

namespace PA.File.Plugin.Tests
{
    [TestFixture]
    public class LocalFileHandlerTests
    {
      

        [Test]
        public void TestList1()
        {
            var dir = TestFilesGenerator.GenerateFiles();

            Uri LocalUri = new Uri("file:///"+ dir.FullName +"/**/*.ext??");
            LocalFileHandler lfh = new LocalFileHandler(LocalUri);

            lfh.BatchSize = 1;

            var list = lfh.List().ToArray();
        }
    }
}
