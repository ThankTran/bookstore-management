using System;

namespace bookstore_Management.Core.Utils
{
    internal static class Logger
    {
        public static void Info(string message) => Write("INFO", message);
        public static void Error(string message) => Write("ERROR", message);

        private static void Write(string level, string message)
        {
            try
            {
                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
                System.Diagnostics.Debug.WriteLine(line);
            }
            catch
            {
                // ignore logging failure
            }
        }
    }
}
