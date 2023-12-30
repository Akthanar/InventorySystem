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



    [SerializeField] private ButtonSlot _slotHover;
    [SerializeField] private string _name;
    // clicked slot
    public ButtonSlot newSlot { get => _slotHover; set { _slotHover = value; _name = value.transform.parent.name; } }

    public byte daggingAmount = 0;



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
    public ButtonSlot[] inventorySlots;

    /// <summary> Lista di slot liberi nell'inventario. </summary>
    public List<ButtonSlot> freeInventorySlots;        

    /// <summary> Lista di slot non del tutto pieni nell'inventario. </summary>
    public List<ButtonSlot> notFullInventorySlots;


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




    // public void MouseOverItem(Item item) { }
    // public void MouseOutItem() { }




    /// <summary> register click to confront with next event </summary>
    /// <param name="slot"> memorize clicked slot </param>
    public void ClickOnSlot (ButtonSlot slot)
    {
        print("Click on " + slot.transform.parent.name);

        // if slot is empty, don't change state and return
        if (slot.isEmpty) return;

        // memorize current action
        currentAction = EventAction.click;

        // ...and clicked slot
        //this.slot = slot;
    }



    /// <summary> try to drag item from slot </summary>
    public void TryDragItem(ButtonSlot slot)
    {
        // if not clicking on an item, don't do anything
        if (currentAction == EventAction.none) return;


        // if slot is empty, don't change state and return
        if (slot.isEmpty) return;


        // set dragging action
        currentAction = EventAction.drag;


        // set icon on mouse cursor
        MouseFollower.Instance.SetGrabbedItem(slot.item);
    }



    private void Update ()
    {
        if (Input.GetMouseButtonUp(0) && currentAction == EventAction.drag)
        {
            /* uso l'update per controllare se viene rilasciato il tasto del mouse
             * perchè gli eventi vengono eseguiti prima della Game Logic (quindi dell'update)
             * perciò se lo stato non è ancora cambiato, a questo punto, vuol dire che non
             * è stato rilasciato l'item sopra uno slot dell'inventario
            */
            CancelDraggingAction();
        }
    }



    public void ReleaseOnSlot(ButtonSlot old_slot)
    {
        print("Released " + old_slot.transform.parent.name + " on " + newSlot.transform.parent.name);


        switch (currentAction)
        {
            // if mouse not moved since click, open info panel
            case EventAction.click:
            {
                OpenInfoPanel(old_slot.item);
                
                currentAction = EventAction.none;

                break;
            }



            case EventAction.drag:
            {
                // check if dropped on the same slot where drag started
                if (old_slot.gameObject == newSlot.gameObject)
                {
        print("IF " + old_slot.transform.parent.name + " = " + newSlot.transform.parent.name);
                    // cancel dragging action
                    CancelDraggingAction();
                }
                else if (newSlot.isEmpty)
                {
        print("ELSE IF");
                    // place dragged item in the new slot
                    newSlot.SetItem(old_slot.item, old_slot.amount);

                    // remove item from old slot
                    old_slot.SetItem(null);
                }
                else // swap items in slots
                {
        print("ELSE");
                    // store new item and amount
                    Item item = newSlot.item;
                    byte amount = newSlot.amount;

                    // set dragged item in the new slot
                    newSlot.SetItem(old_slot.item, old_slot.amount);

                    // set stored item in the old slot
                    old_slot.SetItem(item, amount);
                }
                currentAction = EventAction.none;

                break;
            }



            // if not drag or click state, don't do anything
            default: break;
        }
    }



    public void CancelDraggingAction()
    {
        // set dragging to false
        currentAction = EventAction.none;

        // hide icon near mouse cursor (no parameter = null)
        MouseFollower.Instance.SetGrabbedItem();
    }






    /// <summary> open item information panel in inventory</summary>
    /// <param name="item">scriptable object of the picked up item</param>
    public void OpenInfoPanel(Item item)
    {
        if (infoPanel is null) { print("null"); return; }


        byte rarity = (byte)Random.Range(0, infoFrameColors.Length - 1);
        
        infoPanel.SetActive(true);
        infoName.text = item.name;
        infoDesc.text = item.desc;
        infoIcon.sprite = item.icon;
        infoFrame.color = infoFrameColors[rarity];
        infoBackground.color = infoIconColors[rarity];
    }



    /// <summary> method to call when player pickup an object </summary>
    /// <param name="item">scriptable object of the picked up item</param>
    public void AddItem (Item item)
    {
        print("Adding item: " + item.name + " to inventory.");

        // if item can be stacked and there's not full slots...
        if (item.isStackable && notFullInventorySlots.Any())
        {
            // ...search in the "not full slots" list for the same item
            foreach (var slot in notFullInventorySlots)
            {
                if (slot.item == item)
                {
                    // if found, add (one) item to stack and return
                    slot.AddItem();

                    // now check if slot is full and remove from list if it is
                    if (slot.amount == item.stackLimit)
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
                freeInventorySlots.Remove(slot);
                slot.SetItem(item);

                return;
            }
        }
    }
}
