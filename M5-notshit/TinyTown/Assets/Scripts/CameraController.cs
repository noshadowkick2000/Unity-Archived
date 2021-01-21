using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;
using Random = UnityEngine.Random;

//GOTTA TEST ALL THIS SHIT

public class CameraController : MonoBehaviour
{
    private Camera _mainCam;
    private States _curState = States.FreeLook;
    private Vector3 _goalPosition;
    private Vector3 _oldPosition;
    private float _transitionEndTime;

    public bool cameraMoving = false;

    [SerializeField] private float zoomMultiplier;
    [SerializeField] private float freeDragSpeed;
    [SerializeField] private float dragTreshold;
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;

    //[SerializeField] private int freeLookCamOffsetY;
    [SerializeField] private Vector3 freeLookCamRotation;
    [SerializeField] private float freeLookTransitionTime;
    
    [SerializeField] private Vector3 tinyEditCamOffset;
    [SerializeField] private float tinyEditTransitionTime;

    [SerializeField] private Renderer grassRenderer;
    private float _grassOffset = 0; //stupid URP with their bullshit doesnt have a working offset setter

    private Transform _followTiny;
    [SerializeField] private int tinyFollowOffsetZ;
    
    public enum States
    {
        FreeLook = 0,
        TinyEditView = 1,
        Transitioning = 2,
        FollowingTiny = 3,
        NonResponsive = 4
    }

    private void Awake()
    {
        //assuming only 1 cam for now
        _mainCam = Camera.main;
    }

    public void SetState(States goalState)
    {
        _curState = goalState;
    }

    public void SetFocusFollow(Transform tiny)
    {
        _followTiny = tiny;
    }

    // Update is called once per frame
    void Update()
    {
        cameraMoving = false;
        
        switch (_curState)
        {
            case States.FreeLook:
                TouchMoveCamera();
                break;
            case States.FollowingTiny:
                FollowTiny();
                break;
        }
        
        //set grass texture
        grassRenderer.material.SetTextureOffset("_BaseMap", new Vector2(0, -_mainCam.transform.position.z * 0.1f));
    }

    private void FollowTiny()
    {
        _mainCam.transform.position = Vector3.Lerp(new Vector3(_mainCam.transform.position.x, _mainCam.transform.position.y, _followTiny.position.z - tinyFollowOffsetZ), _mainCam.transform.position, .4f);
    }

    private void TouchMoveCamera()
    {
        int fingers = Input.touchCount;
        
        if (fingers > 0)
        {
            Touch[] touches = Input.touches;

            Vector3 oldPos = _mainCam.transform.position;
            
            //use single drag to move camera
            //use pinch to zoom in and out
            switch (fingers)
            {
                case 1:
                    //float heightSmoother = _mainCam.orthographicSize / maxSize;
                    float heightSmoother = .5f;
                    if (touches[0].deltaPosition.magnitude > dragTreshold*heightSmoother)
                    {
                        cameraMoving = true;
                        
                        float dragX = -touches[0].deltaPosition.x * freeDragSpeed * heightSmoother;

                        //CHECK
                        Vector3 newPos = Vector3.Lerp(oldPos, oldPos + new Vector3(0 , 0, -dragX), .4f); 
                        _mainCam.transform.position = newPos;
                    } 
                    break;
                /*case 2:
                    float oldDistance = Vector3.Distance( touches[0].position - touches[0].deltaPosition, touches[1].position - touches[1].deltaPosition);
                    float newDistance = Vector3.Distance(touches[0].position, touches[1].position);
                    float zoom = newDistance - oldDistance;
                    if (Mathf.Abs(zoom) > dragTreshold)
                    {
                        float newSize = _mainCam.orthographicSize - zoom * zoomMultiplier;
                        if (newSize > minSize && newSize < maxSize)
                        {
                            cameraMoving = true;
                            _mainCam.orthographicSize = newSize;
                        }
                    }
                    break;*/
            }
        }
    }
}
