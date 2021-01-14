using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace PA.Plugin.FileHandler
{
    [DefaultMember("Directory")]
    [PartNotDiscoverable]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileHandler : IPlugin
    {
        public FileHandler()
        {
        }

        public FileHandler(Uri u)
        {
            Value = u;
        }

        public Uri Value { get; }

        public int BatchSize { get; set; }

        public DateTime CurrentDate { get; set; }

        public void Dispose()
        {
        }

        public static bool IsNetworkPath(string path)
        {
            if (!path.StartsWith(@"/") && !path.StartsWith(@"\"))
            {
                var rootPath = Path.GetPathRoot(path); // get drive's letter
                var driveInfo = new DriveInfo(rootPath); // get info about the drive
                return driveInfo.DriveType == DriveType.Network; // return true if a network drive
            }

            return true; // is a UNC path
        }

        protected static bool IsFileLocked(FileInfo file)
        {
            try
            {
                if (!file.IsReadOnly && !IsNetworkPath(file.DirectoryName))
                    using (var stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        stream.Close();
                    }
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }

        #region DatePartition Helpers

        public delegate DateTime GetFileDate<T>(T o);

        public static DateTime GetFileInfoDate(FileInfo o)
        {
            return o.LastWriteTime;
        }

        public static int CompareFileDates(FileInfo x, FileInfo y)
        {
            return DateTime.Compare(x.LastWriteTime, y.LastWriteTime);
        }

        public static int GetDatePartition<T>(ref T[] files, int preferedsize, GetFileDate<T> d, DateTime newerthan)
        {
            return GetDatePartition(ref files, preferedsize, d, newerthan, DateTime.Now);
        }

        public static int GetDatePartition<T>(ref T[] files, int preferedsize, GetFileDate<T> d, DateTime newerthan,
            DateTime olderthan)
        {
            // Filter by date, extention
            files = Array.FindAll(files, delegate(T f) { return d(f) > newerthan && d(f) < olderthan; });

            // Sort by date ASC
            Array.Sort(files, delegate(T f1, T f2) { return DateTime.Compare(d(f1), d(f2)); });

            // Partitioning ~preferedsize, keep files of same date/time together
            var size = 0;
            for (size = files.Length < preferedsize ? files.Length : preferedsize; size < files.Length; size++)
                if (d(files[size - 1]) < d(files[size]))
                    break;

            // Resizing
            Array.Resize(ref files, size);

            // Return real partition size
            return size;
        }

        #endregion DatePartition Helpers
    }
}