using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class ArmorSlot : MonoBehaviour, IPointerDownHandler//, IPointerUpHandler
//ISelectHandler , IDragHandler, IPointerEnterHandler, IPointerExitHandler
{


    public void OnPointerDown (PointerEventData eventData)
    {
        if (isEmpty) return;

        Inventory.instance.infoArmor = this;
        Inventory.instance.OpenInfoPanel(item, false);
    }
    // public void OnPointerUp (PointerEventData eventData) => Inventory.instance.ReleaseOnSlot(this);

     
    // public void OnSelect (BaseEventData eventData) =>  print ("SELECT");



    private Button button;


    public enum SlotType { mainHand, offHand, armor }
    public SlotType slotType;


    public Item item { get; private set; }


    [SerializeField] ItemType itemType;
    [SerializeField] Image itemIcon;
    public Image typeIcon;


    public bool isEmpty => item == null;
    [SerializeField] bool hasTwoHandedWeapon = false;


    void Start()
    {
        TryGetComponent(out button);

        // itemType = item.type;
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
        else UnlockSlot2();
    }


    /// <summary> remove item from slot and place in inventory </summary>
    public void RemoveItem()
    {
        typeIcon.enabled = true;
        itemIcon.enabled = false;

        button.interactable = false;
        item = null;

        if (hasTwoHandedWeapon) UnlockSlot2();
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



    private void UnlockSlot2()
    {
        print("unlock slot 2");
        hasTwoHandedWeapon = false;
        Inventory.instance.offHandSlot.typeIcon.color = Color.black;
    }




    // quando il pulsante viene cliccato passa il valore a inventory per l'info panel

    // quando apri pannello passa (item e FALSE)
}
