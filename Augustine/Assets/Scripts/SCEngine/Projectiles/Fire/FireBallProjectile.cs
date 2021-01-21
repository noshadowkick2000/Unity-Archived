using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine.Effects.Fire;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace SCEngine.Projectiles.Fire
{
    public class FireBallProjectile : BaseProjectile
    {
        private void Awake()
        {
            InitDieDelay();
        }

        private void Update()
        {
            MoveStraight();
            CheckCollisionEnemy();
        }

        protected override void Hit()
        {
            ColliderBuffer[0].gameObject.AddComponent<Immolation>();
            
            // Temp ----------------------------------------------------------------------------------------------------
            Destroy(gameObject);
        }
    }
}