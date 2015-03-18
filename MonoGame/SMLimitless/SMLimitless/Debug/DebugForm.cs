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
	public partial class DebugForm : Form
	{
		private const int MaximumDisplayedLines = 200;

		private int displayedLines = 0;

		public DebugForm()
		{
			InitializeComponent();
			// TODO: add a log of old commands that the user can press Up or Down to view
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

			this.TextLog.AppendText(text);
		}

		private void ButtonSubmit_Click(object sender, EventArgs e)
		{
			string fullCommand = this.TextCommand.Text;
			string[] commandWords = fullCommand.Split(' ');
			string command = commandWords[0];
			var arguments = commandWords.Skip(1);
			this.TextLog.Text = "";

			if (!string.IsNullOrEmpty(command))
			{
				this.AddToLogText(command);
				this.ProcessCommand(command, arguments);
			}
		}

		private void ProcessCommand(string command, IEnumerable<string> arguments)
		{
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
				default:
					this.AddToLogText(string.Format("The command \"{0}\" does not exist.", command));
					break;
			}
		}
	}
}
