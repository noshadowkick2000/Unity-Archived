using gameObjects;
using UnityEngine;

namespace gameCore
{
    public class InputListener : MonoBehaviour
    {

        private bool[] controls = new bool[4];
        //1: w
        //2: s
        //3: a
        //4: d

        private Player _player;

        private void Start()
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }

        private void Update()
        {

            //reset all controls
            if (Input.GetKeyUp(KeyCode.W))
            {
                controls[0] = false;
            }
            
            if (Input.GetKeyUp(KeyCode.S))
            {
                controls[1] = false;
            }
            
            if (Input.GetKeyUp(KeyCode.A))
            {
                controls[2] = false;
            }
            
            if (Input.GetKeyUp(KeyCode.D))
            {
                controls[3] = false;
            }

            //collect all true inputs

            if (Input.GetKeyDown(KeyCode.W))
            {
                controls[0] = true;
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                controls[1] = true;
            }
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                controls[2] = true;
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                controls[3] = true;
            }
        }

        private void FixedUpdate()
        {
            int vertical = 0;
            
            if (controls[0])
            {
                vertical += 1;
            }

            if (controls[1])
            {
                vertical -= 1;
            }

            int horizontal = 0;
            
            if (controls[2])
            {
                horizontal -= 1;
            }
            
            if (controls[3])
            {
                horizontal += 1;
            }
            
            _player.MovePlayer(horizontal, vertical);
        }
    }
}