using Assets.Console;
using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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

    [SerializeField]
    public ButtonManager CompileAndRunButtom;

    public static bool IsGameFinishExecution { get; set; }
    static ConcurrentQueue<ConsoleMessage> OutputDataReceived { get; set; } = new ConcurrentQueue<ConsoleMessage>();
    void Start()
    {
        Invoke(nameof(VerifyOutputDataReceived), 0.3f);
        CompileAndRunButtom.Interactable(false);
    }

    private void VerifyOutputDataReceived()
    {
        if (OutputDataReceived.TryDequeue(out var output))
        {
            AddNewLine(output);
        }
        Invoke(nameof(VerifyOutputDataReceived), 0.3f);
    }

    private void FixedUpdate()
    {
        if (IsGameFinishExecution &&
            !EngineManager.IsRunning)
        {
            Loading.SetActive(false);
            IsGameFinishExecution = false;
            SendMsg("Finished.");
        }
    }
    public static void SendMsg(string text)
    {
        var message = new ConsoleMessage
        {
            Message = text,
            Time = DateTime.Now,
        };
        OutputDataReceived.Enqueue(message);
    }
    public void AddNewLine(ConsoleMessage message)
    {
        var line = Instantiate(ConsoleLinePrefab, transform.position, Quaternion.Euler(1,1,0), ConsoleList.transform);
        var time = message.Time.ToString("HH:mm:ss");
        line.GetComponent<TextMeshProUGUI>().text = $"{time} {message.Message}";
    }
    public static void EnterInFolder(string folder)
    {
        EngineManager.ProjectFolder += $"\\{folder}";
    }
    public static void ExitFromFolder()
    {
        EngineManager.ProjectFolder = Path.GetDirectoryName(EngineManager.ProjectFolder);
    }
    public static async Awaitable ExecuteCommand(string command, bool waitForExit = false)
    {
        UnityEngine.Debug.Log($"ExecuteCommand: {command}");
        var processInfo = new ProcessStartInfo();

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
        if (EngineManager.ProjectFolder != null)
            processInfo.WorkingDirectory = EngineManager.ProjectFolder;

        Process process = new Process();
        process.StartInfo = processInfo;
        process.OutputDataReceived += (sender, args) =>
        {
            if (args != null &&
                args.Data != null)
                SendMsg(args.Data);
        };
        process.ErrorDataReceived += (sender, args) => 
        {
            if (EngineManager.IsRunning)
            {
                EngineManager.IsRunning = false;
                IsGameFinishExecution = true;
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (waitForExit)
        {
            while(!process.HasExited)
            {
                await Awaitable.FixedUpdateAsync();
            }
        }
    }

    private static void Process_Exited(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    public void CompileAndRun()
    {
        SendMsg("Compile and run...");
        Loading.SetActive(true);
        EngineManager.IsRunning = true;
        IsGameFinishExecution = false;
        ExecuteCommand("dotnet run", true);
    }
}
