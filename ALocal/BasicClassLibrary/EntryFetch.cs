using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class EntryFetch
    {
        public EntryFetch()
        {
            int sourceType;
            try
            {
                sourceType = int.Parse(GlobalSettingsService.Instance.GetValue("defaultEntryFetchSource"));
            }
            catch (Exception)
            {
                throw new ArgumentException("EntryFetch: wrong sourceType");
            }

            switch (sourceType)
            {
                case 0:
                    entryFetch = new EntryFetchInYucWiki();
                    break;
                default:
                    entryFetch = new EntryFetchInYucWiki();
                    break;
            }
        }
        public EntryFetch(int sourceType)
        {
            switch (sourceType)
            {
                case 0:
                    entryFetch = new EntryFetchInYucWiki();
                    break;
                default:
                    entryFetch = new EntryFetchInYucWiki();
                    break;
            }
        }

        private IEntryFetch entryFetch;

        public async Task<List<EntryInfoSet>> FetchAsync()
        {
            return await entryFetch.FetchAsync();
        }
    }
}
