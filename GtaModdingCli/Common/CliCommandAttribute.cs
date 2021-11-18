using System;

namespace GtaModdingCli.Common
{
    public class CliCommandAttribute : Attribute
    {
        /// <summary>
        /// List of possible commands
        /// </summary>
        public string[] Commands { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Usage info
        /// </summary>
        public string Usage { get; set; }

        public CliCommandAttribute(string description, string usage, params string[] commands)
        {
            Commands = commands;
            Description = description;
            Usage = usage;
        }
    }
}
