using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string name;
    public string desc;
    public Sprite icon;

    [Space(20)]

    public ItemType type;

    [Space(10)]
    [Header("Item infos:")]


    [Range(1, 99)] // don't exceed 127 (for sbyte)
    public byte stackLimit;

    public bool isStackable => (stackLimit > 1); // if stackLimit is greater than 1, the item is stackable
    
   
    [Space(10)]

    [Header("Item values:")]

    [Tooltip("How much health will be added or removeduse")]
    public byte healthModifier;

    [Tooltip("How much mana will be added or removed")]
    public byte manaModifier;

    [Tooltip ("How much it slows down or speeds up the player")]
    public byte motionSpeedModifier;

    [Tooltip("How much damage will be prevented or added on every attack")]
    public byte damageModifier;

    [Tooltip("How much increase or decrease the attack speed")]
    public byte attackSpeedModifier;

    [Tooltip("How much increase or decrease the damage inflicted")]
    public byte attackModifier;
    
    public byte objectDurability;
    public byte effectDuration;


    public byte itemWeight;
}
