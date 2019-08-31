namespace SecureGovernment.Domain.Interfaces.Infastructure
{
    public interface IProcessService
    {
        string StartProcess(string path, string args);
    }
}
