using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;

namespace BasicClassLibrary
{
    public interface IResourceSearch
    {
        public Task<List<ResourceItem>> SearchAsync(string keyword, CancellationToken ct = default);
        public Task<List<ResourceItem>> SearchMultipleKeywordsAsync(IEnumerable<string> keywords, CancellationToken ct = default);
    }
}
