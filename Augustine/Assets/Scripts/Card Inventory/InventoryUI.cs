using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    [Header("Setup")]
    public Inventory inventory;
    public Transform content;
    public ItemUI itemUIPrefab;
    [Header("Settings")]
    public bool showPlayerInventory = false;
    public bool showEquippedInventory = false;

    [Header("Events")]
    public ItemEvent itemSelected;
    
    // Start is called before the first frame update
    void Start()
    {
        if (showPlayerInventory)
        {
            Display(Inventory.playerInventory);
        }
        else if (showEquippedInventory)
        {
            Display(Inventory.equippedInventory);
        }
        else if (inventory)
        {
            Display(inventory);
        }
    }

    public virtual void Display(Inventory i)
    {
        if (this.inventory)
        {
            this.inventory.onChanged.RemoveListener(Refresh);
        }
        
        this.inventory = i;
        this.inventory.onChanged.AddListener(Refresh);
        Refresh();
    }

    public virtual void Refresh()
    {
        foreach (Transform t in content) //Clear old UI items
        {
            Destroy(t.gameObject);
        }
        
        foreach (Item i in inventory.items) //Create new UI items
        {
            ItemUI ui = ItemUI.Instantiate(itemUIPrefab, content);
            ui.onClicked.AddListener(UIClicked);
            ui.Display(i);
        }
    }

    public virtual void UIClicked(ItemUI iui)
    {
        itemSelected.Invoke(iui.item);
    }
    
    //Code referenced by Unity Events only

    #region  UnityEventResponders

    public void TransferToPlayerInventory(Item item)
    {
        if (Inventory.playerInventory.GetAmount() < Inventory.playerInventory.maxSize)
        {
            AddToPlayerInventory(item);
            RemoveFromOwnInventory(item);
        }
        else
        {
            Debug.Log("Can't add more items to the Inventory! Maximum number of stored items: " + Inventory.playerInventory.maxSize);
        }
    }

    public void TransferToEquippedInventory(Item item)
    {
        if (Inventory.equippedInventory.GetAmount() < Inventory.equippedInventory.maxSize)
        {
            AddToEquippedInventory(item);
            RemoveFromOwnInventory(item);
        }
        else
        {
            Debug.Log("Can't equip more items! Maximum number of equippable items: " + Inventory.equippedInventory.maxSize);
        }
    }

    public void AddToPlayerInventory(Item item)
    {
        if (Inventory.playerInventory.GetAmount() < Inventory.playerInventory.maxSize)
        {
            Inventory.playerInventory.Add(item);
        }
    }

    public void AddToEquippedInventory(Item item)
    {
        if (Inventory.equippedInventory.GetAmount() < Inventory.equippedInventory.maxSize)
        {
            Inventory.equippedInventory.Add(item);
        }
    }

    public void RemoveFromOwnInventory(Item item)
    {
        
        inventory.Remove(item);
    }

    public void Use(Item item)
    {
        if (!item.Use())
        {
            RemoveFromOwnInventory(item);
        }
    }
    
   

    #endregion
}
