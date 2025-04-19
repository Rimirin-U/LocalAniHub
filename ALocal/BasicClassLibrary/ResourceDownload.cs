using MonoTorrent.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class ResourceDownload
    {
        public ResourceDownload()
        {
            clientEngine = new();
            TrackerManager = new();
        }

        public TrackerManager TrackerManager { get; set; }

        private MonoTorrent.Client.ClientEngine clientEngine;

        public MagnetDownloadManager GetMagnetDownloadManager(string magnetUrl)
        {
            MagnetDownloadManager manager = new(magnetUrl, clientEngine, TrackerManager);
            manager.Initialize();
            return manager;
        }
    }
}
