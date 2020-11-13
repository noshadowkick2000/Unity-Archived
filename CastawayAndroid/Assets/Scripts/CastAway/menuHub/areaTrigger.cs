/*using UnityEngine;

namespace menuHub
{
    public class areaTrigger : MonoBehaviour
    {

        [SerializeField] private GameObject id;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                id.GetComponent<Animator>().SetBool("inRange", true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                id.GetComponent<Animator>().SetBool("inRange", false);
            }
        }
    }
}
*/
