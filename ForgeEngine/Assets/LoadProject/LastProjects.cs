using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.LoadProject
{
    [System.Serializable]
    public class LastProjects
    {
        [OdinSerialize]
        public List<ForgeProject> ProjectsList { get; set; }
    }
}
