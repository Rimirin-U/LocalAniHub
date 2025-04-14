using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class VideoPlayService
    {
        public VideoPlayService()
        {
            // get player type
            int playerType;
            try
            {
                playerType = int.Parse(GlobalSettingsService.Instance.GetValue("defaultPlayerType"));
            }
            catch (Exception)
            {
                throw new ArgumentException("VideoPlayerService: wrong playerType");
            }

            // get player path
            this.playerPath = GlobalSettingsService.Instance
                                .GetValue("defaultPlayerPath");
            if (string.IsNullOrEmpty(playerPath)) throw new ArgumentException("VideoPlayService: wrong player path");

            switch (playerType)
            {
                case 0:
                    VideoPlay = new VideoPlayWithPotPlayer();
                    break;
                default:
                    VideoPlay = new VideoPlayWithPotPlayer();
                    break;
            }
        }
        public VideoPlayService(int playerType,string playerPath)
        {
            if(string.IsNullOrEmpty(playerPath)) throw new ArgumentException("VideoPlayService: wrong player path");
            this.playerPath = playerPath;
            switch (playerType)
            {
                case 0:
                    VideoPlay = new VideoPlayWithPotPlayer();
                    break;
                default:
                    VideoPlay = new VideoPlayWithPotPlayer();
                    break;
            }
        }

        private IVideoPlay VideoPlay;
        private string playerPath;

        public void Play(string videoPath)
        {
            VideoPlay.Play(videoPath, playerPath);
        }
    }
}
