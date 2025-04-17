using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicClassLibrary
{
    public class MaterialService
    {
        private MaterialManager MaterialManager;

        public MaterialService(MaterialManager materialManager)
        {
            this.MaterialManager = materialManager;
        }
    }
}
