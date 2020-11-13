using System.Collections;
using gameHub;
using ui;
using UnityEngine;
using Random = UnityEngine.Random;

//tempsolution for hiding ui: setting position outside of screenview in prefab. Later change to set active

namespace gameObjects
{
    public class SpecialZombie : MonoBehaviour
    {
        [FMODUnity.EventRef] 
        [SerializeField] public string deathSound;
        
        [FMODUnity.EventRef] 
        [SerializeField] public string zombieDamages;
        
        [FMODUnity.EventRef] 
        [SerializeField] public string walkRef;
        FMOD.Studio.EventInstance walkSound;

        private Rigidbody _rb;
        
        [SerializeField] private float damageDistance;

        [SerializeField] private float walkSpeed;
        
        [SerializeField] private float endTargetRange;
        [SerializeField] private float playerFollowRange;
        private Vector3 correctedSpawnPosition;
        private Vector3 direction;
        private Vector3 targetLocation;
        private Vector3 endTargetLocation;

        private bool isFollowingPlayer = false;

        private int comboLength;
        
        private bool active = false;
        private bool dead = false;
        private float initTime;
        private float startTime;
        private Rigidbody rb;
        
        private bool inDamageRange = false;
        [SerializeField] private float startDelay;
        [SerializeField] private float yOffset;
        private float speed; //gets from wavehandler
        [SerializeField] private float rotSpeed;
        [SerializeField] private float damageInterval;
        [SerializeField] private float deathDuration;
        private bool waitingToDamage = false;

        private Animator _animator;
        
        //connected scripts
        private LevelHandler _levelHandler;
        //private Player _playerScript;
        //private Player _player2Script;
        private Player[] _players;
        private Player _curPlayer;
        private SharedZombie _sharedZombie;

        public void setValues(int length, float zombiespeed, Vector3 goal)
        {
            correctedSpawnPosition = transform.position + new Vector3(0, yOffset, 0);
            endTargetLocation = goal + new Vector3(0, yOffset, 0);
            targetLocation = endTargetLocation;
            comboLength = length;
            speed = zombiespeed;
        }

        private void Awake()
        {
            //_playerScript = GameObject.FindWithTag("Player").GetComponent<Player>();
            //if (multiplayerLevel)
                //_player2Script = GameObject.FindWithTag("Player2").GetComponent<Player>();
            _levelHandler = GameObject.FindWithTag("levelhandler").GetComponent<LevelHandler>();
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _sharedZombie = GetComponent<SharedZombie>();
            _players = FindObjectsOfType<Player>();
        }

        void Start()
        {
            _sharedZombie.CreateCombo(comboLength);

            initTime = Time.time;
            startTime = initTime + startDelay;
            
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            GetComponent<Collider>().enabled = false;

            SetInitRotation();
            
            walkSound = FMODUnity.RuntimeManager.CreateInstance(walkRef);
            walkSound.setParameterByName("WalkSpeed", walkSpeed, true);

            StartCoroutine(StartActivity());
        }

        private void SetInitRotation()
        {
            direction = targetLocation - rb.transform.position;
            direction.y = 0;

            Quaternion rotategoal = Quaternion.LookRotation(-direction, Vector3.up);

            rb.MoveRotation(rotategoal);
        }
        
        private IEnumerator StartActivity()
        {
            yield return new WaitForSecondsRealtime(startDelay);
            active = true;
            GetComponent<Collider>().enabled = true;
            rb.isKinematic = false;
            walkSound.start();
            _animator.SetBool("walking", true);
            _sharedZombie.Spawned();
        }
        // Update is called once per frame
        void Update()
        {
            if (active)
            {
                CalcFollowPlayer();
                _sharedZombie.PlayerInput();
                SetDamageRange();
                DamagePlayer();
                CheckIfReachedEndTarget();
            }
        }

        void CalcFollowPlayer()
        {
            //if (!multiplayerLevel)
                isFollowingPlayer = Vector3.Distance(transform.position, GetClosestPlayer().transform.position) < playerFollowRange;
            //else
                //isFollowingPlayer = Vector3.Distance(transform.position, _playerScript.transform.position) < playerFollowRange || Vector3.Distance(transform.position, _player2Script.transform.position) < playerFollowRange;
        }

        private void FixedUpdate()
        {
            if (active)
            {
                MoveToTarget();
            }
            else if (!active && !dead)
            {
                rb.MovePosition(Vector3.Lerp(rb.position, correctedSpawnPosition, 1/(startDelay*(startTime-initTime))));
            }
        }

        private void DamagePlayer()
        {
            if (inDamageRange && !waitingToDamage)
            {
                waitingToDamage = true;
                FMODUnity.RuntimeManager.PlayOneShotAttached(zombieDamages, gameObject);
                //int temp = GetClosestPlayer().ID;
                GetClosestPlayer().healthBar.TakeDamage(); //still need to add multiplayer
                if (_curPlayer == null)
                    _curPlayer = GetClosestPlayer();
                _curPlayer.HurtAnimation();
                //if (temp == 1)
                    //_playerScript.HurtAnimation();
                //else if (temp == 2)
                    //_player2Script.HurtAnimation(); // multiplayer
                StartCoroutine(WaitInterval());
            }
        }
        private IEnumerator WaitInterval()
        {
            yield return new WaitForSecondsRealtime(damageInterval);
            waitingToDamage = false;
        }
        private void MoveToTarget()
        {
            if (!inDamageRange)
            {
                if (isFollowingPlayer)
                {
                    targetLocation = GetClosestPlayer().GetPlayerPosition();
                }
                else
                {
                    targetLocation = endTargetLocation;
                }
                
                direction = targetLocation - rb.transform.position;
                direction.y = 0;
                direction.Normalize();
                direction = direction * speed;
                rb.MovePosition(rb.position + direction);
                
                Quaternion rotategoal = Quaternion.LookRotation(-direction, Vector3.up);
                rotategoal = Quaternion.Slerp(rb.rotation, rotategoal, rotSpeed);
                
                rb.MoveRotation(rotategoal);
            }
        }
        
        private Player GetClosestPlayer()
        {
            int closestPlayerI = 0;
            float closestDistance = 1000;

            for (int i = 0; i < _players.Length; i++)
            {
                float curDistance = Vector3.Distance(_players[i].transform.position, transform.position);
                
                if (curDistance < closestDistance)
                {
                    closestDistance = curDistance;
                    closestPlayerI = i;
                }
            }

            return _players[closestPlayerI];
            
            /*if (multiplayerLevel)
            {
                if (Vector3.Distance(_playerScript.transform.position, transform.position) <
                    Vector3.Distance(_player2Script.transform.position, transform.position))
                {
                    return _playerScript;
                }
                else
                {
                    return _player2Script;
                }
            }
            else
            {
                return _playerScript;
            }*/
        }

        private void CheckIfReachedEndTarget()
        {
            if ((Vector3.Distance(transform.position, endTargetLocation)<endTargetRange) && !_sharedZombie.isFocus)
                OnZombieDisappears();
            if (_levelHandler.state == 2)
            {
                OnZombieDisappears();
            }
        }

        private void OnZombieDisappears()
        {
            dead = true;
            active = false;

            if (_sharedZombie.isFocus)
                _sharedZombie._uiTarget.DestroyComboUI(_curPlayer.ID);
            
            _sharedZombie.Exited(false);
            
            GetComponent<Collider>().enabled = false;

            StartCoroutine(AnimationDelay());
            StartCoroutine(DeathDelay());
        }

        public void OnZombieDefeated()
        {
            dead = true;
            active = false;
            
            _curPlayer.healthBar.Heal();
            
            FMODUnity.RuntimeManager.PlayOneShotAttached(deathSound, gameObject);
            
            _curPlayer.AttackAnimation();
            
            _sharedZombie.Exited(true);
            
            /*_curPlayer.SetInInputRange(false, gameObject.GetComponent<SharedZombie>());
            for (int i = 0; i < _players.Length; i++)
            {
                if (_curPlayer!=_players[i])
                    _curPlayer.SetInInputRange(false, gameObject.GetComponent<SharedZombie>());
            }*/
            
            /*if (_curPlayerID == 0)
                _playerScript.AttackAnimation();
            else if (_curPlayerID == 1)
                _player2Script.AttackAnimation();
            if (!multiplayerLevel)
                _playerScript.SetInInputRange(false, gameObject.GetComponent<SharedZombie>());
            else if (multiplayerLevel)
            {
                if (_curPlayerID == 0)
                {
                    _playerScript.SetInInputRange(false, gameObject.GetComponent<SharedZombie>());
                    _player2Script.SetInInputRange(false, gameObject.GetComponent<SharedZombie>()); //multiplayer
                }
                else if (_curPlayerID == 1)
                {
                    _player2Script.SetInInputRange(false, gameObject.GetComponent<SharedZombie>());
                    _playerScript.SetInInputRange(false, gameObject.GetComponent<SharedZombie>());
                }
            }*/

            GetComponent<Collider>().enabled = false;

            StartCoroutine(AnimationDelay());
            StartCoroutine(DeathDelay());
        }
        
        private IEnumerator AnimationDelay()
        {
            yield return new WaitForSecondsRealtime(deathDuration/4);
            _animator.SetTrigger("die");
        }

        private IEnumerator DeathDelay()
        {
            yield return new WaitForSecondsRealtime(deathDuration);
            _sharedZombie.dead = true;
            walkSound.release();
            Destroy(gameObject);
        }
        
        private bool CheckDamageRange()
        {
            if (Vector3.Distance(GetClosestPlayer().transform.position, transform.position) < damageDistance)
                return true;
            else
                return false;
        }


        private void SetDamageRange()
        {
            bool rangeCheck = CheckDamageRange();

            if (inDamageRange != rangeCheck)
            {
                inDamageRange = rangeCheck;

                if (inDamageRange)
                {
                    walkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    _animator.SetBool("walking", false);
                }
                else
                {
                    if (walkSound.isValid())
                    {
                        FMODUnity.RuntimeManager.AttachInstanceToGameObject(walkSound, transform, rb);
                        FMOD.Studio.PLAYBACK_STATE playbackState;
                        walkSound.getPlaybackState(out playbackState);
                        if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPING)
                        {
                            walkSound.start();
                        }
                    }

                    _animator.SetBool("walking", true);
                }
            }
        }

        public void SetFollowPlayer(bool value)
        {
            isFollowingPlayer = value;
        }
        
        public void SetCurPlayer(Player curPlayerScript)
        {
            _curPlayer = curPlayerScript;
        }
    }
}