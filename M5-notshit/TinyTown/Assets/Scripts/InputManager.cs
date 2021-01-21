using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Unity.Collections;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Camera _mainCam;
    //check first if camera is moving otherwise dont select items
    private CameraController _cameraController;

    public GameObject focusObject;
    private bool _hasFocus = false;
    private bool _buttonPressed = false;

    private bool _tinySelected = false;

    private void Awake()
    {
        _mainCam = Camera.main;
        _cameraController = FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_buttonPressed)
        {
            CheckInput();
        }
        _buttonPressed = false;
    }

    private void CheckInput()
    {
        if (Input.touchCount > 0 && !_cameraController.cameraMoving)
        {
            Touch temp = Input.GetTouch(0);

            if (temp.phase == TouchPhase.Began)
            {
                Ray ray = _mainCam.ScreenPointToRay(temp.position);
                RaycastHit hit;
            
                if (Physics.Raycast(ray, out hit, 100))
                {
                    GameObject selectedObject = hit.transform.gameObject;

                    if (selectedObject.CompareTag("tiny"))
                    {
                        if (_tinySelected)
                        {
                            _hasFocus = true;
                            selectedObject.GetComponent<Skeleton>().EditMode();

                            focusObject = selectedObject;
                        }
                        else
                        {
                            _cameraController.SetFocusFollow(selectedObject.transform);
                            _cameraController.SetState(CameraController.States.FollowingTiny);
                            _tinySelected = true;
                        }
                    }
                }
            }
            else if (temp.phase == TouchPhase.Moved)
            {
                if (temp.deltaPosition.magnitude > 10)
                {
                    if (_hasFocus)
                        focusObject.GetComponent<Skeleton>().FreeMode();
                    _hasFocus = false;
                    _tinySelected = false;
                    _cameraController.SetState(CameraController.States.FreeLook);
                }
            }
        }
    } 

    public void DisableCheck()
    {
        //disables input checks for 1 frame
        //called when button is pressed
        _buttonPressed = true;
    }
}
