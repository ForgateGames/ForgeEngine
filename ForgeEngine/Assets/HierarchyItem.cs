using TMPro;
using UnityEngine;

public class HierarchyItem : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI Name;

    public void SetName(string name)
    {
        Name.text = name;
    }
}
