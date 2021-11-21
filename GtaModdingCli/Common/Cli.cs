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
            Process process = new()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "unrealPak", "UnrealPak-With-Compression.bat"),
                    Arguments = string.Join(" ", args),
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "temp"),
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
        /// Command execution
        /// </summary>
        /// <param name="args">Arguments</param>
        public void Execute(params string[] args)
        {
            string command = args[0];
            Type commandType = Commands
                .FirstOrDefault(t => t.GetCustomAttribute<CliCommandAttribute>().Commands.Contains(command));

            if (commandType == null)
                throw new Exception("Unknown command");

            ICliCommand commandInstance = (ICliCommand)Activator.CreateInstance(commandType, this);
            commandInstance.Execute(args[1..]);
        }

        #endregion Main functions
    }
}
