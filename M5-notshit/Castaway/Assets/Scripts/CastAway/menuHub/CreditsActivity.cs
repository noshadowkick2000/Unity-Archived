using System.Collections;
using gameCore;
using ui;
using UnityEngine;


namespace menuHub
{
    public class CreditsActivity : MonoBehaviour
    {

        private bool started = false;

        [SerializeField] private MenuHandler _targetUI;
        
        private void Start()
        {
            StartCoroutine(WaitForCamera());
        }

        private IEnumerator WaitForCamera()
        {
            yield return new WaitForSecondsRealtime(5);
            started = true;
        }
        
        void Update()
        {
            if (started)
            {
                if (Input.GetButtonDown("down"))
                {
                    if (GetComponent<Animator>().GetBool("inRange"))
                    {
                        _targetUI.ShowCredits();
                    }
                }
            }
        }
    }
}