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

        #endregion ICliComman
    }
}
