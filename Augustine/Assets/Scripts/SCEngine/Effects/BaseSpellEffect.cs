using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace SCEngine.Effects
{
    public class BaseSpellEffect : MonoBehaviour
    {
        protected Health EntityHealth;
        protected Elemental EntityElement;
        protected int CountDownTime; // Length of cooldown of effect in Ticks
        protected int CountDownCounter; // Amount of Ticks until cooldown is deactivated

        /// <summary>
        /// Call at start of child instantiation
        /// Caches the Health and Elemental Components
        /// </summary>
        protected void Init()
        {
            EntityHealth = GetComponent<Health>();
            EntityElement = GetComponent<Elemental>();
            CountDownCounter = CountDownTime;
        }

        /// <summary>
        /// Takes away one tick from the CountDownCounter
        /// Use for standard lasting of effect
        /// </summary>
        protected void CountDown()
        {
            if (CountDownCounter > 0)
                CountDownCounter--;
            else
                EffectOver();
        }

        /// <summary>
        /// Use override call for executing code at the end of the countdown
        /// </summary>
        protected virtual void EffectOver()
        {
            
        }
    }
}