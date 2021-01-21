using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Camera;
using UnityEngine;

[RequireComponent(typeof(Score))]
public class Tennis : Singleton<Tennis>
{
    [Header("GameObjects")]
    [SerializeField] private Player playerOne;
    [SerializeField] private Player playerTwo;
    [SerializeField] private GameObject ballPrefab;

    [Header("Balancing")] 
    [SerializeField] private float maxMoveSpeed; // Maximum speed at which player moves towards the x coord of the ball
    [SerializeField] private float maxBallSpeed; // Actually more like max range
    [SerializeField] private float minBallSpeed; // Minimum range
    [SerializeField] private float ballSpeedMultiplier;
    [SerializeField] private float maxDeviationAngle; // Maximum angle which can be applied to ball when hit based on timing
    [SerializeField] public float maxHitTimeWindow; // Maximum amount of time before or after ball is at player z coordinate while player can still hit the ball
    [SerializeField] private float racketRange; // Maximum x distance at which player can still return ball
    [SerializeField] private float fieldHalfWidthX; // Width of half a field
    [SerializeField] private float fieldHalfWidthY; // Length of half a field
    [SerializeField] private float netHeight;
    
    public bool ballServed = false;

    public Ball ball;

    private float _netY = 0; // Y coordinate of location of net
    
    private float _playerAttractorOffset;
    private Vector2 _playerAttractor; // Point with an offset in the direction of the ball with size _playerAttractorOffset to which the player moves when ball is returned;
    private bool _returning; // True if player has hit the ball but playerTwo has not returned the ball yet;

    private float _forceMultiplier = 5000;

    private bool _aftermath = false;

    private bool _controlsActive = false;

    private void Start()
    {
        playerOne.Init(true);
        playerTwo.Init(false);
    }

    public Player ReturnOppositePlayer(Player thisPlayer)
    {
        if (thisPlayer == playerOne)
            return playerTwo;
        return playerOne;
    }

    // Start is called before the first frame update
    public void StartGame()
    {
        Director.Instance.SetParameters(fieldHalfWidthX, playerOne);
        
        _playerAttractorOffset = racketRange * .8f;

        _controlsActive = true;

        GiveServe(true);
    }

    private void GiveServe(bool playerServe)
    {
        Director.Instance.ResetCamera();

        ball = Instantiate(ballPrefab).GetComponent<Ball>();

        _forceMultiplier = ball.ForceScale() * _forceMultiplier;

        if (playerServe)
        {
            ball.Serve(playerOne, fieldHalfWidthX, fieldHalfWidthY, _netY, netHeight, minBallSpeed, ballSpeedMultiplier);
            _returning = true;
        }
        else
        {
            ball.Serve(playerTwo, fieldHalfWidthX, fieldHalfWidthY, _netY, netHeight, minBallSpeed, ballSpeedMultiplier);
            _returning = false;
        }
    }

    public void HitServe(float powerFactor)
    {
        float reach = minBallSpeed + (powerFactor * (maxBallSpeed - minBallSpeed));
        ball.ServeHit(playerTwo, playerOne, powerFactor, reach, _returning);
        ballServed = true;

        GenerateWalkGoal();
    }

    public void HitReturn(float powerFactor, bool sentByplayerOne)
    {
        if ((!sentByplayerOne || _returning) && (sentByplayerOne || !_returning)) return;
        
        float deltaZ;
            
        if (ball.ballPosition.y > 0)
        {
            deltaZ = ball.ballPosition.y - playerTwo.Get2DPosition().y;
        }
        else
        {
            deltaZ = (-ball.ballPosition.y) - (-playerOne.Get2DPosition().y);
        }
            
        float deltaZNormalized = deltaZ / maxHitTimeWindow;
                
        // Return hit, calc angle and speed etc

        float timingAngle = 0;
        Vector2 ballDirection;

        if (!_returning)
        {
            if (playerOne.Get2DPosition().x > ball.ballPosition.x)
            {
                // player plays backhand
                timingAngle = -deltaZNormalized * maxDeviationAngle;
                print("player backhand");
            }
            else
            {
                // player plays forehand
                timingAngle = deltaZNormalized * maxDeviationAngle;
                print("player forehand");
            }

            Vector3 direction = Quaternion.Euler(0, timingAngle, 0) * Vector3.forward;
            ballDirection = new Vector2(direction.x, direction.z);
        }
        else
        {
            if (playerTwo.Get2DPosition().x < ball.ballPosition.x)
            {
                // playerTwo plays forehand
                timingAngle = -deltaZNormalized * maxDeviationAngle;
            }
            else
            {
                // playerTwo plays backhand
                timingAngle = deltaZNormalized * maxDeviationAngle;
            }

            Vector3 direction = Quaternion.Euler(0, timingAngle, 0) * Vector3.back;
            ballDirection = new Vector2(direction.x, direction.z);
        }
                
        float reach = minBallSpeed + (powerFactor * (maxBallSpeed - minBallSpeed));
            
        ball.ReturnHit(powerFactor, reach, ballDirection);

        _returning = !_returning;

        GenerateWalkGoal();
    }

    public void Point(bool returningPoint)
    {
        bool humanPoint;

        if (returningPoint)
            humanPoint = _returning;
        else
            humanPoint = !_returning;

        print(humanPoint ? "point player" : "point playerTwo");

        _aftermath = true;
        
        ball.ReleaseBall();
        
        ballServed = false;

        bool set = Score.Instance.GivePoint(humanPoint);
        
        StartCoroutine(ResetGame(set));

        Director.Instance.DisplayScore(Score.Instance.ReturnScore(true), Score.Instance.ReturnScore(false));
    }

    private IEnumerator ResetGame(bool set)
    {
        yield return new WaitForSeconds(2);
        
        // Stop game, delete ball and give point
        Destroy(ball.gameObject);
        Racket.CleanUpRackets();
        
        playerOne.ResetPlayer();
        playerTwo.ResetPlayer();

        if (!set)
        {
            GiveServe(true);
        }
        else
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Director.Instance.DisplayEndScreen();
        Director.Instance.SetEndCard(Score.Instance.ReturnScore(true), Score.Instance.ReturnScore(false),
            Score.Instance.ReturnTimesKnockedDown(true), Score.Instance.ReturnRacketsUsed(playerOne));
        Score.Instance.SaveScore();
        StartCoroutine(LoopEndScreen());
    }
    
    private IEnumerator LoopEndScreen()
    {
        while (true)
        {
            Director.Instance.OrbitEndScreenCamera();
            
            // Replace with touchinput later
            if (Input.anyKey)
            {
                StopCoroutine(LoopEndScreen());
                LevelManager.Instance.LoadNextScene("MainMenu");
                break;
            }
                
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ballServed)
        {
            SetPlayersInRange();
            CheckPlayerInputs();
            MovePlayers();
            UpdateBall();
            Director.Instance.MoveCamera(ball.ballPosition);
        }
        else
        {
            if (ball != null)
                Director.Instance.AimSideLineCam(ball.gameObject.transform.position);
        }
    }

    private void SetPlayersInRange()
    {
        playerOne.InRacketRange = (Mathf.Abs(playerOne.Get2DPosition().x - ball.ballPosition.x) < racketRange) && (Mathf.Abs(playerOne.Get2DPosition().y - ball.ballPosition.y) < maxHitTimeWindow);
        playerTwo.InRacketRange = (Mathf.Abs(playerTwo.Get2DPosition().x - ball.ballPosition.x) < racketRange)  && (Mathf.Abs(playerTwo.Get2DPosition().y - ball.ballPosition.y) < maxHitTimeWindow);
    }

    private void CheckPlayerInputs()
    {
        playerOne.CheckInput();
        playerTwo.CheckInput();
    }

    private void FixedUpdate()
    {
        if (_aftermath)
        {
            ForceBall();
        }
    }

    void MovePlayers()
    {
        if (_returning)
        {
            playerOne.MovePlayerToBall(ball.ballPosition);
            playerTwo.MovePlayerToGoal(_playerAttractor, maxMoveSpeed);
        }
        else
        {
            playerTwo.MovePlayerToBall(ball.ballPosition);
            playerOne.MovePlayerToGoal(_playerAttractor, maxMoveSpeed);
        }
    }

    void GenerateWalkGoal()
    {
        // Far position = point in between first bounce and second bounce with offset
        // Close position = point in between first bounce and center line (use ball direction to calculate center) with offset
        // If position of player y is closer to the far position than the close position
        // Move the player to the middle of the first bounce and second bounce location with an x offset
        // Else 
        // Move the player to the middle of the first bounce and center line intersect location 

        Vector2 farPosition = Vector2.Lerp(ball.firstBouncePosition, ball.secondBouncePosition, .4f);

        // Calculate line formula for intersect with x axis
        float a = ball.ballDirection.y / ball.ballDirection.x;
        float b = ball.firstBouncePosition.y - (a * ball.firstBouncePosition.x);
        float intersectX = ((_netY-b) / a);
        Vector2 closePosition = Vector2.Lerp(ball.firstBouncePosition, new Vector2(intersectX, 0), .5f);

        if (_returning)
        {
            float distanceToFar = Mathf.Abs(playerTwo.Get2DPosition().y - farPosition.y);
            float distanceToClose = Mathf.Abs(playerTwo.Get2DPosition().y - closePosition.y);
            
            _playerAttractor = distanceToFar < distanceToClose ? farPosition : closePosition;
            
            if (playerTwo.Get2DPosition().x > _playerAttractor.x)
            {
                _playerAttractor += new Vector2(_playerAttractorOffset, 0);
                playerTwo.ballSideLeft = false;
            }
            else
            {
                _playerAttractor += new Vector2(-_playerAttractorOffset, 0);
                playerTwo.ballSideLeft = true;
            }
            playerTwo.PrimePlayer();
        }
        else
        {
            float distanceToFar = Mathf.Abs(playerOne.Get2DPosition().y - farPosition.y);
            float distanceToClose = Mathf.Abs(playerOne.Get2DPosition().y - closePosition.y);
            
            _playerAttractor = distanceToFar < distanceToClose ? farPosition : closePosition;
            
            if (playerOne.Get2DPosition().x > _playerAttractor.x)
            {
                _playerAttractor += new Vector2(_playerAttractorOffset, 0);
                playerOne.ballSideLeft = true;
            }
            else
            {
                _playerAttractor += new Vector2(-_playerAttractorOffset, 0);
                playerOne.ballSideLeft = false;
            }
            playerOne.PrimePlayer();
        }
    }

    void UpdateBall()
    {
        ball.MoveBall(_returning); 
        
        if (ball.HitNet(_returning))
        {
            Point(!_returning);
            ball.FreezeBall(_returning);
            Director.Instance.Net();
        }
    }

    void ForceBall()
    {
        ball.ForceMoveBall();
        _aftermath = false;
    }
}
