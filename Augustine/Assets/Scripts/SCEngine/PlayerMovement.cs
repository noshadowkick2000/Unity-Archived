using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace SCEngine
{
    public class PlayerMovement : BaseMovement
    {
        private AugustineControls _input;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _input = new AugustineControls();
            _rigidbody = GetComponent<Rigidbody>();
            
            Init();
            
            _input.Player.Enable();
        }

        private void Update()
        {
            MovePlayer(_input.Player.Move.ReadValue<Vector2>());
        }

        private void MovePlayer(Vector2 input)
        {
            input *= 50;
            _rigidbody.AddForce(new Vector3(input.x, 0, input.y));
        }
    }
}