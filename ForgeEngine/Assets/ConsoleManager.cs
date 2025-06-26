using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class ConsoleManager : MonoBehaviour
{
    [SerializeField]
    public GameObject ConsoleLinePrefab;

    [SerializeField]
    public GameObject ConsoleList;

    [SerializeField]
    public GameObject Loading;

    public bool IsFinishExecution { get; set; }
    public static ConcurrentQueue<string> OutputDataReceived { get; set; } = new ConcurrentQueue<string>();
    void Start()
    {
        Invoke(nameof(VerifyOutputDataReceived), 1);
    }

    private void VerifyOutputDataReceived()
    {
        if (OutputDataReceived.TryDequeue(out var output))
        {
            AddNewLine(output);
        }
        Invoke(nameof(VerifyOutputDataReceived), 1);
    }

    private void FixedUpdate()
    {
        if (IsFinishExecution &&
            !EngineManager.IsRunning)
        {
            Loading.SetActive(false);
            IsFinishExecution = false;
            OutputDataReceived.Enqueue("Finished.");
        }
    }
    public void AddNewLine(string text)
    {
        var line = Instantiate(ConsoleLinePrefab, transform.position, Quaternion.Euler(1,1,0), ConsoleList.transform);
        var time = DateTime.Now.ToString("HH:mm:ss");
        line.GetComponent<TextMeshProUGUI>().text = $"{time} {text}";
    }
    public void ExecuteCommand(string command, bool waitForExit = false)
    {
        UnityEngine.Debug.Log($"ExecuteCommand: {command}");
        ProcessStartInfo processInfo = new ProcessStartInfo();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        processInfo.FileName = "cmd.exe";
        processInfo.Arguments = $"/C {command}";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_EDITOR_LINUX
        processInfo.FileName = "/bin/bash";
        processInfo.Arguments = $"-c \"{command}\"";
#endif

        processInfo.RedirectStandardOutput = true;
        processInfo.RedirectStandardError = true;
        processInfo.UseShellExecute = false;
        processInfo.CreateNoWindow = true;
        if (EngineManager.ProjectDirectory != null)
            processInfo.WorkingDirectory = EngineManager.ProjectDirectory;

        Process process = new Process();
        process.StartInfo = processInfo;
        process.OutputDataReceived += (sender, args) =>
        {
            if (args != null &&
                args.Data != null)
                OutputDataReceived.Enqueue(args.Data);
        };
        process.ErrorDataReceived += (sender, args) => 
        {
            if (EngineManager.IsRunning)
            {
                EngineManager.IsRunning = false;
                IsFinishExecution = true;
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        //if (waitForExit)
        //    process.WaitForExit();
    }
    public void CompileAndRun()
    {
        OutputDataReceived.Enqueue("Compile and run...");
        Loading.SetActive(true);
        EngineManager.IsRunning = true;
        IsFinishExecution = false;
        ExecuteCommand("dotnet run", true);
    }
}
