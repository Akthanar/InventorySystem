using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class ArmorSlot : MonoBehaviour, IPointerDownHandler
{


    public void OnPointerDown (PointerEventData eventData)
    {
        if (isEmpty) return;

        Inventory.instance.infoArmor = this;
        Inventory.instance.OpenInfoPanel(item, false);
    }




    public enum SlotType { mainHand, offHand, armor } public SlotType slotType;

    public Image itemIcon { get; private set;}
    public Image typeIcon { get; private set; }
    public Item item { get; private set; }

    
    public bool isEmpty => item == null;
    private bool hasTwoHandedWeapon = false;
    [SerializeField] private Button button;



    void Start ()
    {
        TryGetComponent(out button);

        typeIcon = transform.GetChild(0).GetComponent<Image>();
        itemIcon = transform.GetChild(1).GetComponent<Image>();
    }


    public void SetItem(Item item = null)
    {
        if (item == null) RemoveItem();
        
        this.item = item;
        itemIcon.sprite = item.icon;
        button.interactable = true;

        typeIcon.enabled = false;
        itemIcon.enabled = true;


        if (this.item.twoHandedWeapon)
        {
            print("2 handed weapon");
            hasTwoHandedWeapon = true;
            Inventory.instance.offHandSlot.typeIcon.color = Color.red;
        }
        else UnlockSecondSlot();
    }


    /// <summary> remove item from slot and place in inventory </summary>
    public void RemoveItem()
    {
        typeIcon.enabled = true;
        itemIcon.enabled = false;

        button.interactable = false;
        item = null;

        if (hasTwoHandedWeapon) UnlockSecondSlot();
    }



    /// <summary> Swap items between armor slot and inventory slot </summary>
    /// <param name="slot"> inventory slot reference. </param>
    public void SwapItem(ButtonSlot slot)
    {
        // store current item in temp variable
        var temp_item = this.item;

        // set current item to the new item
        SetItem(slot.item);

        // move this item to the inv. slot
        slot.SetItem(temp_item);
    }



    private void UnlockSecondSlot()
    {
        hasTwoHandedWeapon = false;
        Inventory.instance.offHandSlot.typeIcon.color = Color.black;
    }
}
