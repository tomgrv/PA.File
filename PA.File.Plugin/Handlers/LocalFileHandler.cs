using PA.Plugin.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Common.Logging;

namespace PA.Plugin.FileHandler
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [PluginDescription("Local File Handler")]
    [ExportWithLabel(typeof(IFileHandlerPlugin), "file")]
    public class LocalFileHandler : FileHandler, IFileHandlerPlugin
    {
        public LocalFileHandler()
        {
        }

        [ImportingConstructor]
        public LocalFileHandler(Uri uri)
            : base(uri)
        {

        }

        private static readonly ILog log = LogManager.GetLogger<LocalFileHandler>();

        public IEnumerable<string> List()
        {
            log.Info("List " + this.Value.LocalPath);

            if (System.IO.File.Exists(this.Value.LocalPath))
            {
                var lastw = System.IO.File.GetLastWriteTime(this.Value.LocalPath);

                if (lastw > this.CurrentDate)
                {
                    this.CurrentDate = lastw;
                    yield return this.Value.LocalPath;
                }
            }
            else
            {
                var files = Glob.Glob.Expand(this.Value.LocalPath).Select(
                    f => new FileInfo(f.FullName)
                    ).ToArray();

                log.Info("Found " + files.Count() + " files");

                if (files.Count() > 0)
                {
                    FileHandler.GetDatePartition<FileInfo>(ref files, this.BatchSize > 0 ? this.BatchSize : files.Count(), FileHandler.GetFileInfoDate, this.CurrentDate);
                }

                log.Info("List " + files.Count() + " files");

                foreach (FileInfo f in files)
                {
                    yield return f.FullName;
                    this.CurrentDate = f.LastWriteTime;
                }
            }
        }

        public void Get(ref string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                this.Get(ref files[i]);
            }
        }

        public void Get(ref string file)
        {
            FileInfo remote = new FileInfo(file);
            FileInfo local = new FileInfo(Path.GetTempPath() + "\\" + remote.Name);

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
                    log.Warn("File <" + local.FullName + "> cannot be retrieved from <" + remote.DirectoryName + ">\n" + e.Message);
                }

            }
            else
            {
                log.Warn("File <" + remote.FullName + "> not found!");
            }

        }

        public void Put(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] is string)
                {
                    this.Put(files[i]);
                }
            }
        }

        public void Put(string file)
        {
            FileInfo local = new FileInfo(file);
            FileInfo remote = new FileInfo(this.Value.LocalPath + local.Name);

            if (local.FullName != remote.FullName)
            {
                while (IsFileLocked(local))
                {
                    log.Warn("File <" + local.FullName + "> is locked!");
                    Thread.Sleep(10000);
                }

                if (local.Exists)
                {
                    try
                    {
                        System.IO.File.Copy(local.FullName, remote.FullName, true);
                        System.IO.File.SetCreationTime(remote.FullName, local.CreationTime);
                        log.Info("File <" + local.FullName + "> pushed to <" + remote.DirectoryName + ">!");
                    }
                    catch (Exception e)
                    {
                        log.Warn("File <" + local.FullName + "> cannot be pushed to <" + remote.DirectoryName + ">\n" + e.Message);
                    }
                }
                else
                {
                    log.Warn("File <" + local.FullName + "> not found!");
                }
            }
        }
    }
}