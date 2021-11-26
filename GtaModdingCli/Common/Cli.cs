using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GtaModdingCli.Common
{
    /// <summary>
    /// Root CLI class
    /// </summary>
    public class Cli
    {
        #region Properties

        internal Type[] Commands { get; set; }

        public int CommandsCount => Commands.Length;

        #endregion Properties

        #region Main functions

        public Cli()
        {
            Commands = Assembly.GetEntryAssembly()?
                .GetTypes()
                .Where(t => t.GetCustomAttribute<CliCommandAttribute>() != null)
                .ToArray();
        }

        internal void ExecutePak(params string[] args)
        {
            args[0] = args[0].TrimEnd('\\') + "\\";

            Process process = new()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "unrealPak", "UnrealPak-With-Compression.bat"),
                    Arguments = string.Join(" ", args),
                    //WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "temp"),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.OutputDataReceived += (s, e) => {
                Console.WriteLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        /// <summary>
        /// List of available commands
        /// </summary>
        /// <returns></returns>
        public string[] GetCommands()
        {
            return Commands
                .Select(t => t.GetCustomAttribute<CliCommandAttribute>().Commands.First())
                .ToArray();
        }

        /// <summary>
        /// Get <typeref name="ICliCommand"/> by name
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        public ICliCommand GetCommand(string command)
        {
            Type commandType = Commands
                .FirstOrDefault(t => t.GetCustomAttribute<CliCommandAttribute>().Commands.Contains(command));

            if (commandType == null)
            {
                Console.WriteLine("Unknown command.");
                return null;
            }

            return (ICliCommand)Activator.CreateInstance(commandType, this);
        }

        /// <summary>
        /// Command execution
        /// </summary>
        /// <param name="args">Arguments</param>
        public void Execute(params string[] args)
        {
            ICliCommand commandInstance = GetCommand(args[0]);
            commandInstance.Execute(args[1..]);
        }

        #endregion Main functions
    }
}
