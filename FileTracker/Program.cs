using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileTracker
{
    //unstable draft version for internal usage on dev env
    class Program
    {
        private static ConfigModel _config;
        private static List<string> _logBuffer;

        static void Main(string[] args)
        {
            _config = new ConfigModel();

            var p = new OptionSet()
            {
                { "target=", v => _config.FolderToBeTracked = v },
                { "log=", v => _config.FullPathToLogFile = v },
                { "dumpsize=", v => _config.SizeOfLogDump = int.Parse(v) }
            };

            _logBuffer = new List<string>();
            FileSystemWatcher watcher = new FileSystemWatcher(_config.FolderToBeTracked);

            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += Watcher_Renamed;

            watcher.EnableRaisingEvents = true;
            Log("Press any key to stop tracker (necessary to write log file down).");
            Console.ReadKey();
            watcher.EnableRaisingEvents = false;
            Log("Exit command received, tool will be closed right now");

            File.AppendAllLines(_config.FullPathToLogFile, _logBuffer);
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string whatHappens = $"Renamed";
            string newFilePath = e.FullPath;
            string oldFullPath = e.OldFullPath;
            string logMsg = CreateMsg(whatHappens, $"{oldFullPath} -> {newFilePath}");

            Log(logMsg);
        }

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string whatHappens = $"Deleted";
            string filePath = e.FullPath;
            string logMsg = CreateMsg(whatHappens, filePath);

            Log(logMsg);
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string whatHappens = $"Created";
            string filePath = e.FullPath;
            string logMsg = CreateMsg(whatHappens, filePath);

            Log(logMsg);
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string whatHappens = $"Changed";
            string filePath = e.FullPath;
            string logMsg = CreateMsg(whatHappens, filePath);

            Log(logMsg);
        }

        public static string CreateMsg(string whatHappens, string path)
        {
            return $"[{whatHappens}] [{DateTime.Now:dd.MM.yyyy - hh:mm:ss.ff}] {path}";
        }

        public static void Log(string msg)
        {
            Console.WriteLine(msg);

            SaveToFile(msg);
        }

        public static void SaveToFile(string msg)
        {
            _logBuffer.Add(msg);

            if (_logBuffer.Count >= _config.SizeOfLogDump)
            {
                lock (_logBuffer)
                {
                    File.AppendAllLines(_config.FullPathToLogFile, _logBuffer);
                    _logBuffer.Clear();
                }
            }
        }
    }
}
