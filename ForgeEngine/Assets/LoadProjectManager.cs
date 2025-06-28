using Michsky.MUIP;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class LoadProjectManager : MonoBehaviour
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

    public string FolderPath { get; set; }
    public string ProjectName { get; set; }


    void Start()
    {
        OpenFromGit.Interactable(false);
        ChatGpt.Interactable(false);
        Claude.Interactable(false);
        Gemini.Interactable(false);
        Copilot.Interactable(false);
    }
    public async void NewProject()
    {
        EngineManager.ProjectFolder = FolderPath;
        await ConsoleManager.ExecuteCommand($"dotnet new mgdesktopgl -o {ProjectName}", true);
        await ConsoleManager.ExecuteCommand($"mkdir ForgeEngineScenes", true);
        OnProjectCreated?.Invoke();
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
