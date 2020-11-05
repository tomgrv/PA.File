using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PA.File.Plugin.Tests
{
    static class TestFilesGenerator
    {
        public static DirectoryInfo GenerateFiles()
        {
            var top = new DirectoryInfo(Path.GetTempPath() +  "testfiles");
            var ext = new string[] { "ext1", "ext2", "ext3" };
            var dir = new string[] { "dir1", "dir2", "dir3" };

            top.Create();

            foreach (var d in dir)
            {
                var sub = top.CreateSubdirectory(d);

                foreach (var e in ext)
                {
                    for (int i = 1; i < 30; i++)
                    {
                        var f = new FileInfo(sub.FullName + Path.DirectorySeparatorChar + "file_" + sub.Name+"_"+ i + "." + e);
                        f.Create();
                    }
                }
            }

            return top;
        }

    }
}
