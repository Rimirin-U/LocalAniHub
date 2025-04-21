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
        private readonly EntryMetaDataManager entryMetaDataManager;//条目元数据管理
        private readonly EntryTimeInfoManager entryTimeInfoManager;//条目播出时间
        //条目服务的构造函数
        public EntryService(
            EntryFetch fetch,
            EntryManager manager,
            EpisodeManager episodeManager,
            EntryRatingManager entryRatingManager,
            EntryMetaDataManager entryMetaDataManager,
            EntryTimeInfoManager entryTimeInfoManager)
        {
            this.fetch = fetch;
            this.manager = manager;
            this.episodeManager = episodeManager;
            this.entryRatingManager = entryRatingManager;
            this.entryMetaDataManager = entryMetaDataManager;
            this.entryTimeInfoManager = entryTimeInfoManager;
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
        public EntryMetaDataManager EntryMetaDataManager
        {
            get
            {
                return entryMetaDataManager;
            }
        }
        public EntryTimeInfoManager EntryTimeInfoManager
        {
            get
            {
                return entryTimeInfoManager;
            }
        }
        public void AddEntryWithEpisodes(Entry entry)
        {
            manager.Add(entry); // 添加主条目

            // 自动生成并关联EpisodeCount个单集
            for (int i = 1; i <= entry.EpisodeCount; i++) 
            {
                var episode = new Episode(entry.Id,entry,i)
                {
                    EntryId = entry.Id,//关联到当前条目
                    Entry = entry,
                    EpisodeNumber = i//集数
                };

                episodeManager.Add(episode); // 添加单集
            }
        }
    }

}
