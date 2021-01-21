using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float serveHeight = 8;
    [SerializeField] private float serveCurve = -10;
    [SerializeField] private float racketRecoverTime = 1;
    [SerializeField] private float koRecoveryTime = 5;
    [SerializeField] private GameObject racketPrefab;
    [SerializeField] private Transform rightHandAnchor;

    [SerializeField] private Material material;
    private Color _primeColor;
    private Color _restColor;

    private Animator _animator;

    protected float powerT; // val between 0 and 1 which denotes the power of the current swing
    
    protected bool serving = true;
    protected bool inAir = false;

    protected bool hasRacket;
    protected bool ko = false;

    protected bool recoveringRacket;

    private bool _inRacketRange;

    private bool _playerOne;

    private Vector3 _startPos;

    public bool InRacketRange
    {
        set
        {
            if (_inRacketRange != value)
            {
                _inRacketRange = value;
                PrimeSwing(_inRacketRange);
            }
        }
    }

    public bool ballSideLeft;

    public Racket racketObject;

    private readonly int _primeHash = Animator.StringToHash("Prime");
    private readonly int _hitHash = Animator.StringToHash("Hit");
    private readonly int _leftHash = Animator.StringToHash("Left");
    private readonly int _rightHash = Animator.StringToHash("Right");
    private readonly int _movingHash = Animator.StringToHash("Walking");
    private readonly int _koHash = Animator.StringToHash("KO");
    private readonly int _notKoHash = Animator.StringToHash("NotKO");

    // Start is called before the first frame update
    public void Init(bool playerOne)
    {
        GiveNewRacket();
        _playerOne = playerOne;
        _startPos = transform.position;
        _animator = GetComponentInChildren<Animator>();
    }

    public virtual void CheckInput()
    {
        
    }

    private void GiveNewRacket()
    {
        racketObject = Instantiate(racketPrefab, rightHandAnchor).GetComponent<Racket>();
        hasRacket = true;
        Score.Instance.IncrementRackets(_playerOne);
    }

    public virtual void StartServe(GameObject curBall)
    {
    }

    public virtual void ResetPlayer()
    {
        transform.position = _startPos;
        GiveNewRacket();
        _animator.SetBool(_primeHash, false);
        _animator.ResetTrigger(_hitHash);
        _animator.ResetTrigger(_rightHash);
        _animator.ResetTrigger(_leftHash);
        _animator.SetBool(_movingHash, false);
        _animator.SetTrigger(_notKoHash);
        _animator.ResetTrigger(_notKoHash);
    }

    public IEnumerator ThrowBall(GameObject curBall)
    {
        Vector3 basePosition = curBall.transform.position;
        float startTime = Time.time;
        float intersectOrigin = Mathf.Sqrt(-serveHeight / serveCurve);

        while (Time.time <= startTime + intersectOrigin*2)
        {
            float x = Time.time - startTime;

            var deltaY = serveCurve * Mathf.Pow(x - intersectOrigin, 2) + serveHeight;

            powerT = deltaY / serveHeight;

            curBall.transform.position = basePosition + new Vector3(0, deltaY, 0);
            
            yield return null;
        }

        inAir = false;
    }

    public Vector2 Get2DPosition()
    {
        Vector2 planePosition = new Vector2(transform.position.x, transform.position.z);
        return planePosition;
    }

    public void KnockDown()
    {
        Score.Instance.IncrementKnockDown(_playerOne);
        StartCoroutine(RecoverKO());
    }

    private IEnumerator RecoverKO()
    {
        ko = true;
        _animator.SetTrigger(_koHash);
        yield return new WaitForSeconds(koRecoveryTime);
        ko = false;
        _animator.SetTrigger(_notKoHash);
    }

    protected IEnumerator RecoverRacket()
    {
        GiveNewRacket();
        yield return new WaitForSeconds(racketRecoverTime);
        hasRacket = true;
        recoveringRacket = false;
    }

    protected void ThrowRacket(Vector3 direction)
    {
        hasRacket = false;
        racketObject.transform.SetParent(null, true);
        racketObject.ThrowRacket(direction, Tennis.Instance.ReturnOppositePlayer(this));
        racketObject = null;
        _animator.SetBool(_primeHash, false);
    }

    protected void HitReturn(float pf, bool humanPlayer)
    {
        _animator.SetBool(_primeHash, true);
        _animator.SetTrigger(_hitHash);
        _animator.ResetTrigger(ballSideLeft ? _rightHash : _leftHash);
        _animator.SetTrigger(ballSideLeft ? _leftHash : _rightHash);

        if (_inRacketRange)
        {
            Tennis.Instance.HitReturn(pf, humanPlayer);
            _animator.SetBool(_primeHash, false);
        }
    }

    private void PrimeSwing(bool primed)
    {
        // TEMP for debugging replace with animation set later
        material.color = primed ? Color.green : Color.red;
    }

    public void MovePlayerToGoal(Vector2 playerAttractor, float maxMoveSpeed)
    {
        if (ko) return;
        Vector3 target = new Vector3(playerAttractor.x, transform.position.y, playerAttractor.y);
        if (transform.position == target)
            _animator.SetBool(_movingHash, false);
        else
        {
            _animator.SetBool(_movingHash, true);
            transform.position = Vector3.MoveTowards(transform.position, target, maxMoveSpeed);
        }
    }

    public void MovePlayerToBall(Vector2 ballPosition)
    {
        if (ko) return;
        _animator.SetBool(_movingHash, true);
        transform.position = new Vector3(Mathf.Lerp(ballPosition.x, transform.position.x, .99f), 
            transform.position.y, Mathf.Lerp(_startPos.z, transform.position.z, .99f));
    }

    public void PrimePlayer()
    {
        _animator.SetBool(_primeHash, true);
        _animator.ResetTrigger(ballSideLeft ? _rightHash : _leftHash);
        _animator.SetTrigger(ballSideLeft ? _leftHash : _rightHash);
    }
}
