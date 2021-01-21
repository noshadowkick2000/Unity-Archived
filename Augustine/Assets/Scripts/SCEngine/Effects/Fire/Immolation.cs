using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine.Effects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCEngine.Effects.Fire
{
    public class Immolation : BaseSpellEffect
    {
        // For oversight reasons, put all hardcoded values here
        private int _effectDuration = 5;
        private float _baseDmg = 20;
        private Elemental.Element _element = Elemental.Element.Fire;
        
        private void Awake()
        { 
            // Set the duration of the effect
            CountDownTime = _effectDuration;

            // Call Init() after setting the CountDownTime
            Init();
            GameClock.Instance.Ticked += Burn;
        }

        private void Burn()
        {
            EntityHealth.Damage(_element, _baseDmg);
            CountDown();
        }

        protected override void EffectOver()
        {
            GameClock.Instance.Ticked -= Burn;
            Destroy(this);
        }

        private void OnDestroy()
        {
            GameClock.Instance.Ticked -= Burn;
        }
    }
}