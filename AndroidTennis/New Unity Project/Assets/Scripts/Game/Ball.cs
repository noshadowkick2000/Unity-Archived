using Game.Camera;
using UnityEngine;

namespace Game
{
  public class Ball : MonoBehaviour
  {
    private Rigidbody _ballRigidBody;
    
    public Vector2 ballPosition;
    public Vector2 ballDirection;
    private float _ballSpeed;
    public Vector2 firstBouncePosition; // Vector2 at which ball will bounce on floor
    public Vector2 secondBouncePosition;
    //private GameObject _curBall; is just "gameobject"

    private float _ballA;
    private float _ballC;
    private float _ballXOffset;
    private float _ballDeltaY;
    private float _ballStartY;
    private float _ballStartDifference;
    private float _ballBaseY;

    private bool _firstBounce;
    
    private Vector3 _ballLastPosition;

    private float _fieldHalfWidthX;
    private float _fieldHalfWidthY;
    private float _netY;
    private float _netHeight;
    private float _minSpeed;
    private float _speedMult;

    private void ShadowBallPosition()
    {
      ballPosition = new Vector2(transform.position.x, transform.position.z);
    }
    public float ForceScale()
    {
      return transform.localScale.x;
    }

    public void Serve(Player startingPlayer, float fieldX, float fieldY, float netY, float netHeight, float minSpeed, float speedMult)
    {
      _firstBounce = false;
      _ballRigidBody = GetComponent<Rigidbody>();
      transform.position = startingPlayer.transform.position;
      startingPlayer.StartServe(gameObject);

      _fieldHalfWidthX = fieldX;
      _fieldHalfWidthY = fieldY;
      _netY = netY;
      _netHeight = netHeight;
      _minSpeed = minSpeed;
      _speedMult = speedMult;

      ShadowBallPosition();
    }

    public void ServeHit(Player cpu, Player humanPlayer, float powerFactor, float reach, bool returning)
    {
      if (powerFactor > _minSpeed / 100)
        _ballSpeed = powerFactor;
      else
        _ballSpeed = _minSpeed / 100;

      ballDirection = returning ? (cpu.Get2DPosition() - ballPosition).normalized : (humanPlayer.Get2DPosition() - ballPosition).normalized;
      
      firstBouncePosition = ballPosition + ballDirection * reach;
      secondBouncePosition = ballPosition + ballDirection * 1.5f * reach;
        
      _ballDeltaY = 0;
      _ballC = transform.position.y;
      _ballA = -_ballC;
      _ballBaseY = 0;
      _ballXOffset = 0;
      _ballStartY = ballPosition.y;
      _ballStartDifference = Mathf.Abs(_ballStartY - firstBouncePosition.y);
    }

    public void ReturnHit(float powerFactor, float reach, Vector2 direction)
    {
      _ballSpeed = powerFactor > 1 ? 1.5f : powerFactor;
      ballDirection = direction;
      
      firstBouncePosition = ballPosition + ballDirection * reach;
      secondBouncePosition = ballPosition + ballDirection * 1.5f * reach;
                
      _ballDeltaY = 0;
      _ballC = 5f;
      _ballA = -_ballC*2;
      _ballBaseY = 0;
      _ballXOffset = .4f * Mathf.Sqrt(-_ballC / _ballA);
      _ballStartY = ballPosition.y;
      _ballStartDifference = Mathf.Abs(_ballStartY - firstBouncePosition.y);

      _firstBounce = false;
    }

    public void ReleaseBall()
    {
      _ballRigidBody.isKinematic = false;
    }
    
    // TODO ATTACH BALL SCRIPT TO PREFAB
    
    public void MoveBall(bool returning)
    {
      _ballLastPosition = transform.position;

      float x = Mathf.Abs(ballPosition.y - _ballStartY)/_ballStartDifference;

      transform.position += new Vector3(ballDirection.x, 0, ballDirection.y) * _ballSpeed * _speedMult;
      _ballDeltaY = _ballA * Mathf.Pow(x - _ballXOffset, 2) + _ballC;
      transform.position = new Vector3(transform.position.x, _ballBaseY + _ballDeltaY, transform.position.z);

      // Check bouncing
      if (x >= 1)
      {
        Bounce(returning);
      }

      Debug.DrawLine(transform.position, new Vector3(firstBouncePosition.x, 0, firstBouncePosition.y));
      ShadowBallPosition();
    }
    
    private void Bounce(bool returning)
    {
      if (!_firstBounce)
      {
        if ((returning && ballPosition.y < _netY) || (!returning && ballPosition.y > _netY))
        {
          Tennis.Instance.Point(false);
          Director.Instance.SecondBounce();
          print("ball on own side");
        }
        else
        {
          _firstBounce = true;
            
          _ballDeltaY = 0;
          _ballC = _ballC * 1.5f;
          _ballA = -_ballC;
          _ballBaseY = 0;
          _ballXOffset = Mathf.Sqrt(-_ballC / _ballA);
          _ballStartY = ballPosition.y;
          _ballStartDifference = Mathf.Abs(_ballStartY - secondBouncePosition.y);
                
          print("First Bounce");
                
          CheckBallOut();
        }
      }
      else
      {
        Tennis.Instance.Point(true);
        Director.Instance.SecondBounce();
        print("Second bounce");
      }
    }

    public bool HitNet(bool returning)
    {
      return (((returning && ballPosition.y + _netY <= transform.localScale.x / 2) ||
               (!returning && -ballPosition.y + _netY <= transform.localScale.x / 2)) &&
              transform.position.y <= _netHeight - transform.localScale.x / 2 &&
              Mathf.Abs(ballPosition.x) <= _fieldHalfWidthX);
    }

    public void ForceMoveBall()
    {
      _ballRigidBody.AddForce((transform.position - _ballLastPosition).normalized*4500);
    }
    
    private void CheckBallOut()
    {
      if (Mathf.Abs(ballPosition.x) > _fieldHalfWidthX || Mathf.Abs(ballPosition.y) > _fieldHalfWidthY)
      {
        Tennis.Instance.Point(false);
        Director.Instance.BallOut();
        print("Ball out");
      }
    }

    public void FreezeBall(bool returning)
    {
      transform.position = returning ? new Vector3(ballPosition.x, transform.position.y, -transform.localScale.x/2) : new Vector3(ballPosition.x, transform.position.y, transform.localScale.x/2);
      print("net " + ballPosition.ToString());
    }
  }
}