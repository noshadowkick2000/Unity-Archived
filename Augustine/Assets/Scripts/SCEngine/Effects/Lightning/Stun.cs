using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine.Effects.Lightning
{
    public class Stun : BaseSpellEffect
    {
        // For oversight reasons, put all hardcoded values here
        private int _effectDuration = 3;
        private Elemental.Element _element = Elemental.Element.Lightning;
        private BaseMovement _baseMovement;
        
        private void Awake()
        { 
            // Set the duration of the effect
            CountDownTime = _effectDuration;

            // Call Init() after setting the CountDownTime
            Init();
            GameClock.Instance.Ticked += CountDown;
            GameClock.Instance.Ticked += Lock;

            _baseMovement = GetComponent<BaseMovement>();
        }

        private void Lock()
        {
            if (CountDownCounter > 0)
            {
                _baseMovement.LockMovement(true);
            }
        }

        protected override void EffectOver()
        {
            GameClock.Instance.Ticked -= CountDown;
            Destroy(this);
        }
        
        protected void OnDestroy()
        {
            GameClock.Instance.Ticked -= CountDown;
        }
    }
}