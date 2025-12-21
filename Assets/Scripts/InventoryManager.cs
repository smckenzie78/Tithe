using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    ItemData itemData;
    private int totalItems;
    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    public void addItem(ItemData item, int quantity)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += quantity;
        }
        else
        {
            inventory[item] = quantity;
        }
    }

    public void showInventory()
    {
        foreach (ItemData item in inventory.Keys)
        {
            Debug.Log(item.itemName + "(" + inventory[item] + ")" + " " + item.category);
        }
    }

    public Dictionary<ItemData, int> queryByCategory(ItemData.Category query)
    {
        Dictionary<ItemData, int> filteredList = new Dictionary<ItemData, int>();
        foreach (ItemData item in inventory.Keys)
        {
            if (item.category == query)
            {
                filteredList.Add(item, inventory[item]);
            }
        }
        return filteredList;
    }

    public void removeItem(ItemData item, int quantity)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] -= quantity;
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
            }
        }
    }
}
