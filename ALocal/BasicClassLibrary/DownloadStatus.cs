using MonoTorrent.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class DownloadStatus
    {
        public DownloadStatus(double progress, string speed, TorrentState torrentState, int peersAvailable)
        {
            Progress = progress;
            Speed = speed;
            TorrentState = torrentState;
            PeersAvailable = peersAvailable;
        }

        public double Progress { get; set; }
        public string Speed { get; set; }
        public TorrentState TorrentState { get; set; }
        public int PeersAvailable { get; set; }
    }
}
