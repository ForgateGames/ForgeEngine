using Assets.LoadProject;
using Michsky.MUIP;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

[ShowOdinSerializedPropertiesInInspector]
public class LoadProjectManager : SerializedMonoBehaviour
{
    [SerializeField]
    public ButtonManager OpenFromGit;

    [SerializeField]
    public ButtonManager ChatGpt;
    [SerializeField]
    public ButtonManager Claude;
    [SerializeField]
    public ButtonManager Gemini;
    [SerializeField]
    public ButtonManager Copilot;
    [SerializeField]
    public UnityEvent<string> OnFolderSelected;
    [SerializeField]
    public UnityEvent OnProjectCreated;

    [OdinSerialize]
    public GameObject LastProjectsList { get; set; }
    [OdinSerialize]
    public GameObject LastProjectItem { get; set; }

    public string SaveFileName { get; set; } = "ForgeEngineSettings.json";

    public string FolderPath { get; set; }
    public string ProjectName { get; set; }
    [OdinSerialize]
    public LastProjects LastProjects { get; set; } = new LastProjects();

    void Start()
    {
        OpenFromGit.Interactable(false);
        ChatGpt.Interactable(false);
        Claude.Interactable(false);
        Gemini.Interactable(false);
        Copilot.Interactable(false);
        ClearLastProjectsList();
        LastProjects.ProjectsList = new List<ForgeProject>();
        LoadLastProjects();
    }

    private void ClearLastProjectsList()
    {
        foreach (Transform item in LastProjectsList.transform)
        {
            Destroy(item.gameObject);
        }
    }

    private void LoadLastProjects()
    {
        var path = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(path))
        {
            var bytes = File.ReadAllBytes(path);
            LastProjects = SerializationUtility.DeserializeValue<LastProjects>(bytes, DataFormat.JSON);
            foreach (var project in LastProjects.ProjectsList)
            {
                var item = Instantiate(LastProjectItem, LastProjectsList.transform);
                item.GetComponent<LastProjectItem>().SetInfo(project);
                item.GetComponent<LastProjectItem>().OnClickPlay.AddListener(OpenProject);
            }
        }else
        {
            SaveLastProjects();
        }
    }

    private void OpenProject(ForgeProject project)
    {
        ProjectName = project.Name;
        FolderPath = project.FolderPath;
        EngineManager.ProjectFolder = FolderPath;
        OnProjectCreated?.Invoke();
    }

    private void SaveLastProjects()
    {
        string path = Path.Combine(Application.persistentDataPath, SaveFileName);
        var bytes = SerializationUtility.SerializeValue(LastProjects, DataFormat.JSON);
        File.WriteAllBytes(path, bytes);
        //string json = UnityEngine.JsonUtility.ToJson(LastProjects, true);
        //File.WriteAllText(path, json);
        Debug.Log($"Project saved: {path}");
    }

    public void SaveProjectInternalList()
    {
        LastProjects.ProjectsList.Add(new ForgeProject
        {
            Name = ProjectName,
            FolderPath = FolderPath
        });
        SaveLastProjects();
    }

    public async void NewProject()
    {
        await ConsoleManager.ExecuteCommand($"dotnet new mgdesktopgl -o {ProjectName}", true);
        ConsoleManager.EnterInFolder(ProjectName);
        await ConsoleManager.ExecuteCommand($"mkdir ForgeEngineScenes", true);
        OpenProject(new ForgeProject { FolderPath = FolderPath, Name = ProjectName});
    }

    public void SelectFolder()
    {
        var projectPath = EditorUtility.OpenFolderPanel("Select a destiny", "", "");
        if (string.IsNullOrEmpty(projectPath))
        {
            ConsoleManager.SendMsg("No folder selected.");
            return;
        }
        FolderPath = projectPath;
        OnFolderSelected?.Invoke(projectPath);
    }
}
