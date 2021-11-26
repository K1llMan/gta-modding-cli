using GtaModdingCli.Commands.Classes;

namespace GtaModdingCli.Common
{
    /// <summary>
    /// CLI command interface
    /// </summary>
    public interface ICliCommand
    {
        void Execute(string[] args);

        string[] GetInteractiveData();
    }
}
