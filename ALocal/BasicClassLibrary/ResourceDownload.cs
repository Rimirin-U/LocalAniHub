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

        // use default download path
        public MagnetDownloadManager GetMagnetDownloadManager(string magnetUrl)
        {
            MagnetDownloadManager manager = new(magnetUrl,
                GlobalSettingsService.Instance.GetValue("downloadPath"),
                clientEngine, TrackerManager);
            return manager;
        }

        // not use default download path
        public MagnetDownloadManager GetMagnetDownloadManager(string magnetUrl,string downloadPath)
        {
            MagnetDownloadManager manager = new(magnetUrl,downloadPath, clientEngine, TrackerManager);
            return manager;
        }
    }
}
