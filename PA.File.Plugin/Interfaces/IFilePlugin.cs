using System.IO;
using System.ComponentModel.Composition;
using PA.Plugin;

namespace PA.File.Plugin.Interfaces
{
    [InheritedExport]
    public interface IFilePlugin : IPlugin, IFileObject
    {
        
    }
}
