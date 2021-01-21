using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;

[System.Serializable]
public class ItemUIEvent : UnityEvent<ItemUI> {}

public class ItemUI : MonoBehaviour
{
    [Header("Item Settings")]
    public Item item;
    public Text itemName;
    [Header("UI Components")]
    public Text itemTooltipName;
    public Text itemTooltipType;
    public Text itemTooltipDescription;

    public ItemUIEvent onClicked;

    // Start is called before the first frame update
    void Start()
    {
        if (item)
        {
            Display(item);
        }
    }

    public virtual void Display(Item item)
    {
        this.item = item;
        itemName.text = item.name;
        itemTooltipName.text = item.name;
        itemTooltipType.text = item.type;
        itemTooltipDescription.text = item.description;
    }

    public virtual void Click()
    {
        onClicked.Invoke(this);
    }
}
