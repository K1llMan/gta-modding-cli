using System;
using System.Linq;
using System.Reflection;

using GtaModdingCli.Commands.Classes;
using GtaModdingCli.Common;

using Sharprompt;

namespace GtaModdingCli.Commands
{
    [CliCommand("Interactive mode.",
        "Usage: i", 
        "i"
    )]
    public class Interactive : AbstractCliCommand
    {
        private string exitCommand = "exit";

        public override void Execute(string[] args)
        {
            while (true)
            {
                string[] answers = GetInteractiveData();
                if (answers.First() == exitCommand)
                    return;

                ICliCommand command = cli.GetCommand(answers.First());
                if (command == null)
                    return;

                answers = command.GetInteractiveData();
                command.Execute(answers);

                Console.Write("Press any key...");
                Console.ReadKey();

                Console.Clear();
            }
        }

        public override string[] GetInteractiveData()
        {
            return new[] { 
                Prompt.Select("Select command", cli.GetCommands().Concat(new[] { exitCommand }))
            };
        }

        public Interactive(Cli cli) : base(cli)
        {

        }
    }
}
