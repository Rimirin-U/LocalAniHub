using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    // 条目服务实现
    public class EntryService : IEntryService
    {
        //定义为readonly,防止被意外修改
        private readonly IEntryFetcher fetcher;
        private readonly IEntryManager manager;
        private readonly IEpisodeManager episodeManager;
        //条目服务的构造函数
        public EntryService(
            IEntryFetcher fetcher1,
            IEntryManager manager1,
            IEpisodeManager episodeManager1)
        {
            fetcher = fetcher1;
            manager = manager1;
            episodeManager = episodeManager1;
        }
        public IEntryFetcher Fetcher
        {
            get
            {
                return fetcher;
            }
        }
        public IEntryManager Manager
        {
            get
            {
                return manager;
            }
        }
        public IEpisodeManager EpisodeManager
        {
            get
            {
                return episodeManager;
            }
        }

        public void AddEntryWithEpisodes(Entry entry, List<Episode> episodes)
        {
            manager.AddEntry(entry); // 添加条目
                                     //初始化所有Episode对象
            foreach (var episode in episodes)
            {
                episode.EntryId = entry.Id;
                episodeManager.AddEpisode(episode);
            }
        }
    }

}
