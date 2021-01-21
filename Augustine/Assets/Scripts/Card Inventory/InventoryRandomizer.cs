using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryRandomizer : MonoBehaviour
{
    [Header("Setup")]
    public Inventory inventoryToRandomize;
    public Inventory itemPool;
    [Header("Result")]
    public int drawAmount;
    public List<Item> drawnItems;

    void Awake()
    {
        drawAmount = inventoryToRandomize.maxSize;
        if (itemPool.GetAmount() <= 0)
        {
            Debug.Log("InventoryRandomizer disabled due to item pool being empty or negative");
            enabled = false;
        }

        else
        {
            for (int i = 0; i < drawAmount; i++)
            {
                Item item;
                int itemId = Random.Range(0,itemPool.GetAmount());
                item = itemPool.items[itemId];
                drawnItems.Add(item);
                inventoryToRandomize.items.Add(item);
            }
        }
    }
}
