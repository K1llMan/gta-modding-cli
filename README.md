# GTA Modding CLI

CLI (Command Line Interface) for GTA modding utility functions.

## Usage

.\GtaModdingCli.exe [command] [argument]

Argument list can be obtained by using "help" command:

`Example: .\GtaModdingCli.exe help material`

## Commands

**h|help** - CLI help

**radio** - Radio Builder function

**material** - texture path replacing command in material

All commands has example bat.

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