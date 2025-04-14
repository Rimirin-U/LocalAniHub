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
            int playerType;
            try
            {
                playerType = int.Parse(GlobalSettingsService.Instance.GetValue("playerType"));
            }
            catch (Exception)
            {
                throw new ArgumentException("VideoPlayerService: wrong playerType");
            }

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

        private IVideoPlay VideoPlay { get; set; }

        public void Play(string path)
        {
            VideoPlay.Play(path);
        }
    }
}
