using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.LoadProject
{
    public class ForgeProject
    {
        [OdinSerialize]
        public string Name { get; set; }
        [OdinSerialize]
        public string FolderPath { get; set; }
    }
}
