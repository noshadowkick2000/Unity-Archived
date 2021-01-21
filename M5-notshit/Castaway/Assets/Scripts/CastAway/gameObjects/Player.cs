using System;
using System.Collections;
using System.Collections.Generic;
using gameHub;
using menuHub;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


namespace gameObjects
{
    public class Player : MonoBehaviour
    {
        [SerializeField] public int ID;
        //[SerializeField] private Player otherPlayer;

        //[SerializeField] private bool multiplayerLevel;
        private int zombiesInRange = 0;
        //private List<SharedZombie> zombiesInRangeList = new List<SharedZombie>();

        [FMODUnity.EventRef]
        [SerializeField] public string walkRef;
        FMOD.Studio.EventInstance walkSound;

        [SerializeField] public float walkingSpeed;
        
        [SerializeField] private float speed;
        [SerializeField] private float rotSpeed;
        [SerializeField] private float attackDuration;
        [SerializeField] private GameObject oarWeapon;
        private Rigidbody _rb;
        private Animator _animator;
        private float _horizontal;
        private float _vertical;

        public bool bigZombieTime = false;

        public SharedZombie FocusZombie;

        [SerializeField] private Vector3 bigZombieSpawnPosition;

        //private bool stunned = false;

        public bool zombieHasFocus = false;

        private float CameraRotation;

        private bool attacking;

        private bool joyStickZero = false;

        [SerializeField] private GameObject levelHandlerObjects;
        public HealthBar healthBar;
        public ScoreHandler scoreHandler;

        // Start is called before the first frame update
        void Awake()
        {
            _rb = this.GetComponent<Rigidbody>();
            _animator = this.GetComponent<Animator>();
            oarWeapon.SetActive(false);
            walkSound = FMODUnity.RuntimeManager.CreateInstance(walkRef);
            walkSound.setParameterByName("WalkSpeed", walkingSpeed, true);
            if (levelHandlerObjects != null)
            {
                healthBar = levelHandlerObjects.GetComponentInChildren<HealthBar>();
                healthBar.player = gameObject;
                scoreHandler = levelHandlerObjects.GetComponentInChildren<ScoreHandler>();
            }

            OnLevelLoad();
        }

        private void OnLevelLoad()
        {
            zombieHasFocus = false;
            zombiesInRange = 0;
            attacking = false;
            bigZombieTime = false;
            _animator.SetBool("inRange", false);
        }

        /*private void Start()
        {
            SetCameraOffset();
        }*/

        public void MovePlayer(float horizontal, float vertical)
        {
            Vector3 addVector = new Vector3(horizontal * speed * Time.deltaTime, 0, vertical * speed * Time.deltaTime);
            addVector = Quaternion.Euler(0, CameraRotation, 0) * addVector;
            _rb.MovePosition(transform.position + addVector);
        }
        private void SetRotation()
        {
            if (!attacking)
            {
                if (bigZombieTime)
                {
                    Vector3 direction = transform.position - bigZombieSpawnPosition;
                    direction.y = 0;
                    direction.Normalize();
                    Quaternion rotategoal = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    _rb.rotation = (rotategoal);
                }
                else if (!zombieHasFocus)
                {
                    _rb.rotation = _rb.rotation * Quaternion.Euler(0, -CameraRotation, 0);

                    float angle = Mathf.Atan2(_horizontal, _vertical);
                    angle = angle * Mathf.Rad2Deg;
                    Quaternion rotateGoal = Quaternion.Euler(0.0f, angle + 180, 0.0f);
                    rotateGoal = Quaternion.Slerp(_rb.rotation, rotateGoal, rotSpeed);
                    if (_horizontal != 0 || _vertical != 0)
                    {
                        _rb.MoveRotation(rotateGoal);
                    }

                    _rb.rotation = _rb.rotation * Quaternion.Euler(0, CameraRotation, 0);
                }
                else if (zombieHasFocus)
                {
                    Vector3 direction = transform.position - FocusZombie.transform.position;
                    direction.y = 0;
                    direction.Normalize();
                    Quaternion rotategoal = Quaternion.LookRotation(direction.normalized, Vector3.up);
                    _rb.rotation = (rotategoal);
                }
            }
        }

        private void WalkSound()
        {
            if (walkSound.isValid())
            {
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(walkSound, transform, _rb);
                FMOD.Studio.PLAYBACK_STATE playbackState;
                walkSound.getPlaybackState(out playbackState);
                if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    walkSound.start();
                }
            }
        }
        
        private void Update()
        {
            SetCameraOffset();
            GetInput();
            SetAnimation();
        }
        private void FixedUpdate()
        {
            MovePlayer(_horizontal, _vertical);
            SetRotation();
        }

        private void SetAnimation()
        {
            if (!joyStickZero)
            {
                _animator.SetBool("walking", true);
                WalkSound();
            }
            else
            {
                _animator.SetBool("walking", false);
                walkSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            if (zombieHasFocus)
            {
                walkSound.setParameterByName("WalkSpeed", 2, true);
                _animator.SetBool("inRange", true);
            }
            else
            {
                walkSound.setParameterByName("WalkSpeed", walkingSpeed, true);
                _animator.SetBool("inRange", false); 
            }
        }

        private void SetCameraOffset()
        {
            CameraRotation = Camera.main.transform.localEulerAngles.y;
        }

        public void AttackAnimation()
        {
            //tempsolution as animations repeat when bool is used for transition from any state
            _animator.SetBool("attacking", true);
            _animator.SetTrigger("attack");
            oarWeapon.SetActive(true);
            oarWeapon.GetComponent<Animator>().SetTrigger("attack");
            attacking = true;
            StartCoroutine(SuccesfulAttack());
        }

        private IEnumerator SuccesfulAttack()
        {
            yield return new WaitForSecondsRealtime(attackDuration);
            _animator.SetBool("attacking", false);
            oarWeapon.SetActive(false);
            attacking = false;
        }

        public void HurtAnimation()
        {
            _animator.SetTrigger("hurt");
        }

        private void GetInput()
        {
            if (ID == 0)
            {
                _horizontal = Input.GetAxis("Horizontal");
                _vertical = Input.GetAxis("Vertical");

                if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                    joyStickZero = false;
                else
                    joyStickZero = true;
            }

            if (ID == 1)
            {
                _horizontal = Input.GetAxis("Horizontal2");
                _vertical = Input.GetAxis("Vertical2");

                if (Input.GetAxisRaw("Horizontal2") != 0 || Input.GetAxisRaw("Vertical2") != 0)
                    joyStickZero = false;
                else
                    joyStickZero = true;
            }
        }

        public Vector3 GetPlayerPosition()
        {
            return transform.position;
        }

        private void OnDestroy()
        {
            walkSound.release();
        }
    }
}