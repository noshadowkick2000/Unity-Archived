
using System;
using UnityEngine;
using CoreStuff;

namespace Rest
{
    public class Listen : MonoBehaviour
    {
        private Test _gameObject;

        private void Start()
        {
            _gameObject = GameObject.Find("Test").GetComponent<Test>();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Q))
            {
                print("test");
                _gameObject.ChangeScene(true);
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                print("test");
                _gameObject.ChangeScene(false);
            }
        }
    }
}