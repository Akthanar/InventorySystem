using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class Inventory : MonoBehaviour
{
    #region SINGLETON
    void Awake () => instance = this;
    public static Inventory instance;
    #endregion


    [Header("---------------- HOTBAR SETTINGS ----------------------")]
    [Space(10)]

#region HOTBAR

    /// <summary> Indica se la hotbar è abilitata. </summary>
    public bool enableHotbarInInventory = true;
    
    /// <summary> Array di slot della hotbar. </summary>
    public GameObject[] hotbarSlots;

    public const byte HOTBAR_SIZE = 10;
    
    #endregion

    [Space(20)]

    [Header("---------------- INV. SLOT SETTINGS --------------------")]
    [Space(10)]

#region SLOTS

    /// <summary> Indica se l'inventario è aperto. </summary>
    public bool inventoryOpen = false;


    /// <summary> Array di slot dell'inventario. </summary>
    public ButtonBehaviour[] inventorySlots;

    /// <summary> Lista di slot liberi nell'inventario. </summary>
    public List<ButtonBehaviour> freeInventorySlots;        

    /// <summary> Lista di slot non del tutto pieni nell'inventario. </summary>
    public List<ButtonBehaviour> notFullInventorySlots;


    public const byte INVENTORY_SIZE = 96;

    #endregion

    [SerializeField] private GameObject infoPanel;
    [SerializeField] TextMeshProUGUI infoName;
    [SerializeField] TextMeshProUGUI infoDesc;
    [SerializeField] Image infoIcon;
    [SerializeField] Image infoBackground;
    [SerializeField] Image infoFrame;

    [SerializeField] Color[] infoIconColors;
    [SerializeField] Color[] infoFrameColors;



    private void Start () { freeInventorySlots = inventorySlots.ToList(); notFullInventorySlots.Remove(freeInventorySlots[1]); }




    public void OpenInfoPanel(Item item)
    {
        if (infoPanel == null) { print("null"); return; }


        infoPanel.SetActive(true);
        
        infoName.text = item.name;
        infoDesc.text = item.desc;
        
        infoIcon.sprite = item.icon;
        
        byte rarity = (byte)Random.Range(0, infoFrameColors.Length - 1);
        infoBackground.color = infoIconColors[rarity];
        infoFrame.color = infoFrameColors[rarity];
    }




    public void AddItem (Item item)
    {
        print("Adding item: " + item.name + " to inventory.");


        // search in the not full slots for the same item before searching for an empty slot

        if (item.isStackable && notFullInventorySlots.Any())
        {
            foreach (var slot in notFullInventorySlots)
            {
                if (slot.item == item)
                {
                    print("Found item: " + item.name + " in inventory, adding to stack.");

                    // if found, add (one) item to stack and return

                    slot.AddItem();

                    // now check if slot is full and remove from list if it is

                    if (slot.stackAmount == item.stackLimit)
                    {
                        notFullInventorySlots.Remove(slot);
                    }

                    return;
                }
            }
        }
        // if there's empty slots

        if (freeInventorySlots.Any())
        {
            // search for an empty slot to add the item

            foreach (var slot in inventorySlots)
            {
                // if slot is not empty, skip

                if (slot.isEmpty == false) continue;

                // if slot is empty, add item and remove slot from list

                print(slot.name + " is empty, adding item to it.");

                freeInventorySlots.Remove(slot);
                slot.SetItem(item);

                // if item is stackable and slot amount is not to limit...

                // --------------------------------------------------------------
                // viene già fatto da ButtonBehaviour.UpdateSlotInInventoryLists:
                // --------------------------------------------------------------

                // if (item.isStackable && slot.stackAmount < item.stackLimit)
                // {
                //     // ...then add it to the not full slots
                //     notFullInventorySlots.Add(slot);
                // }

                return;
            }
        }
    }
}
