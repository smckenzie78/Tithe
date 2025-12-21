using System;
using System.Runtime.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int itemID;
    public enum Category { Crop, Tool, Medicine, Misc };
    public Category category;
    public int baseYield;
    public int harvestTime;
    
}
