using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMLimitless.Debug
{
	public static class Logger
	{
		private static List<string> loggedLines;
		// private static DebugForm debugForm;

		public static bool LoggingEnabled { get; set; }
		public static bool DisplayLogsInDebugForm { get; set; }

		static Logger()
		{
			loggedLines = new List<string>();

			LoggingEnabled = true;

			string logFolder = Path.Combine(Directory.GetCurrentDirectory(), "logs");

			if (!Directory.Exists(logFolder))
			{
				Directory.CreateDirectory(logFolder);
			}
		}

		public static void Log(LogLevel level, string message)
		{
			if (LoggingEnabled)
			{
				string now = DateTime.Now.ToString("HH:mm:ss");
				string prepend = (level == LogLevel.Information) ? "[INFO]" : (level == LogLevel.Warning) ? "[WARN]" : "[ERROR]";

				loggedLines.Add(string.Concat("[", now, "] ", prepend, " ", message));
			}
		}

		public static void LogInfo(string message)
		{
			Log(LogLevel.Information, message);
		}

		public static void LogWarning(string message)
		{
			Log(LogLevel.Warning, message);
		}

		public static void LogError(string message)
		{
			Log(LogLevel.Error, message);
		}

		public static void SaveToLogFile()
		{
			if (LoggingEnabled)
			{
				string fileName = string.Concat("applog_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"), ".txt");
				string filePath = Path.Combine(Directory.GetCurrentDirectory(), "logs", fileName);

				File.WriteAllLines(filePath, loggedLines);
			}
			else
			{
				throw new InvalidOperationException("Logger.SaveToLogFile(): Cannot save the log file as logging is disabled.");
			}
		}
	}
}
