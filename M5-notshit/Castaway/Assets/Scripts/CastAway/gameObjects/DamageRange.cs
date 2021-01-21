//DEPRECATED
/*using System;
using UnityEngine;

namespace gameObjects
{
    public class DamageRange : MonoBehaviour
    {

        private Player _player;

        private void Start()
        {
            _player = gameObject.GetComponentInParent<Player>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("zombie"))
            {
                _player.SetInDamageRange(true, other.gameObject.GetComponent<SharedZombie>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("zombie"))
            {
                _player.SetInDamageRange(false, other.gameObject.GetComponent<SharedZombie>());
            }
        }
    }
}*/