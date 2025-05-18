using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace BasicClassLibrary
{
    public class ResourceSearch
    {
        
        public ResourceSearch()//构造函数——从默认数据源拉取
        {
            int sourceType;
            try
            {
                sourceType = int.Parse(GlobalSettingsService.Instance.GetValue("defaultResourceSearchSource"));
            }
            catch (Exception)
            {
                throw new ArgumentException("ResourceSearch: invalid sourceType configuration");
            }
            switch (sourceType)
            {
                case 0:
                    resourceSearch = new ResourceSearchInAnimesGarden();
                    break;
                default:
                    resourceSearch = new ResourceSearchInAnimesGarden();
                    break;
            }
        }

        public ResourceSearch(int sourceType)//使用指定数据
        {
            switch (sourceType)
            {
                case 0:
                    resourceSearch = new ResourceSearchInAnimesGarden();
                    break;
                default:
                    resourceSearch = new ResourceSearchInAnimesGarden();
                    break;
            }
        }
        private IResourceSearch resourceSearch;

        public async Task<List<ResourceItem>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be null or empty");

            return await resourceSearch.SearchAsync(keyword);
        }
        public async Task<List<ResourceItem>> SearchMultipleKeywordsAsync(IEnumerable<string> keywords)
        {
            if (keywords == null || !keywords.Any())
                throw new ArgumentException("Keywords cannot be null or empty");

            return await resourceSearch.SearchMultipleKeywordsAsync(keywords);
        }
    }


}