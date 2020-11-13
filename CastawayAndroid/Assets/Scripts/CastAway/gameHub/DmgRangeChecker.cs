//CURENTLY OBSOLETE
using UnityEngine;

namespace gameHub
{
    public class DmgRangeChecker : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("zombie"))
            {
                //other.GetComponent<Zombie>().SetInBoatRange(true);;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("zombie"))
            {
                //other.GetComponent<Zombie>().SetInBoatRange(false);
            }
        }
    }
}