using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMLimitless.Debug
{
	/// <summary>
	/// Provides a global method to log debug, warning, and error messages to a form,
	/// or save logs to a file on disk.
	/// </summary>
	public static class Logger
	{
		private static List<string> loggedLines;

		/// <summary>
		/// Gets or sets a value indicating whether logging is enabled.
		/// If logging is disabled, all calls to log methods are ignored.
		/// </summary>
		public static bool LoggingEnabled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether logs should be
		/// displayed in the debug form when sent.
		/// </summary>
		public static bool DisplayLogsInDebugForm { get; set; }

		static Logger()
		{
			loggedLines = new List<string>();

			LoggingEnabled = true;
			DisplayLogsInDebugForm = true;

			string logFolder = Path.Combine(Directory.GetCurrentDirectory(), "logs");

			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}
		}

		/// <summary>
		/// Adds a message to the log and prepends it with the current time and log level.
		/// Additionally, sends the message to the DebugForm if the <see cref="DisplayLogsInDebugForm"/> property is set.
		/// </summary>
		/// <param name="level">The level of message (Information, Warning, Error) at which this log message is.</param>
		/// <param name="message">The message to log.</param>
		public static void Log(LogLevel level, string message)
		{
			if (LoggingEnabled)
			{
				string now = DateTime.Now.ToString("HH:mm:ss");
				string prepend = (level == LogLevel.Information) ? "[INFO]" : (level == LogLevel.Warning) ? "[WARN]" : "[ERROR]";
				string completeMessage = string.Concat("[", now, "]", prepend, " ", message);

				loggedLines.Add(completeMessage);

				if (DisplayLogsInDebugForm)
				{
					GameServices.DebugForm.AddToLogText(completeMessage);
				}
			}
		}

		/// <summary>
		/// Logs an informational message.
		/// </summary>
		/// <param name="message">The informational message to log.</param>
		public static void LogInfo(string message)
		{
			Log(LogLevel.Information, message);
		}

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The warning message to log.</param>
		public static void LogWarning(string message)
		{
			Log(LogLevel.Warning, message);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The error message to log.</param>
		public static void LogError(string message)
		{
			Log(LogLevel.Error, message);
		}

		/// <summary>
		/// Saves the log to disk in a file in the program's logs directory.
		/// </summary>
		/// <remarks>The file will be located at {program path}/logs/applog_yyyyMMdd_HHmmss.txt.</remarks>
		public static void SaveToLogFile()
		{
			if (LoggingEnabled)
			{
				string fileName = string.Concat("applog_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt");
				string filePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", fileName);	// TODO: move this folder somewhere better, like AppData

				File.WriteAllLines(filePath, loggedLines);
			}
			else
			{
				throw new InvalidOperationException("Logger.SaveToLogFile(): Cannot save the log file as logging is disabled.");
			}
		}
	}
}
