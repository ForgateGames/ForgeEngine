using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EngineManager : MonoBehaviour
{
    public static string ProjectFolder { get; set; }
    public static bool IsProjectLoaded { get; set; }
    public static bool IsRunning { get; set; }

    void Start()
    {
    }
}
