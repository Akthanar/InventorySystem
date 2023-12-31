using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class ButtonSlot : MonoBehaviour, IBeginDragHandler,
    IPointerDownHandler, IPointerUpHandler, IDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    
    GameObject Child => transform.GetChild(0).gameObject;



    private GameObject panelAmount;
    private TextMeshProUGUI textAmount;

    
    public byte index => (byte)transform.GetSiblingIndex();
    public byte amount { get; private set; } = 0;
    
    
    public bool isHotbarSlot { get; private set; } = false;
    public bool isEmpty  = true;




    public bool isDragged { get; private set; } = false;
    public bool isEquipped { get; private set; } = false;





    /// <summary> contained item in this slot </summary>
    public Item item { get; private set; } = null;


    /// <summary> image component of the slot's child object </summary>
    [SerializeField] Image icon;




    private void Start ()
    {
        Child.TryGetComponent(out icon);

        panelAmount = transform.GetChild(1).gameObject;
        panelAmount.transform.GetChild(0).TryGetComponent(out textAmount);
    }




    /// <summary> set item in this slot, if not specified, set just one </summary>
    /// <param name="item"> item's scriptable object </param>
    /// <param name="amount"> default = 1 </param>
    public void SetItem (Item item, byte amount = 1)
    {
        // if item is null, slot is empty
        isEmpty = (item is null);

        // sprite empty if null, else item's sprite
        icon.sprite = (item is null) ? null : item.icon;
      
        // disable image if null, else enable
        icon.enabled = (item is not null);
        
        // can be an item or null
        this.item = item;

        // set amount, if item is null, set to zero
        this.amount = (item is null) ? (byte)0 : amount;

        // at the end, update the inventory lists
        UpdateSlotInInventoryLists(item);
    }



    /// <summary> increase item in this slot (default by 1) </summary>
    public void AddItem (byte amount = 1) => EditItem((sbyte)+amount);

    /// <summary> decrease item in this slot (default by 1) </summary>
    public void SubItem (byte amount = 1) => EditItem((sbyte)-amount);





    /// <summary> add or remove item in this slot </summary>
    /// <param name="amount"> amount to change, can be positive or negative </param>
    private void EditItem (sbyte amount)
    {
        // change amount, preventing it from going under zero or over stackLimit
        this.amount = (byte)Mathf.Clamp(this.amount + amount, 0, item.stackLimit);

        // if there are no items, clear the slot
        if (this.amount == 0)
        {
            icon.sprite = null;
            icon.enabled = false;
            item = null;
            isEmpty = true;
        }

        // at the end, update the inventory lists
        UpdateSlotInInventoryLists(item);
    }



    void UpdateSlotInInventoryLists(Item item)
    {
        textAmount.text = amount.ToString();


        // if slot is empty...
        if (isEmpty)
        {
            // ...and "free slot list" not contains this slot, remove from "not full"
            if (Inventory.instance.freeInventorySlots.Contains(this) is false)
            { Inventory.instance.freeInventorySlots.Add(this);   }
            
            Inventory.instance.notFullInventorySlots.Remove(this);  // no need to check if it's in the list
        }

        else if (item.isStackable)
        {
            if (isEmpty is false && Inventory.instance.notFullInventorySlots.Contains(this) is false)
            {
                Inventory.instance.freeInventorySlots.Remove(this);
                Inventory.instance.notFullInventorySlots.Add(this);
            }

            //panelAmount.SetActive(true);
        }
        
        panelAmount.SetActive(amount > 1);


        Inventory.instance.ReorderListsInHierarchyOrder();
    }





    
    public void OnPointerDown (PointerEventData eventData) => Inventory.instance.ClickOnSlot(this);
    public void OnPointerUp (PointerEventData eventData) => Inventory.instance.ReleaseOnSlot(this);


    public void OnDrag (PointerEventData eventData) {}
    public void OnBeginDrag (PointerEventData eventData) => Inventory.instance.TryDragItem(this);



    public void OnPointerEnter (PointerEventData eventData) => Inventory.instance.newSlot = this;
    public void OnPointerExit (PointerEventData eventData) => Inventory.instance.newSlot = null;
}
