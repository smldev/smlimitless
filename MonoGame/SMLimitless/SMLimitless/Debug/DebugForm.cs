using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SMLimitless.Forms;

namespace SMLimitless.Debug
{
	/// <summary>
	///   A form that exposes debugging logs and commands to the user.
	/// </summary>
	[Debug]
	public partial class DebugForm : Form
	{
		/// <summary>
		///   The maximum number of lines that will be displayed in the log
		///   textbox before it begins to remove old lines.
		/// </summary>
		private const int MaximumDisplayedLines = 200;

		/// <summary>
		///   The index of the command number that is currently being displayed
		///   (changed when the user uses KeyUp/KeyDown to change the command).
		/// </summary>
		private int displayedCommandNumber = -1;

		/// <summary>
		///   The number of lines in the log textbox.
		/// </summary>
		private int displayedLines = 0;

		/// <summary>
		///   A collection of previously submitted commands.
		/// </summary>
		private List<string> previousCommands = new List<string>();

		/// <summary>
		///   Initializes a new instance of the <see cref="DebugForm" /> class.
		/// </summary>
		public DebugForm()
		{
			InitializeComponent();
		}

		/// <summary>
		///   Adds a string to the debug log text.
		/// </summary>
		/// <param name="text">The text to add.</param>
		public void AddToLogText(string text)
		{
			text = string.Concat("> ", text, Environment.NewLine);
			if (displayedLines >= MaximumDisplayedLines)
			{
				// http://stackoverflow.com/a/16167194/2709212
				TextLog.Text = TextLog.Text.Substring(TextLog.Text.IndexOf(Environment.NewLine) + 1);
			}
			else
			{
				displayedLines++;
			}

			TextLog.AppendText(text);
		}

		/// <summary>
		///   This method is called when the Submit button is clicked.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">Arguments for this event.</param>
		private void ButtonSubmit_Click(object sender, EventArgs e)
		{
			SubmitCommand();
		}

		private void ProcessCommand(string command, IEnumerable<string> arguments)
		{
			previousCommands.Add(command);

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
					AddToLogText(message);
					break;
				case "physedit":
					GameServices.PhysicsSettingsEditorForm.Show();
					break;
				case "colldebug":
					if (!GameServices.CollisionDebuggerActive)
					{
						// Activate the collision debugger
						GameServices.CollisionDebuggerActive = true;
						GameServices.CollisionDebuggerForm = new CollisionDebuggerForm();
						GameServices.CollisionDebuggerForm.Show();
					}
					else
					{
						// Disable the collision debugger
						GameServices.CollisionDebuggerActive = false;
						GameServices.CollisionDebuggerForm.Close();
						GameServices.CollisionDebuggerForm = null;
					}
					break;
				default:
					AddToLogText(string.Format("The command \"{0}\" does not exist.", command));
					break;
			}
		}

		private void SeekCommand(Direction direction)
		{
			if (previousCommands.Count == 0)
			{
				// There are no submitted commands.
				return;
			}

			if (direction == Direction.Down)
			{
				if (displayedCommandNumber == previousCommands.Count - 1)
				{
					// We're already at the bottom of the submitted commands.
					return;
				}
				else
				{
					// Seek to the next command.
					displayedCommandNumber++;
					TextCommand.Text = previousCommands[displayedCommandNumber];
					TextCommand.SelectionStart = 0;
				}
			}
			else if (direction == Direction.Up)
			{
				if (displayedCommandNumber == 0)
				{
					// We're already the the top of the submitted commands.
					return;
				}
				else if (displayedCommandNumber == -1)
				{
					// In the default case, we want to seek to whatever the last
					// command was.
					displayedCommandNumber = previousCommands.Count - 1;
					TextCommand.Text = previousCommands[displayedCommandNumber];
					TextCommand.SelectionStart = 0;
				}
				else
				{
					// Seek to the previous command.
					displayedCommandNumber--;
					TextCommand.Text = previousCommands[displayedCommandNumber];
					TextCommand.SelectionStart = 0;
				}
			}
			else
			{
				throw new ArgumentException(string.Format("DebugForm.SeekCommand(Direction): You can only seek up or seek down through commands, not {0}.", direction));
			}
		}

		private void SubmitCommand()
		{
			string fullCommand = TextCommand.Text;
			string[] commandWords = fullCommand.Split(' ');
			string command = commandWords[0];
			var arguments = commandWords.Skip(1);
			TextCommand.Text = "";

			if (!string.IsNullOrEmpty(command))
			{
				AddToLogText(command);
				ProcessCommand(command, arguments);
			}
		}

		private void TextCommand_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up)
			{
				SeekCommand(Direction.Up);
			}
			else if (e.KeyCode == Keys.Down)
			{
				SeekCommand(Direction.Down);
			}
			else if (e.KeyCode == Keys.Enter)
			{
				SubmitCommand();
			}
			else
			{
				// Reset the currently displayed command number in case the user
				// edits whatever's in the box
				displayedCommandNumber = -1;
			}
		}
	}
}
