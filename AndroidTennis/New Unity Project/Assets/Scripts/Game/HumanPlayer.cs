using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    [SerializeField] private int recoverySwings;
    [SerializeField] private float swingCoolDown;
    
    private bool _swinging = false;
    private int _recoverySwingsCounter = 0;

    public override void CheckInput()
    {
        if (ko) return;
        if (hasRacket && !_swinging)
        {
            // Listen for input on swinging racket and throwing racket
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Swing(.8f);
            }
            else
            {
                // TEMP ----------------------------------------------------------------
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    ThrowRacket(Vector3.left);
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    ThrowRacket(Vector3.right);
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                    ThrowRacket((Tennis.Instance.ReturnOppositePlayer(this).transform.position - transform.position));
            }
        }
        else
        {
            // Listen for input on swinging hand and shouting for new racket
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                CheckRacketRecover();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !_swinging)
            {
                Swing(.6f);
            }
        }
    }

    private void CheckRacketRecover()
    {
        if (recoveringRacket) return;
        if (_recoverySwingsCounter == recoverySwings)
        {
            StartCoroutine(RecoverRacket());
            _recoverySwingsCounter = 0;
            recoveringRacket = true;
        }
        else
        {
            _recoverySwingsCounter++;
        }
    }

    public void Swing(float powerFactor)
    {
        HitReturn(powerFactor, true);
        StartCoroutine(SwingCoolDown());
        print("returning ball");
    }

    private IEnumerator SwingCoolDown()
    {
        _swinging = true;
        yield return new WaitForSeconds(swingCoolDown);
        _swinging = false;
    }

    public override void StartServe(GameObject curBall)
    {
        StartCoroutine(Serving(curBall));
    }

    public override void ResetPlayer()
    {
        base.ResetPlayer();
        _recoverySwingsCounter = 0;
    }

    private IEnumerator Serving(GameObject curBall)
    {
        serving = true;
        inAir = false;
        
        while (serving)
        {
            if (!inAir)
            {
                // replace with mobile input later
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    inAir = true;
                    StartCoroutine(ThrowBall(curBall));
                    print("ballup");
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Tennis.Instance.HitServe(powerT);
                    serving = false;
                    StopAllCoroutines();
                    print("ballserve, strength: " + powerT);
                }
            }

            yield return null;
        }
    }
}
