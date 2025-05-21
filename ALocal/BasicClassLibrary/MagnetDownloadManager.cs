using MonoTorrent.Client;
using MonoTorrent.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class MagnetDownloadManager
    {
        // initialize
        public MagnetDownloadManager(string magnetUrl,string savePath, ClientEngine clientEngine, TrackerManager trackerManager)
        {
            // SavePath = GlobalSettingsService.Instance.GetValue("downloadPath");
            SavePath = savePath;
            Directory.CreateDirectory(SavePath);

            this.clientEngine = clientEngine;
            this.trackerManager = trackerManager;
            MagnetUrl = magnetUrl;
            manager = null;
            DownloadStatus = new(0, "", TorrentState.Starting, 0);
        }

        public string MagnetUrl { get; init; }
        public string SavePath { get; init; }

        private readonly ClientEngine clientEngine;
        private readonly TrackerManager trackerManager;
        private TorrentManager? manager;

        public async Task Initialize()
        {
            // magnet link parse
            MonoTorrent.MagnetLink magnetLink = MonoTorrent.MagnetLink.Parse(MagnetUrl);

            // initialize manager
            manager = await clientEngine.AddAsync(magnetLink, SavePath);

            // add trackers
            foreach (Tracker tracker in trackerManager.Query(TrackerManager.All))
            {
                await manager.TrackerManager.AddTrackerAsync(new Uri(tracker.Url));
            }
        }

        public DownloadStatus DownloadStatus { get; set; }

        public void UpdateDownloadStatus()
        {
            if (manager == null) throw new Exception("MagnetDownloadProcedure: not initialized");
            DownloadStatus = new DownloadStatus(
                manager.Progress,
                Tools.FormatSpeed(manager.Monitor.DownloadRate),
                manager.State,
                manager.Peers.Available
                );
        }

        public string GetTitle()
        {
            if (manager == null) throw new Exception("MagnetDownloadProcedure: not initialized");
            return manager.Name;
        }

        public async Task Start()
        {
            if (manager == null) throw new Exception("MagnetDownloadProcedure: not initialized");
            await manager.StartAsync();
            await manager.WaitForMetadataAsync();
        }
        public async Task Pause()
        {
            if (manager == null) throw new Exception("MagnetDownloadProcedure: not initialized");
            await manager.PauseAsync();
        }
        public async Task Stop()
        {
            if (manager == null) throw new Exception("MagnetDownloadProcedure: not initialized");
            await manager.StopAsync();
        }
    }

    internal static class Tools
    {
        public static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int suffixIndex = 0;
            double size = bytes;

            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:0.##} {suffixes[suffixIndex]}";
        }

        public static string FormatSpeed(long bytesPerSecond)
        {
            return $"{FormatBytes(bytesPerSecond)}";
        }
    }
}
