using UnityEngine;

[CreateAssetMenu(fileName = "EditorScriptMemory", menuName = "Scriptable Objects/EditorScriptMemory", order = 1)]
public class EditorScriptMemory : ScriptableObject
{
    public Inventory inventory;

    public GameObject hotbar;
}
