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
        public List<SyndicationFeed> Search(string keyWord);
    }
}
