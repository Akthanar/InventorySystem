using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class Inventory : MonoBehaviour
{
    #region SINGLETON
    void Awake () => instance = this;
    public static Inventory instance;
    #endregion


#region VARIABLES

    #region OTHER_VARIABLES

    public EventAction currentAction = EventAction.none;

    // clicked slot
    public ButtonSlot newSlot;


    const string USE_BUTTON_NAME = "USE";
    const string EQUIP_BUTTON_NAME = "EQUIP";
    const string REMOVE_BUTTON_NAME = "REMOVE";

    #endregion

    #region HEADER

    [Space(40)]


    [Header("---------------- HOTBAR SETTINGS ----------------------")]
    [Space(10)]

#endregion
    
    #region ARMOR
    
    /// <summary> Array of slots for armor. </summary>
    public ArmorSlot[] armorSlots;
    public ArmorSlot mainHandSlot;
    public ArmorSlot offHandSlot;


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

    // visualized slot
    public ButtonSlot infoSlot;
    public ArmorSlot infoArmor;

    [Space(10)]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Button buttonUse;
    [Space(10)]
    [SerializeField] private TextMeshProUGUI useButtonText;
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoDesc;
    [Space(10)]
    [SerializeField] private Image infoIcon;
    [SerializeField] private Image infoBackground;
    [SerializeField] private Image infoFrame;
    [Space(10)]
    [SerializeField] private Color[] infoIconColors;
    [SerializeField] private Color[] infoFrameColors;

    #endregion

    #region STATS_VALUES

    [SerializeField] private Image healthBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private TextMeshProUGUI attackSpeed;
    [SerializeField] private TextMeshProUGUI attackDamage;

    #endregion

#endregion


    private void Start ()
	{
		freeInventorySlots = inventorySlots.ToList();
		notFullInventorySlots.Remove(freeInventorySlots[1]);
	}



    public void ReorderListsInHierarchyOrder()
    {
        // reoreder freeInventorySlots list following hierarchy order
        freeInventorySlots = freeInventorySlots.OrderBy(x => x.transform.parent.GetSiblingIndex()).ToList();

        // reoreder notFullInventorySlots list following hierarchy order
        notFullInventorySlots = notFullInventorySlots.OrderBy(x => x.transform.parent.GetSiblingIndex()).ToList();
    }



    #region CLICK_ON_SLOT

    /// <summary> register click to confront with next event </summary>
    /// <param name="slot"> memorize clicked slot </param>
    public void ClickOnSlot (ButtonSlot slot)
    {
        print("Click on " + slot.transform.parent.name);

        // if slot is empty, don't change state and return
        if (slot.isEmpty) return;

        // memorize current action
        currentAction = EventAction.click;
    }
    #endregion

    #region DRAG_ITEM_ON_SLOT

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
    #endregion

    #region RELEASE_MOUSE_ON_EMPTY_SPACE
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
        if (infoPanel.activeSelf)
        {
            bool is_armor = infoArmor != null;


            if (Input.GetKeyDown(KeyCode.Escape)) CloseInfoPanel();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DropItem(1);
                if (is_armor) CloseInfoPanel();
                else if (infoSlot.amount == 0) CloseInfoPanel();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                DropItem(0);
                CloseInfoPanel();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (buttonUse.interactable is false) return;

                UseItem();
            }
        }
    }
    #endregion

    #region RELEASE_MOUSE_ON_SLOT
    public void ReleaseOnSlot(ButtonSlot old_slot)
    {
        switch (currentAction)
        {
            // if mouse not moved since click, open info panel
            case EventAction.click:
            {
                // store slot reference
                infoSlot = old_slot;

                // open the info panel
                OpenInfoPanel(old_slot.item);
                
                // set state to none
                currentAction = EventAction.none;

                break;
            }



            case EventAction.drag:
            {
                // check if dropped on the same slot where drag started
                if ((newSlot == null) || (old_slot.gameObject == newSlot.gameObject))
                {
                    // cancel dragging action
                    CancelDraggingAction();
                }
                // check if dropped on an empty slot
                else if (newSlot.isEmpty)
                {
                    // place dragged item in the new slot
                    newSlot.SetItem(old_slot.item, old_slot.amount);

                    // remove item from old slot
                    old_slot.SetItem(null);
                }
                else // swap items in slots
                {
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
    #endregion

    #region CANCEL_SELECTION_ON_EMPTY_SPACE

    public void CancelDraggingAction()
    {
        // set dragging to false
        currentAction = EventAction.none;

        // hide icon near mouse cursor (no parameter = null)
        MouseFollower.Instance.SetGrabbedItem();
    }

    #endregion




    #region CLOSE_INFO_PANEL

    // only called by buttons
    public void CloseInfoPanel()
    {
        StartCoroutine(CleanReferences());
        infoPanel.SetActive(false);
    }
    IEnumerator CleanReferences()
    {
        // wait for the end of the frame to avoid null references
        yield return null;
        infoArmor = null;
        infoSlot = null;
    }
    #endregion

    #region OPEN_INFO_PANEL

    /// <summary> open item information panel in inventory</summary>
    /// <param name="item">scriptable object of the picked up item</param>
    public void OpenInfoPanel(Item item, bool is_inventory_slot = true)
    {
        if (infoPanel == null) { print("null"); return; }

        byte rarity = (byte)Random.Range(0, infoFrameColors.Length - 1);


        infoPanel.SetActive(true);
        infoName.text = item.name;
        infoDesc.text = item.desc;
        infoIcon.sprite = item.icon;
        infoFrame.color = infoFrameColors[rarity];
        infoBackground.color = infoIconColors[rarity];


        if (is_inventory_slot)
        {
            // change button text 
            useButtonText.text = (item.type == ItemType.Useless) ? "OK" :
                (item.type == ItemType.Consumable) ?
                USE_BUTTON_NAME : EQUIP_BUTTON_NAME;


            // if item is a weapon and is two handed...
            if (item.type == ItemType.MainHand && item.twoHandedWeapon)
            {
                // ...and shield slot is occupied
                if (offHandSlot.isEmpty is not true)
                {
                    // lock button equip
                    buttonUse.interactable = false;
                }
            }
            // if item is a shield
            else if (item.type == ItemType.OffHand)
            {
                // ...and main hand slot is occupied by a two handed weapon
                if (mainHandSlot.isEmpty is not true && mainHandSlot.item.twoHandedWeapon)
                {
                    // lock button equip
                    buttonUse.interactable = false;
                }
            }
            // check if there's an armor slot free
            else if (item.type is ItemType.Equipable && armorSlots.Any(x => x.isEmpty) is false)
            {
                // lock button equip
                buttonUse.interactable = false;
            }
            // in any other case, unlock use/equip button
            else buttonUse.interactable = true;
        }
        else
        {
            // change button text 
            useButtonText.text = REMOVE_BUTTON_NAME;

            // if free slot unlock button
            // (inventory item are not stackable, no need to check semi-full list)
            buttonUse.interactable = freeInventorySlots.Any();
        }
    }
    #endregion

    #region ADD_ITEM_TO_DEBUG

    /// <summary> method to call when player pickup an object </summary>
    /// <param name="item">scriptable object of the picked up item</param>
    public void AddItem (Item item)
    {

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


        // if there's empty slots set item there
        if (TryFindEmptySlot(out ButtonSlot empty_slot))
        {
            empty_slot.SetItem(item);
        }
    }
    #endregion

    #region DROP_ITEM_FROM_INVENTORY

    /// <summary > method to call when player drop an object </summary>
    /// <param name="amount"> default amount 0 (= all)</param>
    public void DropItem(int amount = 0)
    {
        if (infoSlot != null)
        {
            // if amount to drop is 0 or higher than current amount, drop all
            if ((amount is 0) || (amount >= infoSlot.amount))
            {
                infoSlot.SetItem(null);
                if (infoPanel.activeSelf) CloseInfoPanel();
            }
            else infoSlot.SubItem((byte)amount);
        }
        else
        {
            EditStats();
            infoArmor.RemoveItem();
            if (infoPanel.activeSelf) CloseInfoPanel();
        }
    }

    #endregion

    #region USE_EQUIP_REMOVE_ITEM

    public void UseItem()
    {
        #region USE_INVENTORY_ITEM
        // if slot is from inventory (not armor)
        if (infoSlot != null)
        {
            switch (infoSlot.item.type)
            {
                case ItemType.Consumable:
                {
                    // if item is consumable, use it
                    EditStats(infoSlot.item);
                 
                    // remove item from inventory
                    infoSlot.SubItem();

                    if (infoSlot.amount == 0) CloseInfoPanel();

                    break;
                }
                
                case ItemType.Equipable: // place item in armor slot
                {
                    // search for empty armor slot
                    if (TryFindEmptySlot(out ArmorSlot slot))
                    {
                        // move armor item in inventory slot
                        slot.SetItem(infoSlot.item);
                        
                        // update stats
                        EditStats(infoSlot.item);

                        // remove item from inventory
                        infoSlot.SetItem(null);

                        CloseInfoPanel();
                    }
                    break;
                }
                
                case ItemType.OffHand:  // place item in shield slot
                {
                    if (offHandSlot.isEmpty)
                    {
                        offHandSlot.SetItem(infoSlot.item);
                        infoSlot.SetItem(null);
                    }
                    else offHandSlot.SwapItem(infoSlot);

                    EditStats(infoSlot.item);
                    CloseInfoPanel();

                    break;
                }

                case ItemType.MainHand: // place item in weapon slot
                {
                    if (mainHandSlot.isEmpty)
                    {
                        mainHandSlot.SetItem(infoSlot.item);
                        infoSlot.SetItem(null);
                    }
                    else mainHandSlot.SwapItem(infoSlot);

                    EditStats(infoSlot.item);
                    CloseInfoPanel();

                    break;
                }

                default: break;
            }
        }
        #endregion
        #region USE_HOTBAR_ITEM

        // if is armor slot and find empty slot
        else if (TryFindEmptySlot(out ButtonSlot slot))
        {
            // move armor item in inventory slot
            slot.SetItem(infoArmor.item);

            // remove item from armor slot
            infoArmor.RemoveItem();

            // update stats
            EditStats();

            CloseInfoPanel();
        }
        #endregion
    }
    #endregion

    #region SEARCH_EMPTY_SLOT

    private bool TryFindEmptySlot(out ButtonSlot value)
    {
        if (freeInventorySlots.Any())
        {
            // save free slot reference
            value = freeInventorySlots[0];

            // remove from list
            freeInventorySlots.Remove(value);

            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }
    private bool TryFindEmptySlot (out ArmorSlot value)
    {
        // search for empty armor slot
        foreach (var slot in armorSlots)
        {
            // if slot is not empty, skip
            if (slot.isEmpty is false) continue;

            value = slot;
            return true;
        }

        value = null;
        return false;
    }
    #endregion

    #region CHANGE_FAKE_PLAYER_STATS

    /// <summary> Edit stats when an item is equipped or removed or used. </summary>
    /// <param name="item"> item from which to take the modifiers. Default null </param>
    private void EditStats (Item item = null)
    {
        const float CONVERT_TO_PERCENTAGE = 0.01f;
        const byte INITIAL_ATTACK_SPEED = 255;
        const byte INITIAL_ATTACK_DAMAGE = 10;



        if (item != null)
        {
            var health = healthBar.fillAmount + (item.healthModifier * CONVERT_TO_PERCENTAGE);
            healthBar.fillAmount = Mathf.Clamp01(health);

            if (healthBar.fillAmount == 0) SceneManager.LoadScene(1);
        }



        int armor = 0;
        int attack_speed = 0;
        int damage = 0;


        // sum all modifiers in armor slots and offhand slot
        for (int i = 0; i < armorSlots.Length; i++)
        {
            if (armorSlots[i].isEmpty) continue;

            armor += armorSlots[i].item.resistanceModifier;
            attack_speed += armorSlots[i].item.attackSpeedModifier;
        }

        if (mainHandSlot.isEmpty is false)
        {
            armor += mainHandSlot.item.resistanceModifier;
            attack_speed += mainHandSlot.item.attackSpeedModifier;
            damage += mainHandSlot.item.damageModifier;
        }

        if (offHandSlot.isEmpty is false)
        {
            armor += offHandSlot.item.resistanceModifier;
            attack_speed += offHandSlot.item.attackSpeedModifier;
            damage += offHandSlot.item.damageModifier;
        }



        // convert to percentage and clamp between 0 and 1 and fill the bar
        shieldBar.fillAmount = Mathf.Clamp01(armor * CONVERT_TO_PERCENTAGE);

        // clamp between 1 and 100 and show on UI
        attackSpeed.text = Mathf.Clamp(INITIAL_ATTACK_SPEED + attack_speed, 1, 255).ToString();

        // clamp between 1 and 255 and show on UI
        attackDamage.text = Mathf.Clamp(INITIAL_ATTACK_DAMAGE + damage, 1, 255).ToString();
    }
    #endregion
}
