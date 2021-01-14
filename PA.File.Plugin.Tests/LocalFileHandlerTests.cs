// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation

using System;
using System.Linq;
using NUnit.Framework;
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

            var LocalUri = new Uri("file:///" + dir.FullName + "/**/*.ext??");
            var lfh = new LocalFileHandler(LocalUri);

            lfh.BatchSize = 1;

            var list = lfh.List().ToArray();
        }
    }
}