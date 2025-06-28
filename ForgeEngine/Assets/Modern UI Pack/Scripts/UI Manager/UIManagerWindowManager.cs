using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Michsky.MUIP
{
    public class UIManagerWindowManager : MonoBehaviour
    {
        [SerializeField]
        public ButtonManager LoadProjectButtom;
        [SerializeField]
        public ButtonManager Scene;
        [SerializeField]
        public ButtonManager Objects;
        [SerializeField]
        public ButtonManager Components;

        private void Start()
        {
            Scene.Interactable(false);
            Objects.Interactable(false);
            Components.Interactable(false);
        }

        public void LoadProject()
        {
            LoadProjectButtom.SetText("Close Project");
            Scene.Interactable(true);
            Objects.Interactable(true);
            Components.Interactable(true);
        }
    }
}