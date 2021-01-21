using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine.Spells;
using UnityEngine;

namespace SCEngine.Spells.Earth
{
    public class EarthPlatform : BaseSpell
    {
        private Collider[] _affectedChars = new Collider[10];
        [SerializeField] private int effectDuration;
        private int _effectCounter; // How many ticks the effect has lasted
        [SerializeField] private float range;
        
        private void Awake()
        {
            InitCooldown();
        }

        public override bool CastSpell()
        {
            if (BasicCoolDown() && _effectCounter == 0)
            {
                StartCounter();
                return true;
            }

            return false;
        }

        private void StartCounter()
        {
            GetEnemiesInRange();
            _effectCounter = effectDuration;
            GameClock.Instance.Ticked += CountDown;
        }

        private void CountDown()
        {
            _effectCounter--;
            if (_effectCounter == 0)
            {
                GameClock.Instance.Ticked -= CountDown;   
            }
        }

        private void GetEnemiesInRange()
        {
            int amt = Physics.OverlapSphereNonAlloc(transform.position, range, _affectedChars, TestingManager.Instance.enemyMask);
            
            for (int i = 0; i < amt; i++)
            {
                _affectedChars[i].gameObject.GetComponent<BaseMovement>().LockMovement(true);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}