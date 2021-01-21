using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public static Inventory playerInventory;
    public static Inventory equippedInventory;

    [Header("Settings")]
        public List<Item> items;
    public bool isPlayerInventory = false;
    public bool isEquippedInventory = false;
    public int maxSize;

    public UnityEvent onChanged;

    void Awake()
    {
        if (isPlayerInventory)
        {
            playerInventory = this;
        }
        else if (isEquippedInventory)
        {
            equippedInventory = this;
        }
    }

    public void Add(Item item)
    {
        items.Add(item);   
     onChanged.Invoke();
    }

    public void Remove(Item item)
    {
        if (!items.Contains(item))
        {
            return;
        }
        
        items.Remove(item);
        onChanged.Invoke();
    }

    public int GetAmount()
    {
        return items.Count;
    }
}
