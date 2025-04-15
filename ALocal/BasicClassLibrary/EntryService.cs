using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    // 条目服务实现
    public class EntryService
    {
        //定义为readonly,防止被意外修改
        private readonly EntryFetch fetch;//条目拉取
        private readonly EntryManager manager;//条目管理
        private readonly EpisodeManager episodeManager;//单集管理
        //条目服务的构造函数
        public EntryService(
            EntryFetch fetch1,
            EntryManager manager1,
            EpisodeManager episodeManager1)
        {
            fetch = fetch1;
            manager = manager1;
            episodeManager = episodeManager1;
        }
        public EntryFetch Fetch
        {
            get
            {
                return fetch;
            }
        }
        public EntryManager Manager
        {
            get
            {
                return manager;
            }
        }
        public EpisodeManager EpisodeManager
        {
            get
            {
                return episodeManager;
            }
        }
        public void AddEntryWithEpisodes(Entry entry)
        {
            manager.AddEntry(entry); // 添加主条目

            // 自动生成并关联EpisodeCount个单集
            for (int i = 1; i <= entry.EpisodeCount; i++) 
            {
                var episode = new Episode
                {
                    EntryId = entry.Id,//关联到当前条目
                    EpisodeNumber = i//集数
                };

                episodeManager.AddEpisode(episode); // 添加单集
            }
        }
    }

}
