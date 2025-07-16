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
    [OdinSerialize]
    public ButtonManager OpenFromGit;

    [OdinSerialize]
    public ButtonManager ChatGpt { get; set; }
    [OdinSerialize]
    public ButtonManager Claude { get; set; }
    [OdinSerialize]
    public ButtonManager Gemini { get; set; }
    [OdinSerialize]
    public ButtonManager Copilot { get; set; }
    [OdinSerialize]
    public UnityEvent OnProjectValidated { get; set; }

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
    private void OpenProject(ForgeProject forgeProject)
    {
        ProjectName = forgeProject.Name;
        FolderPath = forgeProject.FolderPath;
        OpenProject();
    }
    private void OpenProject()
    {
        var project = new ForgeProject
        {
            Name = ProjectName,
            FolderPath = FolderPath
        };
        EngineManager.ProjectFolder = FolderPath;

        //validate project before opening

        //change view
        OnProjectValidated?.Invoke();
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


    public async void NewProject()
    {
        await ConsoleManager.ExecuteCommand($"dotnet new mgdesktopgl -o {ProjectName}", true);
        ConsoleManager.EnterInFolder(ProjectName);
        await ConsoleManager.ExecuteCommand($"mkdir ForgeEngineScenes", true);
        ConsoleManager.ExitFromFolder();
        
        SaveLastProjects();
        OpenProject();
    }

    void AddToLastProjectsListIfNotExist(ForgeProject forgeProject)
    {
        if (LastProjects.ProjectsList.Exists(p => p.Name == forgeProject.Name && p.FolderPath == forgeProject.FolderPath))
            return;
        LastProjects.ProjectsList.Add(forgeProject);
        SaveLastProjects();
        var item = Instantiate(LastProjectItem, LastProjectsList.transform);
        item.GetComponent<LastProjectItem>().SetInfo(forgeProject);
        item.GetComponent<LastProjectItem>().OnClickPlay.AddListener(OpenProject);
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
