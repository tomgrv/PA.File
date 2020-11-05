using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace PA.Plugin.Interfaces
{
    [InheritedExport]
    public interface IFileHandlerPlugin : IPlugin<Uri>
    {
        DateTime CurrentDate { get; set; }
        int BatchSize { get; set; }

        IEnumerable<string> List();

        void Get(ref string file);

        void Get(ref string[] files);

        void Put(string file);

        void Put(string[] files);
    }
}