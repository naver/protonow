using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Naver.Compass.Module
{
    public class FileChangedWatcher
    {
        public DateTime lastwrite { get; set; }
        public event FileSystemEventHandler FileChanged;
        public FileChangedWatcher(string path, string filter)
        {
            var watcher = new FileSystemWatcher(path, filter);
            watcher.Changed += watcher_Changed;
            //watcher.EnableRaisingEvents = true;
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                var lastWriteTime = File.GetLastWriteTime(e.FullPath);
                if ((lastWriteTime - lastwrite).Milliseconds > 50)
                {
                    lastwrite = lastWriteTime;
                    if (FileChanged != null)
                    {
                        FileChanged(sender, e);
                    }
                }
            }
        }
    }
}
