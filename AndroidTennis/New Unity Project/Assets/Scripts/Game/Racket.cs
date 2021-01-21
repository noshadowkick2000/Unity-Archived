using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Racket : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float activeTime;
        [SerializeField] private float standardPowerFactor;
        [SerializeField] private float detectionRangeDepth;
        [SerializeField] private float detectionRangeLength;
        
        private Vector3 _heading;
        private bool _active;
        private Player _opponentPlayer;
        private Rigidbody _rigidbody;
        private bool _aftermath = false;
        private int _forceMultiplier = 5000;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public static void CleanUpRackets()
        {
            foreach (var racket in FindObjectsOfType<Racket>())
            {
                Destroy(racket.gameObject);
            }
        }

        public void ThrowRacket(Vector3 newHeading, Player opponentPlayer)
        {
            _heading = newHeading.normalized;
            _opponentPlayer = opponentPlayer;

            StartCoroutine(CoolDown());
        }

        private void Update()
        {
            if (!_active) return;
            if (CheckCollisionRacket())
            {
                Tennis.Instance.HitReturn(standardPowerFactor, !(transform.position.z > 0));
                ReleaseRacket();
            }
            else if (CheckCollisionOppositePlayer())
            {
                _opponentPlayer.KnockDown();
                ReleaseRacket();
            }
            else
            {
                transform.position += _heading * speed;
            }
        }

        private void FixedUpdate()
        {
            if (_aftermath)
            {
                ForceRacket();
            }
        }

        private IEnumerator CoolDown()
        {
            _active = true;
            yield return new WaitForSeconds(activeTime);
            ReleaseRacket();
            
        }

        private bool CheckCollisionRacket()
        {
            return (Mathf.Abs(Tennis.Instance.ball.ballPosition.y - transform.position.z) < detectionRangeDepth && 
                   (Mathf.Abs(Tennis.Instance.ball.ballPosition.x - transform.position.x) < detectionRangeDepth));
        }

        private bool CheckCollisionOppositePlayer()
        {
            return (Mathf.Abs(_opponentPlayer.Get2DPosition().y - transform.position.z) < detectionRangeDepth && 
                    (Mathf.Abs(_opponentPlayer.Get2DPosition().x - transform.position.x) < detectionRangeDepth));
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one*detectionRangeLength);
        }

        private void ReleaseRacket()
        {
            _active = false;
            _rigidbody.isKinematic = false;
            _aftermath = true;
        }

        private void ForceRacket()
        {
            _rigidbody.AddForce(_heading * _forceMultiplier);
            _aftermath = false;
        }
    }
}