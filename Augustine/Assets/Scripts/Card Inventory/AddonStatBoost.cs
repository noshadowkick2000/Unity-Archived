using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddonStatBoost : ItemAddon
{
    public string stat = "Cwispyness";
    public int amount = 10;

    public override void Use()
    {
        Debug.Log("Boosting " + stat + " by " + amount);
    }
}
