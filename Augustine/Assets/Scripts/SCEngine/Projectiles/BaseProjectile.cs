using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine.Projectiles
{
    public class BaseProjectile : MonoBehaviour
    {
        public enum ProjectileType
        {
            Straight,
            Homing,
            Lob
        }
        
        [SerializeField] protected Vector3 heading;
        [SerializeField] protected ProjectileType projectileType;
        [SerializeField] protected int duration; // Lifetime of projectile in ticks
        private int _counter; // Counts current ticks
        
        [SerializeField] protected float baseSpeed;
        private Vector3 _colliderSize = Vector3.one; // Bit janky rn but as implemented, it checks whether current position is inside of checked object's collider

        protected Collider[] ColliderBuffer = new Collider[1];
        
        /// <summary>
        /// Call at Awake or Start to make the projectile die after duration ticks
        /// </summary>
        protected void InitDieDelay()
        {
            GameClock.Instance.Ticked += CheckDie;
        }

        private void CheckDie()
        {
            if (_counter < duration)
            {
                _counter++;
            }
            else
            {
                GameClock.Instance.Ticked -= CheckDie;
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Moves projectile in straight line using current heading
        /// </summary>
        protected void MoveStraight()
        {
            transform.position += heading * baseSpeed;
        }
        
        /// <summary>
        /// Call to check whether projectile has hit enemy
        /// </summary>
        protected void CheckCollisionEnemy()
        {
            int count = Physics.OverlapBoxNonAlloc(transform.position, _colliderSize, ColliderBuffer, transform.rotation,  TestingManager.Instance.enemyMask);
            
            if (count > 0)
            {
                Hit();
            }
        }

        /// <summary>
        /// Use override call for executing code at the end of the projectile's lifetime
        /// </summary>
        protected virtual void Hit()
        {
            
        }
        
        /// <summary>
        /// Sets heading and speed, use for normal straight shots
        /// </summary>
        /// <param name="normalizedHeading">Direction in which projectile will travel</param>
        /// /// <param name="mousePosition">Location of mouse for rotation</param>
        public void StraightShot(Vector3 normalizedHeading, Vector3 mousePosition)
        {
            heading = normalizedHeading * baseSpeed;
            transform.LookAt(mousePosition);
        }
        
        /// <summary>
        /// Damages attached character
        /// </summary>
        /// <param name="entityHealth">Health component of Entity</param>
        /// <param name="attackingElement">Element of effect</param>
        /// <param name="dmg">Amount of damage to deal</param>
        protected void Damage(Health entityHealth, Elemental.Element attackingElement, float dmg)
        {
            entityHealth.MHealth -= dmg;
        }

        private void OnDestroy()
        {
            GameClock.Instance.Ticked -= CheckDie;
        }
    }
}