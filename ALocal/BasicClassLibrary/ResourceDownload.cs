using MonoTorrent.Client;
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
            /* new */
            EngineSettingsBuilder engineSettingsBuilder = new()
            {
                AllowLocalPeerDiscovery = true,  // 启用本地网络发现
                MaximumConnections = 150,        // 增加最大连接数
                DhtEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 6881)
            };
            clientEngine = new(engineSettingsBuilder.ToSettings());
            TrackerManager = new();
        }

        public TrackerManager TrackerManager { get; set; }

        private MonoTorrent.Client.ClientEngine clientEngine;

        // use default download path
        public async Task<MagnetDownloadManager> GetMagnetDownloadManager(string magnetUrl)
        {
            MagnetDownloadManager manager = new(magnetUrl,
                GlobalSettingsService.Instance.GetValue("downloadPath"),
                clientEngine, TrackerManager);
            await manager.Initialize();
            return manager;
        }

        // not use default download path
        public async Task<MagnetDownloadManager> GetMagnetDownloadManager(string magnetUrl,string downloadPath)
        {
            MagnetDownloadManager manager = new(magnetUrl,downloadPath, clientEngine, TrackerManager);
            await manager.Initialize();
            return manager;
        }
    }
}
