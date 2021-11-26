# GTA Modding CLI

CLI (Command Line Interface) for GTA modding utility functions.

## Usage

.\GtaModdingCli.exe [command] [arguments]+

Argument list can be obtained by using "help" command:

`Example: .\GtaModdingCli.exe help material`

## Commands

**h|help** - CLI help.

**radio** - Radio Builder.

**material** - Material editing commands.

**pak** - UnreakPak utility usage.

**i** - Interactive mode.

## Examples
Command executing examples can be found in "examples" directory.

## Interactive mode
Interactive mode starts with command "i". Interactive mode helps executing commands in dialog.

## Building

1. Clone repository and initialize submodules

    ```
    git clone https://github.com/K1llMan/gta-radio-builder.git
    cd gta-radio-builder
    git submodule update --init --recursive
    ```
2. Build 
    ``` 
    dotnet build
    ```