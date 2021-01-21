using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine.Projectiles;
using UnityEngine;

namespace SCEngine.Projectiles.Water
{
    public class WaveProjectile : BaseProjectile
    {
        private Collider[] _waveBuffer = new Collider[10];
        [SerializeField] private Vector3 waveSize;

        private void Awake()
        {
            InitDieDelay();
        }

        private void Update()
        {
            MoveStraight();
            CheckEnemiesInWave();
        }

        private void CheckEnemiesInWave()
        {
            int count = Physics.OverlapBoxNonAlloc(transform.position, waveSize, _waveBuffer, transform.rotation,  TestingManager.Instance.enemyMask);

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    _waveBuffer[i].GetComponent<BaseMovement>().LockMovement(true);
                    _waveBuffer[i].gameObject.transform.position += heading * baseSpeed;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, waveSize);
        }
    }
}