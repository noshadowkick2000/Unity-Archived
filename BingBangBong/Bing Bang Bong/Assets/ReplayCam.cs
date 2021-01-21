using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayCam : MonoBehaviour
{
    private bool active = false;
    private Transform target;

    [SerializeField] private Vector3[] positions;
    private int index = 0;

    private Vector3 initPos;
    private Quaternion initRot;
    
    private void Awake()
    {
        initPos = transform.position;
        initRot = transform.rotation;
    }

    public void AnchorCamera(Transform target)
    {
        this.target = target;
    }

    public void DeAnchorDeActivateCamera()
    {
        active = false;
        target = null;
        transform.position = initPos;
        transform.rotation = initRot;
    }

    public void ChangePosition()
    {
        if (!active)
            active = true;
        index++;
        if (index >= positions.Length)
            index = 0;
        transform.position = positions[index];
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        transform.LookAt(target);
    }
}
