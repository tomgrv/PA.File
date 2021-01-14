using System.IO;

namespace PA.File.Plugin.Tests
{
    internal static class TestFilesGenerator
    {
        public static DirectoryInfo GenerateFiles()
        {
            var top = new DirectoryInfo(Path.GetTempPath() + "testfiles");
            var ext = new[] {"ext1", "ext2", "ext3"};
            var dir = new[] {"dir1", "dir2", "dir3"};

            top.Create();

            foreach (var d in dir)
            {
                var sub = top.CreateSubdirectory(d);

                foreach (var e in ext)
                    for (var i = 1; i < 30; i++)
                    {
                        var f = new FileInfo(sub.FullName + Path.DirectorySeparatorChar + "file_" + sub.Name + "_" + i +
                                             "." + e);
                        f.Create();
                    }
            }

            return top;
        }
    }
}