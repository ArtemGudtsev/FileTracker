using System;
using System.Collections.Generic;
using System.Text;

namespace FileTracker
{
    public class ConfigModel
    {
        public string PathToTool { get; set; }
        public string FolderToBeTracked { get; set; }
        public string FullPathToLogFile { get; set; }
        public int SizeOfLogDump { get; set; }
    }
}
