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
    


    public EventAction currentAction = EventAction.none;




    // clicked item
    public Item item = null;

    // clicked slot
    public ButtonBehaviour slot = null;





#region HEADER

    [Space(40)]


    [Header("---------------- HOTBAR SETTINGS ----------------------")]
    [Space(10)]

#endregion
#region HOTBAR

    /// <summary> Indica se la hotbar è abilitata. </summary>
    public bool enableHotbarInInventory = true;
    
    /// <summary> Array di slot della hotbar. </summary>
    public GameObject[] hotbarSlots;

    public const byte HOTBAR_SIZE = 10;

    #endregion
#region HEADER

    [Space(20)]

    [Header("---------------- INV. SLOT SETTINGS --------------------")]
    [Space(10)]

#endregion
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
#region HEADER

    [Space(20)]

    [Header("---------------- INFO ITEM PANEL -----------------------")]
    [Space(10)]

#endregion
#region INFO_PANEL

    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoDesc;
    [SerializeField] private Image infoIcon;
    [SerializeField] private Image infoBackground;
    [SerializeField] private Image infoFrame;
    [Space(10)]
    [SerializeField] private Color[] infoIconColors;
    [SerializeField] private Color[] infoFrameColors;
    
#endregion
    

    private void Start ()
	{
		freeInventorySlots = inventorySlots.ToList();
		notFullInventorySlots.Remove(freeInventorySlots[1]);
	}




    /// <summary> register click to confront with next event </summary>
    /// <param name="slot"> memorize clicked slot </param>
    public void ClickOnSlot (ButtonBehaviour slot)
    {
        // if slot is empty, don't change state and return
        if (slot.isEmpty) return;


        // memorize current action
        currentAction = EventAction.click;

        this.slot = slot;
    }



    /// <summary> try to drag item from slot </summary>
    public void TryDragItem()
    {
        // if not clicking on an item, don't do anything
        if (currentAction == EventAction.none) return;



        // if slot is empty, don't change state and return
        if (this.slot.isEmpty) return;
        

        // set dragging action
        currentAction = EventAction.drag;


        // memorize item
        this.item = this.slot.item;

        // set icon near mouse cursor
        MouseFollower.Instance.SetGrabbedItem(this.item);

        print("Drag started from slot: " + slot.name + " with item: " + this.item.name);
    }



    private void Update ()
    {
        if (Input.GetMouseButtonUp(0) && currentAction == EventAction.drag)
        {
            /* uso l'update per controllare se viene rilasciato il tasto del mouse
             * perchè gli eventi vengono eseguiti prima della Game Logic (quindi dell'update)
             * quindi se lo stato non è ancora cambiato a questo punto, vuol dire che non
             * è stato rilasciato il tasto sopra uno slot dell'inventario
            */
            CancelDraggingAction();
        }
    }



    public void TryToPlaceOnSlot(ButtonBehaviour slot)
    {
        // if not dragging something don't do anything
        if (currentAction != EventAction.drag) return;


        // check if dropped on the same slot where drag started
        if (slot == this.slot)
        {
            
            // non fare niente, fai tornare l'oggetto al suo posto


        }
        else if (slot.isEmpty)
        {


            // piazza l'oggetto


        }
        else
        {
            

            // scambia gli oggetti


        }





        // check if slot is empty
        // if (slot.isEmpty)
        // {
        //     // if empty, place item in slot
        //     slot.SetItem(currentDraggedItem);
        //     // remove item from inventory
        //     RemoveItem(currentDraggedItem);
        //     // stop dragging
        //     StopDragging();
        // }
        // else
        // {
        //     // if not empty, check if item is stackable
        //     if (currentDraggedItem.isStackable)
        //     {
        //         // if stackable, check if item in slot is the same
        //         if (slot.item == currentDraggedItem)
        //         {
        //             // if same, add item to slot
        //             slot.AddItem();
        //             // remove item from inventory
        //             RemoveItem(currentDraggedItem);
        //             // stop dragging
        //             StopDragging();
        //         }
        //         else
        //         {
        //             // if not same, swap items
        //             SwapItems(slot);
        //         }
        //     }
        //     else
        //     {
        //         // if not stackable, swap items
        //         SwapItems(slot);
        //     }
        // }
    }



    public void CancelDraggingAction()
    {
        // set dragging to false
        currentAction = EventAction.none;

        // reset slot where drag started
        // slotWhichDragStarted = null;

        // reset item
        item = null;

        // hide icon near mouse cursor
        MouseFollower.Instance.SetGrabbedItem();

        print("Drag stopped.");
    }






    /// <summary> open item information panel in inventory</summary>
    /// <param name="item">scriptable object of the picked up item</param>
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



    /// <summary> method to call when player pickup an object </summary>
    /// <param name="item">scriptable object of the picked up item</param>
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
                    if (slot.StackAmount == item.stackLimit)
                        notFullInventorySlots.Remove(slot);

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
