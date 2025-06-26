using Michsky.MUIP;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class HierarchyManager : MonoBehaviour
{
    [SerializeField]
    public ButtonManager OpenProjectButton;

    [SerializeField]
    public UnityEvent ProjectLoaded;

    [SerializeField]
    public GameObject HierarchyItem;

    [SerializeField]
    public GameObject HierarchyList;
    public void OpenProject()
    {
        //using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        //{
        //    dialog.Description = "Selecione uma pasta";
        //    dialog.ShowNewFolderButton = true;

        //    if (dialog.ShowDialog() == DialogResult.OK)
        //    {
        //        string selectedPath = dialog.SelectedPath;
        //        Debug.Log("Pasta escolhida: " + selectedPath);
        //    }
        //}
        //string projectPath = EditorUtility.OpenFolderPanel("Selecione uma pasta", "", "");
        var projectPath = EditorUtility.OpenFilePanel("Selecione um arquivo", "", "cs");
        if (!string.IsNullOrEmpty(projectPath))
        {
            var fileName = Path.GetFileName(projectPath);

            if (fileName == "Program.cs")
            {
                EngineManager.ProjectDirectory = Path.GetDirectoryName(projectPath);

                var files = Directory.GetFiles(EngineManager.ProjectDirectory);

                var directories = Directory.GetDirectories(EngineManager.ProjectDirectory);

                ConsoleManager.OutputDataReceived.Enqueue("Loading project...");

                foreach (var dir in directories)
                {
                    var directoryPath = Path.GetDirectoryName(dir + "\\");
                    var name = Path.GetFileName(directoryPath);
                    CreateHierarchyItem(name);
                }

                foreach (var file in files)
                {
                    var name = Path.GetFileName(file);
                    CreateHierarchyItem(name);
                }

                EngineManager.IsProjectLoaded = true;
                OpenProjectButton.Interactable(false);
                ProjectLoaded?.Invoke();
                ConsoleManager.OutputDataReceived.Enqueue("Project loaded.");
            }
            else
            {
                Debug.LogWarning("Nenhum arquivo Program.cs encontrado.");
            }
        }
    }
    void CreateHierarchyItem(string name)
    {
        var item = Instantiate(HierarchyItem, transform.position, Quaternion.Euler(1, 1, 0), HierarchyList.transform);
        item.GetComponent<HierarchyItem>().SetName(name);
    }
}
