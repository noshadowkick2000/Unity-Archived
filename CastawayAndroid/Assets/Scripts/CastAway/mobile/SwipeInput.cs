using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeInput : MonoBehaviour
{
    public enum Direction
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    [SerializeField] private int moveTreshold;

    private bool _engaged = false;

    private int _touchID;
    private int _totalChangeX;
    private int _totalChangeY;

    private int _detectionZoneX;

    private Direction _currentDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        _detectionZoneX = (Screen.width / 2);
    }

    // Update is called once per frame
    void Update()
    {
        _currentDirection = Direction.None;
        if (!_engaged)
            CheckNewTouch();
        else
            TrackTouch();
        print(_engaged.ToString());
    }

    private void CheckNewTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch currentTouch = Input.GetTouch(i);
            if (currentTouch.position.x > _detectionZoneX)
            {
                _touchID = i;
                _engaged = true;
                break;
            }
        }
    }

    private void TrackTouch()
    {
        try
        {
            if (Input.GetTouch(_touchID).phase == TouchPhase.Ended ||
                Input.GetTouch(_touchID).phase == TouchPhase.Canceled || Mathf.Abs(_totalChangeX) > moveTreshold ||
                Mathf.Abs(_totalChangeY) > moveTreshold)
            {
                _engaged = false;

                if (Mathf.Abs(_totalChangeX) > Mathf.Abs(_totalChangeY))
                {
                    if (_totalChangeX > 0)
                        _currentDirection = Direction.Right;
                    else
                        _currentDirection = Direction.Left;
                }
                else
                {
                    if (_totalChangeY > 0)
                        _currentDirection = Direction.Up;
                    else
                        _currentDirection = Direction.Down;
                }

                _totalChangeX = 0;
                _totalChangeY = 0;
            }
            else
            {
                Touch temp = Input.GetTouch(_touchID);
                _totalChangeX += (int) temp.deltaPosition.x;
                _totalChangeY += (int) temp.deltaPosition.y;
            }
        }
        catch
        {
            _engaged = false;
            _totalChangeX = 0;
            _totalChangeY = 0;
        }
    }

    public Direction GetCurrentDirection()
    {
        return _currentDirection;
    }
}
