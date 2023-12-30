using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ButtonBehaviour : MonoBehaviour, /* IPointerClickHandler, */ // evento di click
    IPointerDownHandler, IPointerUpHandler,                 // eventi di pressione del mouse
    IPointerEnterHandler, IPointerExitHandler,              // eventi di posizione del mouse
    /*ISelectHandler,*/ IDeselectHandler,                   // eventi di selezione dei pulsanti
    IBeginDragHandler/*, IDragHandler, IEndDragHandler*/    // eventi di trascinamento del mouse
{
    

    GameObject Child => transform.GetChild(0).gameObject;



    private RectTransform rect;

    private GameObject panelAmount;
    private TextMeshProUGUI textAmount;

    
    public byte SlotIndex => (byte)transform.GetSiblingIndex();
    public byte StackAmount { get; private set; } = 0;
    
    
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
        TryGetComponent(out rect);
        Child.TryGetComponent(out icon);

        panelAmount = transform.GetChild(1).gameObject;
        panelAmount.transform.GetChild(0).TryGetComponent(out textAmount);
    }




    /// <summary> set item in this slot, if not specified, set just one </summary>
    /// <param name="item"> item's scriptable object </param>
    /// <param name="amount"> default = 1 </param>
    public void SetItem (Item item, byte amount = 1)
    {
        this.icon.sprite = item.icon;
        this.icon.enabled = true;
        this.item = item;
        this.isEmpty = false;
        this.StackAmount = amount;

        // at the end, update the inventory lists
        UpdateSlotInInventoryLists();
    }



    /// <summary> increase item in this slot (default by 1) </summary>
    public void AddItem (byte amount = 1) => EditItem((sbyte)+amount);

    /// <summary> decrease item in this slot (default by 1) </summary>
    public void SubItem (byte amount = 1) => EditItem((sbyte)-amount);




    /// <summary> add or remove item in this slot </summary>
    /// <param name="amount"> amount to change, can be positive or negative </param>
    public void EditItem (sbyte amount)
    {
        // change amount, preventing it from going under zero or over stackLimit

        StackAmount = (byte)Mathf.Clamp(StackAmount + amount, 0, item.stackLimit);

        // if there are no items, clear the slot

        if (StackAmount == 0)
        {
            this.icon.sprite = null;
            this.icon.enabled = false;
            this.item = null;
            this.isEmpty = true;
        }

        // at the end, update the inventory lists
        UpdateSlotInInventoryLists();
    }



    void UpdateSlotInInventoryLists()
    {
        textAmount.text = StackAmount.ToString();


        // if slot is empty, add to free slots and remove from not full slots
        if (isEmpty && Inventory.instance.freeInventorySlots.Contains(this) == false)
        {
            Inventory.instance.freeInventorySlots.Add(this);        // add to the free slots list
            Inventory.instance.notFullInventorySlots.Remove(this);  // no need to check if it's in the list

            panelAmount.SetActive(false);
        }
        else if (item.isStackable)
        {
            if (this.isEmpty == false && Inventory.instance.notFullInventorySlots.Contains(this) == false)
            {
                Inventory.instance.freeInventorySlots.Remove(this);
                Inventory.instance.notFullInventorySlots.Add(this);

            }
            
            panelAmount.SetActive(true);
        }
    }






    
    public void OnPointerDown (PointerEventData eventData) => print("CLICK DOWN");
    public void OnPointerUp (PointerEventData eventData) => print ("CLICK UP");
    // Inventory.instance.OpenInfoPanel(item);
        

    public void OnPointerEnter (PointerEventData eventData) => print("ENTER");
    public void OnPointerExit (PointerEventData eventData) => print("EXIT");


    public void OnBeginDrag (PointerEventData eventData) => Inventory.instance.TryDragItem();



    //public void OnSelect (BaseEventData eventData) =>  print ("SELECT");
    public void OnDeselect (BaseEventData eventData) => print("DESELECT");
}
