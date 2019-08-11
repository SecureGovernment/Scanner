using SecureGovernment.Domain.Interfaces.Infastructure;
using System.IO;

namespace SecureGovernment.Domain.Infastructure
{
    public class FileSystem : IFileSystem
    {
        public bool Exists(string path) => File.Exists(path);
    }
}
