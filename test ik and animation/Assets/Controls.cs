using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Controls : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Collider[] _buffer = new Collider[4];
    private bool _onGround;
    private int _layerMask;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int size = Physics.OverlapBoxNonAlloc(transform.position, Vector3.one, _buffer, Quaternion.identity, _layerMask);
        
        _onGround = false;
        if (size > 0)
        {
            _onGround = true;
        }

        float horizontal = Input.GetAxis("Horizontal");
        _rigidbody.AddForce(new Vector3(horizontal * 800, 0, 0));
        if (Input.GetButtonDown("Jump") && _onGround)
        {
            _rigidbody.AddForce(Vector3.up * 2000);
        }
        
        transform.rotation = Quaternion.Euler(0, 180 - horizontal*85, 0);
    }
}
