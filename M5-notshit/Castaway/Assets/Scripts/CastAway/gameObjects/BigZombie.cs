using System.Collections;
using gameHub;
using ui;
using UnityEngine;
using Random = UnityEngine.Random;

//tempsolution for hiding ui: setting position outside of screenview in prefab. Later change to set active

namespace gameObjects
{
    public class BigZombie : MonoBehaviour
    {

        [FMODUnity.EventRef] 
        [SerializeField] public string deathSound;

        private int comboLength = 5;
        
        private Rigidbody rb;
        
        private Player _playerScript;
        
        [SerializeField] private int startDelay;
        [SerializeField] private float deathDuration;
        private bool waitingToDamage = false;
        
        //connected scripts
        private LevelHandler _levelHandler;
        private ScoreHandler _scoreHandler;    
        private ArrowUI _uiTarget;
        
        private KeyCode[] combo;
        private int comboIndex; //to get current combo object

        private float startTimeOfWinningInputs;

        private void Awake()
        {
            _levelHandler = GameObject.FindWithTag("levelhandler").GetComponent<LevelHandler>();
            _uiTarget = GameObject.FindWithTag("arrowui").GetComponent<ArrowUI>();
            _scoreHandler = GameObject.FindWithTag("scorehandler").GetComponent<ScoreHandler>();
            _playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();

            comboIndex = 0;
        }

        void Start()
        {
            CreateCombo();

            rb = GetComponent<Rigidbody>();

            StartCoroutine(StartActivity());
        }

        private void CreateCombo()
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
        private IEnumerator StartActivity()
        {
            yield return new WaitForSecondsRealtime(startDelay);
            rb.isKinematic = false;
            _uiTarget.CreateZombieUI(combo, comboIndex, 0);
        }
        // Update is called once per frame
        void Update()
        {
            if (_levelHandler.state == 2 && _levelHandler.grabbingPoints)
            {
                PlayerInput();
            }
            else if (_levelHandler.state == 3)
            {
                _uiTarget.DestroyComboUI(0);
                GetComponent<Animator>().SetTrigger("die");
                StartCoroutine(DeathDelay());
            }
        }

        private void OnComboSucceed()
        {
            if (comboIndex < comboLength)
            {
                _uiTarget.RemoveSingleCombo(0);
                comboIndex++;
            }
        }

        private IEnumerator DeathDelay()
        {
            rb.useGravity = false;
            yield return new WaitForSecondsRealtime(deathDuration);
            Destroy(gameObject);
        }

        private void PlayerInput()
        { 
            _uiTarget.MoveComboUI(transform.position, 0);

            if (Input.GetButtonDown("left")) 
            {
                
                if (combo[comboIndex] == KeyCode.LeftArrow)
                { 
                    OnComboSucceed();
                }
            }
            else if (Input.GetButtonDown("up")) 
            { 
                if (combo[comboIndex] == KeyCode.UpArrow) 
                { 
                    OnComboSucceed();
                }
            }
            else if (Input.GetButtonDown("right")) 
            { 
                if (combo[comboIndex] == KeyCode.RightArrow) 
                { 
                    OnComboSucceed();
                }
            }
            else if (Input.GetButtonDown("down")) 
            { 
                if (combo[comboIndex] == KeyCode.DownArrow) 
                { 
                    OnComboSucceed();
                }
            }
            
            if (comboIndex > combo.Length-1)
            {
                _playerScript.AttackAnimation();
                FMODUnity.RuntimeManager.PlayOneShotAttached(deathSound, gameObject);
                _scoreHandler.AddComboPoints(true);
                _uiTarget.DestroyComboUI(0);
                CreateCombo();
                _uiTarget.CreateZombieUI(combo, comboIndex, 0);
            }
        }
    }
}