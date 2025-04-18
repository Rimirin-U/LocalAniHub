using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;

namespace BasicClassLibrary
{
    public class ResourceSearch
    {
        
        public ResourceSearch()
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

        public ResourceSearch(int sourceType)
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

            return await resourceSearch.Search(keyword);
        }
    }


}