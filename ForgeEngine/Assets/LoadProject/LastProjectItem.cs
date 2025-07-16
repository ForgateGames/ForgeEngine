using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.LoadProject
{
    [ShowOdinSerializedPropertiesInInspector]
    public class LastProjectItem : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public TextMeshProUGUI Name { get; set; }
        [OdinSerialize]
        public TextMeshProUGUI Folder { get; set; }

        public ForgeProject ForgeProject { get; set; }
        public UnityEvent<ForgeProject> OnClickPlay { get; set; } = new UnityEvent<ForgeProject>();

        public void SetInfo(ForgeProject project)
        {
            ForgeProject = project;
            Name.text = project.Name;
            Folder.text = project.FolderPath;
        }
        public void ClickPlay()
        {
            OnClickPlay.Invoke(ForgeProject);
        }
    }
}
