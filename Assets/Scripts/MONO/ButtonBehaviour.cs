using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class ButtonBehaviour : MonoBehaviour, IPointerClickHandler, // evento di click del mouse
    IPointerDownHandler, IPointerUpHandler,             // eventi di pressione del mouse
    IPointerEnterHandler, IPointerExitHandler,          // eventi di posizione del mouse
    ISelectHandler, IDeselectHandler,                   // eventi di selezione dei pulsanti
    IBeginDragHandler, IDragHandler, IEndDragHandler    // eventi di trascinamento del mouse
{

    GameObject child => transform.GetChild(0).gameObject;



    private RectTransform rect;

    private GameObject panelAmount;
    private TextMeshProUGUI textAmount;

    
    public byte slotIndex => (byte)transform.GetSiblingIndex();
    public byte stackAmount { get; private set; } = 0;
    
    
    public bool isHotbarSlot { get; private set; } = false;
    public bool isEmpty  = true;




    public bool isDragged { get; private set; } = false;
    public bool isEquipped { get; private set; } = false;





    /// <summary> contained item in this slot </summary>
    public Item item { get; private set; } = null;


    /// <summary> image component of the slot's child object </summary>
    [SerializeField] Image icon;



    /// <summary> set item in this slot, if not specified, set just one </summary>
    /// <param name="item"> item's scriptable object </param>
    /// <param name="amount"> default = 1 </param>
    public void SetItem (Item item, byte amount = 1)
    {
        this.icon.sprite = item.icon;
        this.icon.enabled = true;
        this.item = item;
        this.isEmpty = false;
        this.stackAmount = amount;

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

        stackAmount = (byte)Mathf.Clamp(stackAmount + amount, 0, item.stackLimit);

        // if there are no items, clear the slot

        if (stackAmount == 0)
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
        textAmount.text = stackAmount.ToString();


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






    public void OnPointerClick (PointerEventData eventData) { } // => Inventory.instance.OpenInfoPanel(item);


    public void OnPointerDown (PointerEventData eventData) => print("CLICK DOWN");
    public void OnPointerUp (PointerEventData eventData) => Inventory.instance.OpenInfoPanel(item); //print ("CLICK UP");


    public void OnPointerEnter (PointerEventData eventData) => print("ENTER");
    public void OnPointerExit (PointerEventData eventData) => print("EXIT");


    public void OnSelect (BaseEventData eventData) =>  print ("SELECT");
    public void OnDeselect (BaseEventData eventData) => print("DESELECT");


    public void OnBeginDrag (PointerEventData eventData) => MouseFollower.Instance.SetGrabbedItem(item);
    public void OnDrag (PointerEventData eventData) => print("ON DRAG");
    public void OnEndDrag (PointerEventData eventData) => print("END DRAG");


    public void OnClick()
    {
        Debug.Log("Button Clicked");
    }

    public void MouseOver ()
    {
        Debug.Log("Mouse over");
    }

    public void OnMouseExit ()
    {
        Debug.Log("MONO: Mouse exit");
    }

    public void OnMouseEnter ()
    {
        Debug.Log("MONO: Mouse enter");
    }
    public void MousePush ()
    {
        Debug.Log("Mouse down");
    }

    public void MouseHold ()
    {
        Debug.Log("Mouse hold");
    }

    public void MousePull ()
    {
        Debug.Log("Mouse up");
    }


    public void Deselect ()
    {
        Debug.Log("Button Deselected");
    }


    private void OnMouseDown ()
    {
        Debug.Log("MONO: Mouse down");
        OnClick();
    }

    private void OnMouseUp ()
    {
        Debug.Log("MONO: Mouse up");
        MousePull();
    }

    private void OnMouseOver ()
    {
        Debug.Log("MONO: Mouse over");
        MouseOver();
    }


    private void Start ()
    {
        TryGetComponent(out rect);
        child.TryGetComponent(out icon);

        panelAmount = transform.GetChild(1).gameObject;
        panelAmount.transform.GetChild(0).TryGetComponent(out textAmount);
    }

    private void Update ()
    {
        
    }
}
