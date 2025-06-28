using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    public GameObject SceneList;

    [SerializeField]
    public GameObject SceneItem;

    public string SceneName { get; set; }

    public void NewScene()
    {
        if (string.IsNullOrEmpty(SceneName))
        {
            ConsoleManager.SendMsg("Scene name cannot be empty.");
            return;
        }
        var sceneItem = Instantiate(SceneItem, SceneList.transform);
        sceneItem.transform.Find("Title").GetComponent<TMPro.TextMeshProUGUI>().text = SceneName;
    }
}
