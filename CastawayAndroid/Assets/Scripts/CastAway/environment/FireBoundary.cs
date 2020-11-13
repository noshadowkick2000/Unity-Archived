using gameObjects;
using UnityEngine;

namespace environments
{
    public class FireBoundary : MonoBehaviour
    {
        private Collider _collider;
        
        void Start()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("zombie"))
            {
                Physics.IgnoreCollision(other.collider, _collider);
                other.gameObject.GetComponent<SharedZombie>().RelayOnFireDamage();
                other.gameObject.GetComponent<SharedZombie>().SpawnFire();
            }
        }
    }
}