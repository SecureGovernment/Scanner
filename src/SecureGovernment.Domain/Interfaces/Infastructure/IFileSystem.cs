namespace SecureGovernment.Domain.Interfaces.Infastructure
{
    public interface IFileSystem
    {
        bool Exists(string path);
    }
}
