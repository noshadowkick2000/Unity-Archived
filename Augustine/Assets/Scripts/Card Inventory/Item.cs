using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine.Spells;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ItemEvent : UnityEvent<Item> {}

public class Item : MonoBehaviour
{
    public bool consumable = true;
    public string itemName;
    public string type;
    public string description;

    private void Awake()
    {
        itemName = gameObject.name;
    }

    //use this instance. Returns true if item is kept, false if destroyed
    public bool Use()
    {
        ItemAddon[] addons = GetComponents<ItemAddon>();
        foreach (ItemAddon addon in addons)
        {
            addon.Use();
        }
        
        return !consumable;
    }
}
