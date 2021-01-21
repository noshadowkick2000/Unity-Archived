using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine.Projectiles;
using UnityEngine;

namespace SCEngine.Spells
{
  public class BaseSpell : MonoBehaviour
  {
    [SerializeField] public string spellName; // Name of spell
    [SerializeField] protected Elemental.Element spellElement; // Element of spell
    [SerializeField] protected int coolDownTime; // Length of cooldown of spell in Ticks
    protected int CoolDownCounter; // Amount of Ticks until cooldown is deactivated

    [SerializeField] protected GameObject[] projectileQueue;

    protected void InitCooldown()
    {
      GameClock.Instance.Ticked += CoolDown;
    }

    private void CoolDown()
    {
      if (CoolDownCounter > 0)
        CoolDownCounter--;
    }
    
    public virtual bool CastSpell()
    {
      return CoolDownCounter == 0;
    }

    /// <summary>
    /// Sets coolDownCounter to the std coolDownTime and returns whether spell is available
    /// </summary>
    protected bool BasicCoolDown()
    {
      if (CoolDownCounter != 0) return false;
      CoolDownCounter = coolDownTime;
      return true;
    }
    
    /// <summary>
    /// Generates a single projectile and initializes it with a heading and speed
    /// </summary>
    /// <param name="projectileId">Index of object in projectileQueue</param>
    /// <param name="normalizedHeading">Direction in which the projectile will move</param>
    protected void SingleShot(int projectileId, Vector3 normalizedHeading, Vector3 mousePosition)
    {
      if (projectileQueue.Length > 0)
      {
        BaseProjectile outgoing = Instantiate(projectileQueue[projectileId], transform.position, transform.rotation).GetComponent<BaseProjectile>();
        outgoing.StraightShot(normalizedHeading, mousePosition);
      }
      else
      {
        print("No projectile set in projectileQueue");
      }
    }
  }
}