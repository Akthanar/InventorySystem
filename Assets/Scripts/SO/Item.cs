using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string name;
    public string desc;
    public Sprite icon;

    [Space(20)]


    [Header("Item infos:")]
    
    public ItemType type;
    [Space(10)]
    public bool twoHandedWeapon = false;
    [Space(10)]
    [Range(1, 99)] // don't exceed 127 (for sbyte)
    public byte stackLimit;

    public bool isStackable => (stackLimit > 1); // if stackLimit is greater than 1, the item is stackable
    
   
    [Space(10), Header("Item values:"), Space(20)]

    [Tooltip("How much health will be added or removeduse")]
    public sbyte healthModifier;

    [Tooltip("How much armor will be added or removed")]
    public sbyte resistanceModifier;

    [Space(10)]

    [Tooltip("How much damage will be prevented or added on every attack")]
    public sbyte damageModifier;

    [Tooltip("How much increase or decrease the attack speed")]
    public sbyte attackSpeedModifier;

    [Space(10)]

    public byte itemWeight;
}
