using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Logging;
using PA.Plugin.Interfaces;

namespace PA.Plugin.FileHandler
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PluginDescription("Local File Handler")]
    [ExportWithLabel(typeof(IFileHandlerPlugin), "file")]
    public class LocalFileHandler : FileHandler, IFileHandlerPlugin
    {
        private static readonly ILog log = LogManager.GetLogger<LocalFileHandler>();

        public LocalFileHandler()
        {
        }

        [ImportingConstructor]
        public LocalFileHandler(Uri uri)
            : base(uri)
        {
        }

        public IEnumerable<string> List()
        {
            log.Info("List " + Value.LocalPath);

            if (System.IO.File.Exists(Value.LocalPath))
            {
                var lastw = System.IO.File.GetLastWriteTime(Value.LocalPath);

                if (lastw > CurrentDate)
                {
                    CurrentDate = lastw;
                    yield return Value.LocalPath;
                }
            }
            else
            {
                var files = Glob.Glob.Expand(Value.LocalPath).Select(
                    f => new FileInfo(f.FullName)
                ).ToArray();

                log.Info("Found " + files.Count() + " files");

                if (files.Any())
                    GetDatePartition(ref files, BatchSize > 0 ? BatchSize : files.Count(), GetFileInfoDate,
                        CurrentDate);

                log.Info("List " + files.Count() + " files");

                foreach (var f in files)
                {
                    yield return f.FullName;
                    CurrentDate = f.LastWriteTime;
                }
            }
        }

        public void Get(ref string[] files)
        {
            for (var i = 0; i < files.Length; i++) Get(ref files[i]);
        }

        public void Get(ref string file)
        {
            var remote = new FileInfo(file);
            var local = new FileInfo(Path.GetTempPath() + "\\" + remote.Name);

            while (IsFileLocked(remote))
            {
                log.Warn("File <" + remote.FullName + "> is locked!");
                Thread.Sleep(10000);
            }

            if (remote.Exists)
            {
                file = local.FullName;

                try
                {
                    System.IO.File.Copy(remote.FullName, file, true);
                    System.IO.File.SetCreationTime(file, remote.CreationTime);
                    log.Info("File <" + local.FullName + "> retrieved from <" + remote.DirectoryName + ">");
                }
                catch (Exception e)
                {
                    log.Warn("File <" + local.FullName + "> cannot be retrieved from <" + remote.DirectoryName + ">\n" +
                             e.Message);
                }
            }
            else
            {
                log.Warn("File <" + remote.FullName + "> not found!");
            }
        }

        public void Put(string[] files)
        {
            for (var i = 0; i < files.Length; i++)
                if (files[i] is string)
                    Put(files[i]);
        }

        public void Put(string file)
        {
            var local = new FileInfo(file);
            var remote = new FileInfo(Value.LocalPath + local.Name);

            if (local.FullName != remote.FullName)
            {
                while (IsFileLocked(local))
                {
                    log.Warn("File <" + local.FullName + "> is locked!");
                    Thread.Sleep(10000);
                }

                if (local.Exists)
                    try
                    {
                        System.IO.File.Copy(local.FullName, remote.FullName, true);
                        System.IO.File.SetCreationTime(remote.FullName, local.CreationTime);
                        log.Info("File <" + local.FullName + "> pushed to <" + remote.DirectoryName + ">!");
                    }
                    catch (Exception e)
                    {
                        log.Warn("File <" + local.FullName + "> cannot be pushed to <" + remote.DirectoryName + ">\n" +
                                 e.Message);
                    }
                else
                    log.Warn("File <" + local.FullName + "> not found!");
            }
        }
    }
}