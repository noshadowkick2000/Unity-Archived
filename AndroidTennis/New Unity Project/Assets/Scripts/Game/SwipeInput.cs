using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    /*public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }*/

    [SerializeField] private int moveTreshold;
    [SerializeField] private int maxSwipe;
    private HumanPlayer _humanPlayer;

    private int _totalChangeY;

    private void Start()
    {
        _humanPlayer = FindObjectOfType<HumanPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        TrackTouch();
    }

    private void TrackTouch()
    {
        try
        {
            if ((Input.GetTouch(0).phase == TouchPhase.Ended ||
                Input.GetTouch(0).phase == TouchPhase.Canceled) && 
                Mathf.Abs(_totalChangeY) > moveTreshold)
            {
                _humanPlayer.Swing(_totalChangeY/maxSwipe);
                _totalChangeY = 0;
            }
            else
            {
                Touch temp = Input.GetTouch(0);
                _totalChangeY += (int) temp.deltaPosition.y;
            }
        }
        catch
        {
            _totalChangeY = 0;
        }
    }

    /*public Direction GetCurrentDirection()
    {
        return _currentDirection;
    }*/
}
