using System.ComponentModel.DataAnnotations;
using System.IO;

using GtaModdingCli.Commands.Classes;

namespace GtaModdingCli.Common
{
    /// <summary>
    /// Abstract class for CLI command
    /// </summary>
    public abstract class AbstractCliCommand : ICliCommand
    {
        #region Fields

        protected Cli cli;

        #endregion Fields

        #region Validators

        protected ValidationResult DirectoryExists(object dir)
        {
            return Directory.Exists((string)dir) 
                ? ValidationResult.Success 
                : new ValidationResult("Directory does not exist.");
        }

        protected ValidationResult FileExists(object file)
        {
            return File.Exists((string)file)
                ? ValidationResult.Success
                : new ValidationResult("File does not exist.");
        }

        #endregion Validators

        #region ICliComman

        protected AbstractCliCommand(Cli parent)
        {
            cli = parent;
        }

        /// <summary>
        /// Command execution
        /// </summary>
        /// <param name="args">Command arguments</param>
        public virtual void Execute(string[] args)
        {
        }

        public virtual string[] GetInteractiveData()
        {
            throw new System.NotImplementedException();
        }

        #endregion ICliComman
    }
}
