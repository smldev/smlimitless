using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SMLimitless.Debug
{
	[Debug]
	public partial class DebugForm : Form
	{
		private const int MaximumDisplayedLines = 200;

		private int displayedLines = 0;
		private List<string> previousCommands = new List<string>();
		private int displayedCommandNumber = -1;

		public DebugForm()
		{
			InitializeComponent();
		}

		private void ButtonSubmit_Click(object sender, EventArgs e)
		{
			string fullCommand = this.TextCommand.Text;
			string[] commandWords = fullCommand.Split(' ');
			string command = commandWords[0];
			var arguments = commandWords.Skip(1);
			this.TextCommand.Text = "";

			if (!string.IsNullOrEmpty(command))
			{
				this.AddToLogText(command);
				this.ProcessCommand(command, arguments);
			}
		}

		private void TextCommand_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up)
			{
				this.SeekCommand(Direction.Up);
			}
			else if (e.KeyCode == Keys.Down)
			{
				this.SeekCommand(Direction.Down);
			}
			else
			{
				// Reset the currently displayed command number in case the user edits whatever's in the box
				this.displayedCommandNumber = -1;
			}
		}

		public void AddToLogText(string text)
		{
			text = string.Concat("> ", text, Environment.NewLine);
			if (displayedLines >= MaximumDisplayedLines)
			{
				// http://stackoverflow.com/a/16167194/2709212
				this.TextLog.Text = this.TextLog.Text.Substring(this.TextLog.Text.IndexOf(Environment.NewLine) + 1);
			}
			else
			{
				displayedLines++;
			}

			this.TextLog.Text = string.Concat(this.TextLog.Text, text);
		}

		private void ProcessCommand(string command, IEnumerable<string> arguments)
		{
			this.previousCommands.Add(command);

			switch (command.ToLowerInvariant())
			{
				case "log":
					if (arguments.Count() == 1)
					{
						if (arguments.First() == "on")
						{
							Logger.LoggingEnabled = true;
						}
						else if (arguments.First() == "off")
						{
							Logger.LoggingEnabled = false;
						}
					}
					else
					{
						Logger.LoggingEnabled = !Logger.LoggingEnabled;
					}
					string message = (Logger.LoggingEnabled) ? "Logging enabled." : "Logging disabled.";
					this.AddToLogText(message);
					break;
				case "physedit":
					Forms.PhysicsSettingsEditorForm physicsForm = new Forms.PhysicsSettingsEditorForm();
					physicsForm.SelectedObject = GameServices.SpriteBatch;
					physicsForm.Show();
					break;
				default:
					this.AddToLogText(string.Format("The command \"{0}\" does not exist.", command));
					break;
			}
		}

		private void SeekCommand(Direction direction)
		{
			if (this.previousCommands.Count == 0)
			{
				// There are no submitted commands.
				return;
			}
			
			if (direction == Direction.Down)
			{
				if (this.displayedCommandNumber == this.previousCommands.Count - 1)
				{
					// We're already at the bottom of the submitted commands.
					return;
				}
				else
				{
					// Seek to the next command.
					this.displayedCommandNumber++;
					this.TextCommand.Text = this.previousCommands[this.displayedCommandNumber];
					this.TextCommand.SelectionStart = 0;
				}
			}
			else if (direction == Direction.Up)
			{
				if (this.displayedCommandNumber == 0)
				{
					// We're already the the top of the submitted commands.
					return;
				}
				else if (this.displayedCommandNumber == -1)
				{
					// In the default case, we want to seek to whatever the last command was.
					this.displayedCommandNumber = this.previousCommands.Count - 1;
					this.TextCommand.Text = this.previousCommands[this.displayedCommandNumber];
					this.TextCommand.SelectionStart = 0;
				}
				else
				{
					// Seek to the previous command.
					this.displayedCommandNumber--;
					this.TextCommand.Text = this.previousCommands[this.displayedCommandNumber];
					this.TextCommand.SelectionStart = 0;
				}
			}
			else
			{
				throw new ArgumentException(String.Format("DebugForm.SeekCommand(Direction): You can only seek up or seek down through commands, not {0}.", direction));
			}
		}
	}
}
