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
        private readonly EntryRatingManager entryRatingManager;//评分管理
        private readonly EntryMatadataManager entryMatadataManager;//播出时间管理
        //条目服务的构造函数
        public EntryService(
            EntryFetch fetch,
            EntryManager manager,
            EpisodeManager episodeManager,
            EntryRatingManager entryRatingManager,
            EntryMatadataManager entryMatadataManager)
        {
            this.fetch = fetch;
            this.manager = manager;
            this.episodeManager = episodeManager;
            this.entryRatingManager = entryRatingManager;
            this.entryMatadataManager = entryMatadataManager;
        }
        public EntryFetch Fetch
        {
            get
            {
                return fetch;
            }
        }
        public EntryManager EntryManager
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
        public EntryRatingManager EntryRatingManager
        {
            get
            {
                return entryRatingManager;
            }
        }
        public EntryMatadataManager EntryMatadataManager
        {
            get
            {
                return entryMatadataManager;
            }
        }
        public void AddEntryWithEpisodes(Entry entry)
        {
            manager.Add(entry); // 添加主条目

            // 自动生成并关联EpisodeCount个单集
            for (int i = 1; i <= entry.EpisodeCount; i++) 
            {
                var episode = new Episode
                {
                    EntryId = entry.Id,//关联到当前条目
                    EpisodeNumber = i//集数
                };

                episodeManager.Add(episode); // 添加单集
            }
        }
    }

}
