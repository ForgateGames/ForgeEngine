using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyItem : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI Name;

    [SerializeField]
    public Sprite DirectorySprite;

    [SerializeField]
    public Sprite FileSprite;

    [SerializeField]
    public RawImage Icon;

    public void SetName(string name, HierarchyType type)
    {
        Name.text = name;

        switch (type)
        {
            case HierarchyType.File:
                Icon.texture = FileSprite.texture;
                break;
            case HierarchyType.Directory:
                Icon.texture = DirectorySprite.texture;
                break;
            case HierarchyType.Scene:
                break;
            default:
                break;
        }
    }
}
