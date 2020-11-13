using System;
using System.Collections;
using gameHub;
using gameObjects;
using ui;
using UnityEngine;
using Random = UnityEngine.Random;

public class SharedZombie : MonoBehaviour
{
    private int curPlayerID = 0;
    public Player _curPlayer;

    private SwipeInput _mobileInput;

    [Header("1: normal, 2: special")]
    [SerializeField] public int zombieType;

    private bool isSpecial = false;

    [SerializeField] GameObject FirePrefab;
    private bool isOnFire = false;

    [SerializeField] private GameObject FocusIndicator;
    [SerializeField] private Material[] playerColors;

    private Zombie _zombie;
    private SpecialZombie _specialZombie;

    private ZombieAssigner _zombieAssigner;

    public bool dead = false;

    //-----------------------------------------------------------------------new
    private int comboIndex;
    private int startIndex;
    private bool playerStunned = false;
    private KeyCode[] combo;
        
    public bool isFocus;
        
    private float startTimeOfWinningInputs;
    
    public ArrowUI _uiTarget;
    //-----------------------------------------------------------------------

    void Awake()
    {
        switch (zombieType)
        {
            case 1:
                _zombie = gameObject.GetComponent<Zombie>();
                break;
            case 2:
                _specialZombie = gameObject.GetComponent<SpecialZombie>();
                isSpecial = true;
                break;
        }

        _mobileInput = FindObjectOfType<SwipeInput>();
        _zombieAssigner = FindObjectOfType<ZombieAssigner>();
        
        //-----------------------------------------------------------------------new
        _uiTarget = GameObject.FindWithTag("arrowui").GetComponent<ArrowUI>();

        comboIndex = 0;
        startIndex = 0;
        
        FocusIndicator.SetActive(false);
        //-----------------------------------------------------------------------
    }

    public void Spawned()
    {
        _zombieAssigner.ZombieSpawned(this);
    }

    public void Exited(bool defeatedByPlayer)
    {
        _zombieAssigner.ZombieExited(this, defeatedByPlayer);
    }

    //acts as a relay to enable playerscripts to communicate with different zombie scripts
    //yes an inelegant solution as i could probably have done better with subclasses
    public void RelaySetIsFocus(bool inComboRange, Player player)
    {
        if (!dead)
        {
            if (_curPlayer != player && !inComboRange)
            {
                //do nothing, i have a bad feeling about this
            }
            else
            {
                isFocus = inComboRange; //add condition for whether to check whether zombie is not dead
            }

            if (isFocus)
            {
                curPlayerID = player.ID; // here------------------------------------------------------------------------------------------------------------------------------
                _curPlayer = player;

                switch (zombieType)
                {
                    case 1:
                        _zombie.SetCurPlayer(player);
                        break;
                    case 2:
                        _specialZombie.SetCurPlayer(player);
                        break;
                }
                
                FocusIndicator.SetActive(true);
                FocusIndicator.GetComponent<Renderer>().material = playerColors[curPlayerID];
            }
            else
            {
                FocusIndicator.SetActive(false);
            }
        }
    }

    public void RelayCreateUI()
    {
        startTimeOfWinningInputs = Time.time;
        _uiTarget.CreateZombieUI(combo, comboIndex, curPlayerID);
    }

    public void RelayDestroyUI()
    {
        _uiTarget.DestroyComboUI(curPlayerID);
    }

    public void RelayOnFireDamage()
    {
        OnFireDamage();
    }

    public void SpawnFire()
    {
        if (!isOnFire)
        {
            Instantiate(FirePrefab, transform);
            isOnFire = true;
        }
    }
    
    //-----------------------------------------------------------------------new
    public void CreateCombo(int comboLength)
    { 
        combo = new KeyCode[comboLength]; 
        comboIndex = 0;
        
        for (int i = 0; i < combo.Length; i++) 
        { 
            int randomer = Random.Range(0, 4); 
            switch (randomer) 
            { 
                case 0: 
                    combo[i] = KeyCode.LeftArrow; 
                    break;
                case 1: 
                    combo[i] = KeyCode.UpArrow; 
                    break;
                case 2: 
                    combo[i] = KeyCode.RightArrow; 
                    break;
                case 3: 
                    combo[i] = KeyCode.DownArrow; 
                    break;
            }
        }
    }
        
    private void OnComboFail()
    {
        if (!playerStunned)
        {
            
            _uiTarget.DestroyComboUI(curPlayerID);
            comboIndex = 0;
            playerStunned = true;
            _curPlayer.scoreHandler.SubtractComboPoints();
            StartCoroutine(ZombieUnstun());
        }
    }
        
    private IEnumerator ZombieUnstun()
    {
        yield return new WaitForSecondsRealtime(.2f);
        playerStunned = false;
        if (isFocus)
        {
            startTimeOfWinningInputs = Time.time;
            _uiTarget.CreateZombieUI(combo, comboIndex, curPlayerID);
        }
    }
        
    private void OnComboSucceed()
    {
        if (comboIndex < combo.Length)
        {
            _curPlayer.scoreHandler.AddComboPoints(isSpecial);
            _uiTarget.RemoveSingleCombo(curPlayerID);
            comboIndex++;
        }
    }
        
    public void OnFireDamage()
    {
        startIndex++;
    }
    
     public void PlayerInput()
     {
         if (isFocus && !playerStunned)
         {
             _uiTarget.MoveComboUI(transform.position, curPlayerID);

             if (GetLeftInput())
             {
                 if (combo[comboIndex] == KeyCode.LeftArrow)
                 {
                     OnComboSucceed();
                 }
                 else
                 {
                     OnComboFail();
                 }
             }
             else if (GetUpInput())
             {
                 if (combo[comboIndex] == KeyCode.UpArrow)
                 {
                     OnComboSucceed();
                 }
                 else
                 {
                     OnComboFail();
                 }
             }
             else if (GetRightInput())
             {
                 if (combo[comboIndex] == KeyCode.RightArrow)
                 {
                     OnComboSucceed();
                 }
                 else
                 {
                     OnComboFail();
                 }
             }
             else if (GetDownInput())
             {
                 if (combo[comboIndex] == KeyCode.DownArrow)
                 {
                     OnComboSucceed();
                 }
                 else
                 {
                     OnComboFail();
                 }
             }
             if (comboIndex > combo.Length-1)
             {
                 switch (zombieType)
                 {
                     case 1:
                         float tempTotalTime1 = Time.time - startTimeOfWinningInputs;
                         _curPlayer.scoreHandler.AddZombieComboPoints(isSpecial, tempTotalTime1);
                         _uiTarget.DestroyComboUI(curPlayerID);
                         _zombie.OnZombieDefeated();
                         break;
                     case 2:
                         float tempTotalTime2 = Time.time - startTimeOfWinningInputs;
                         _curPlayer.scoreHandler.AddZombieComboPoints(isSpecial, tempTotalTime2);
                         _uiTarget.DestroyComboUI(curPlayerID);
                         _specialZombie.OnZombieDefeated();
                         break;
                 }
             }
         }
         else
         {
             comboIndex = startIndex;
         }
     }

     private bool GetLeftInput()
     {
         bool temp = _mobileInput.GetCurrentDirection() == SwipeInput.Direction.Left;

//         if (curPlayerID == 0)
//         {
//             temp = Input.GetButtonDown("left");
//         }
//         else if (curPlayerID == 1)
//         {
//             temp = Input.GetButtonDown("left2");
//         }

         return temp;
     }
     private bool GetUpInput()
     {
         bool temp = _mobileInput.GetCurrentDirection() == SwipeInput.Direction.Up;
         
//         if (curPlayerID == 0)
//         {
//             temp = Input.GetButtonDown("up");
//         }
//         else if (curPlayerID == 1)
//         {
//             temp = Input.GetButtonDown("up2");
//         }

         return temp;
     }
     private bool GetRightInput()
     {
         bool temp = _mobileInput.GetCurrentDirection() == SwipeInput.Direction.Right;
         
//         if (curPlayerID == 0)
//         {
//             temp = Input.GetButtonDown("right");
//         }
//         else if (curPlayerID == 1)
//         {
//             temp = Input.GetButtonDown("right2");
//         }

         return temp;
     }
     private bool GetDownInput()
     {
         bool temp = _mobileInput.GetCurrentDirection() == SwipeInput.Direction.Down;
         
//         if (curPlayerID == 0)
//         {
//             temp = Input.GetButtonDown("down");
//         }
//         else if (curPlayerID == 1)
//         {
//             temp = Input.GetButtonDown("down2");
//         }

         return temp;
     }
}
