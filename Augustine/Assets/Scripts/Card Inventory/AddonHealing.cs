using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddonHealing : ItemAddon
{
   public int healAmount = 10;

   public override void Use()
   {
      Debug.Log("Healing " + healAmount);
   }
}
