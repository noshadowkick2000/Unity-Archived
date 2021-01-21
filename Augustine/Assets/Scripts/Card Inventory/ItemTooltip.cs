using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    [Header("Item Settings - Unused")]
    public Item item;
    public Text itemTooltipName;
    public Text itemTooltipDescription;
    
    [Header("Tooltip Components")] 
    public GameObject tooltip;

    // Start is called before the first frame update
    void Start()
    {
        if (tooltip.transform.position.x + 300 > Screen.width)
        {
            transform.position = new Vector3((transform.position.x-600), transform.position.y, transform.position.z);
        }
        //if (item)
        //{
            //Display(item);
        //}
    }

    public virtual void Display(Item item)
    {
        this.item = item;
        itemTooltipName.text = item.name;
        itemTooltipDescription.text = item.description;
    }
}
