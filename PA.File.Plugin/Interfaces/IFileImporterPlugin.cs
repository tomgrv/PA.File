using PA.Plugin.Operations.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PA.File.Plugin.Interfaces
{
    public interface IPluginImporter : IFilePlugin
    {
        IFileObject Load(string Filename);
    }
}
