using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuPlayer : Player
{
    // Update is called once per frame
    public override void CheckInput()
    {
        if (Mathf.Abs(Tennis.Instance.ball.ballPosition.y - Get2DPosition().y) < .8f)
        {
            HitReturn(Random.Range(.6f, .9f), false);
        }
    }
}
