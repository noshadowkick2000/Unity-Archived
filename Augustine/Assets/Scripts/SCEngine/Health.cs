using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float health;

        public float MHealth
        {
            get { return health;}
            set
            {
                health = value;
                if (health <= 0)
                {
                    // Prob send call to Game Manager
                    print(gameObject.name + " died");
                    Destroy(gameObject);
                }
            }
        }
        
        /// <summary>
        /// Damages attached character
        /// </summary>
        /// <param name="attackingElement">Element of effect</param>
        /// <param name="dmg">Amount of damage to deal</param>
        public void Damage(Elemental.Element attackingElement, float dmg)
        {
            MHealth -= TestingManager.Instance.ReturnEffectiveDamage(dmg, attackingElement, GetComponent<Elemental>().characterElement);
        }
    }
}