
using System.IO;
namespace PA.File.Plugin.Interfaces
{

    public interface IFileExporterPlugin: IFilePlugin
    {
        void Save(string Filename, object Data);
    }

   
}
