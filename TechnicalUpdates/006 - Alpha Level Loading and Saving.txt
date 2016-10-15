# Technical Update #006 - Alpha Level Loading and Saving

## Description

An alpha-level way to load and save level files will be added to SML.

There are two methods to load files: pass it as a command line argument, or, if no argument is provided, an `OpenFileDialog`.

The method of saving files remains mostly the same: Press `G` to open a `SaveFileDialog` to pick a path.

## Implementation
	* A new property `string SmlProgram.InitialLevelFilePath` will be added and passed by `Program.Main(string[])`. This path is checked in the initialization code.
	* The initialization code will launch a dialog if no path is found.