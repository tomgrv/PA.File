namespace PA.File.Plugin.Interfaces
{
    public interface IPluginImporter : IFilePlugin
    {
        IFileObject Load(string Filename);
    }
}