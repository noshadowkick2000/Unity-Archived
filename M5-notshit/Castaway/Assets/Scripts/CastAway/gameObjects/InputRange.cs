//DEPRECATED
/*using System;
using UnityEngine;

namespace gameObjects
{
    public class InputRange : MonoBehaviour
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
                _player.SetInInputRange(true, other.gameObject.GetComponent<SharedZombie>());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("zombie"))
            {
                _player.SetInInputRange(false, other.gameObject.GetComponent<SharedZombie>());
            }
        }
    }
}*/