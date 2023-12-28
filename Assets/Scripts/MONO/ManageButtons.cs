#if UNITY_EDITOR


using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



// ---------------------------------------- //
//              SCRIPT DI DEBUG
// ---------------------------------------- //



[ExecuteInEditMode]
public class ManageButtons : MonoBehaviour
{
    Transform Hotbar => transform.Find("[PNL] Hotbar Area");
    Transform Panel => transform.Find("[PNL] Inventory Area");

    public GameObject inventorySlotPrefab;



    public bool RefindButtons;
    public bool RefindScripts;
    public bool RefindImages;

    public List<ButtonBehaviour> scripts = new();
    public List<Button> buttons = new();
    public List<Image> images = new();

    public bool doSomethingOnIcons;







    void FindButtons ()
    {
        RefindButtons = false;
        buttons.Clear();
        foreach (Transform child in Panel) buttons.Add(child.GetChild(0).GetComponent<Button>());
        foreach (Transform child in Hotbar) buttons.Add(child.GetChild(0).GetComponent<Button>());
    }
    void FindScript()
    {
        RefindScripts = false;
        scripts.Clear();
        foreach (Transform child in Panel) scripts.Add(child.GetChild(0).GetComponent<ButtonBehaviour>());
        GameObject.Find("INVENTORY").GetComponent<Inventory>().inventorySlots = scripts.ToArray();
        foreach (Transform child in Hotbar) scripts.Add(child.GetChild(0).GetComponent<ButtonBehaviour>());
    }
    void FindImages()
    {
        RefindImages = false;
        images.Clear();
        foreach (var item in buttons) images.Add(item.transform.GetChild(0).GetComponent<Image>());
    }
    void DoSomethingOnIcons()
    {
        doSomethingOnIcons = false;
        foreach (var item in images)
        {
            Transform panel = item.transform.parent;
            if (panel.childCount == 3)
            {
                DestroyImmediate(panel.GetChild(1).gameObject);
                panel.GetChild(1).gameObject.SetActive(false);
            }

        }
    }


    private void Awake () { FindButtons(); FindScript(); FindImages(); }
    private void Update ()
    {
        if (doSomethingOnIcons) DoSomethingOnIcons();
        if (RefindButtons) FindButtons();
        if (RefindScripts) FindScript();
        if (RefindImages) FindImages();



        if (Application.isPlaying == false)
        {
            if (Inventory.INVENTORY_SIZE != Panel.childCount)
            {
                if (Inventory.INVENTORY_SIZE > Panel.childCount)
                {
                    for (int i = Panel.childCount; i < Inventory.INVENTORY_SIZE; i++)
                    {
                        GameObject slot = Instantiate(inventorySlotPrefab, Panel);
                        // write name with 2 cypher
                        slot.name = "Slot " + (i + 1).ToString(Inventory.INVENTORY_SIZE > 99 ? "000" : "00");
                    }
                }
                else if (Inventory.INVENTORY_SIZE < transform.childCount)
                {
                    for (int i = Panel.childCount - 1; i >= Inventory.INVENTORY_SIZE; i--)
                    {
                        if (Application.isPlaying) Destroy(Panel.GetChild(i).gameObject);
                        else DestroyImmediate(Panel.GetChild(i).gameObject);
                    }
                }
            }
        }
    }
}
#endif