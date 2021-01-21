using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Characters
{
    public class Skeleton : MonoBehaviour
    { 
        public enum Customizables
        { 
            Face = 0,
            Hair = 1,
            Body = 2,
            Hands = 3,
            Feet = 4
        }
        
        public enum States
        {
            Idling = 0,
            Wandering = 1,
            InAir = 2,
            Editing = 3
        }
        
        public enum StatSave
        {
            Social = 0,
            Fun = 0,
        }

        public enum TimesSave
        {
            WakeUp = 0,
            Sleep = 1,
            StartWork = 2,
            StopWork = 3
        }
        
        //all limbs
        [SerializeField] private GameObject head;
        [SerializeField] private GameObject hair;
        [SerializeField] private GameObject torso;
        [SerializeField] private GameObject leftHand;
        [SerializeField] private GameObject rightHand;
        [SerializeField] private GameObject leftFeet;
        [SerializeField] private GameObject rightFeet;
        
        //all the customization per body part
        [SerializeField] private CustomHolder[] customHolders;

        //private bool _editing;

        //saved attributes
        public string tinyName; //name
        public int id; //unique static id
        public int homeAdress; //int of tile in worldmanager: idea worldmanager loads tile locations for each tile
        public int[] bodyParts; //ids of all bodyparts
        public int[] stats; //see statSave enum
        public int[] times; //see timesSave enum
        
        /*public int socialParam; //scale to 10 how likely tiny is to interact with other tiny when in range
        public int funParam; //scale to 10 how likely tiny is to interact with recreational building when in range
        public int wakeUp; //time at which tiny leaves house in morning in seconds
        public int sleep; //time at which tiny goes home to sleep at night
        public int startWork; //time at which tiny leaves for work
        public int stopWork; //time at which tiny goes home*/

        public int focusLimb;
        
        private States _state = States.Idling;
        [SerializeField] private float minIdlingTime;
        [SerializeField] private float maxIdlingTime;
        //[SerializeField] private float maxWanderingRange;

        private float _curWalkSpeed;

        private Collider[] _buffer = new Collider[3];
        private LayerMask _activitiesMask;

        private LayerMask _groundMask;

        private Vector3 _goalWayPoint;
        
        private float _timer;
        
        //connected modules
        //private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private WorldManager _worldManager;
        private InGameTime _inGameTime;
        private CameraController _cameraController;
        private GameObject _activeUi;

        private void Awake()
        {
            //initialize connected modules
            //_navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _inGameTime = FindObjectOfType<InGameTime>();
            _worldManager = FindObjectOfType<WorldManager>();
            _cameraController = FindObjectOfType<CameraController>();
            _activeUi = GameObject.FindWithTag("tinyeditor");
            
            _activitiesMask = LayerMask.GetMask("Tiny", "Rec");
            _groundMask = LayerMask.GetMask("Walkable");
        }

        // Start is called before the first frame update
        void Start()
        {
            SetState(States.InAir);
            
            _activeUi.SetActive(false);
        }
        
        public void CreateObjects() //call last in setup
        {
            //instantiate all customized objects
            Instantiate(customHolders[(int) Customizables.Hair].objects[bodyParts[(int) Customizables.Hair]], hair.transform);
            Instantiate(customHolders[(int) Customizables.Face].objects[bodyParts[(int) Customizables.Face]], head.transform);
            
            Instantiate(customHolders[(int) Customizables.Body].objects[bodyParts[(int) Customizables.Body]], torso.transform);
            
            Instantiate(customHolders[(int) Customizables.Hands].objects[bodyParts[(int) Customizables.Hands]], leftHand.transform);
            Instantiate(customHolders[(int) Customizables.Hands].objects[bodyParts[(int) Customizables.Hands]], rightHand.transform);
            
            Instantiate(customHolders[(int) Customizables.Feet].objects[bodyParts[(int) Customizables.Feet]], leftFeet.transform);
            Instantiate(customHolders[(int) Customizables.Feet].objects[bodyParts[(int) Customizables.Feet]], rightFeet.transform);
        }
        
        public void EditMode()
        {
            print("editing");
            //_editing = true;
            
            _activeUi.SetActive(true);
            
            SetState(Skeleton.States.Editing);
            _activeUi.GetComponentInChildren<InputField>().text = tinyName;
        }
        
        public void FreeMode()
        {
            print("free");
            //_editing = false;
            
            //saves new tinyName
            tinyName = _activeUi.GetComponentInChildren<InputField>().text;
            PlayerPrefs.SetString(CharacterSaves.name, tinyName);
            PlayerPrefs.Save();
            
            _cameraController.SetState(CameraController.States.FreeLook);
            SetState(Skeleton.States.Idling);
            
            _activeUi.SetActive(false);
        }
        
        public void CycleObjectNext()
        {
            int newId = bodyParts[focusLimb] + 1;
            if (newId == customHolders[focusLimb].objects.Length)
            {
                newId = 0;
            }

            bodyParts[focusLimb] = newId;

            ChangeObject((Customizables)focusLimb);
        }
        
        public void CycleObjectPrevious()
        {
            int newId = bodyParts[focusLimb] - 1;
            if (newId == -1)
            {
                newId = customHolders[focusLimb].objects.Length - 1;
            }

            bodyParts[focusLimb] = newId;
            
            ChangeObject((Customizables) focusLimb);
        }

        public void SetFocus(int target)
        {
            focusLimb = target;
            CycleObjectNext();
        }

        private void ChangeObject(Customizables target)
        {
            switch (target)
            {
                case Customizables.Hair:
                    Destroy(hair.transform.GetChild(0).gameObject);
                    Instantiate(customHolders[(int) Customizables.Hair].objects[bodyParts[(int) Customizables.Hair]], hair.transform);
                    break;
            }
        }

        public void SetState(States nextState)
        {
            _state = nextState;
            
            switch (_state)
            {
                case States.Idling:
                    _animator.SetTrigger("idle");
                    _timer = Time.time + Random.Range(minIdlingTime, maxIdlingTime);
                    break;
                case States.Wandering:
                    //state exits when agent reaches goal
                    _animator.SetBool("walk", true);
                    _goalWayPoint = _worldManager.RandomLocation();
                    _curWalkSpeed = Random.Range(Constants.minSpeed, Constants.maxSpeed);
                    transform.LookAt(new Vector3(_goalWayPoint.x, transform.position.y, _goalWayPoint.z));
                    break;
                case States.InAir:
                    _animator.SetTrigger("inair");
                    break;
                case States.Editing:
                    _animator.SetBool("idle", false);
                    _goalWayPoint = transform.position;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            switch (_state)
            {
                case States.Idling:
                    Idling();
                    break;
                case States.Wandering:
                    Wandering();
                    break;
                case States.InAir:
                    InAiring();
                    break;
                case States.Editing:
                    Editing();
                    break;
            }
        }

        private void InAiring()
        {
            transform.position -= new Vector3(0, .04f, 0);
            if (Physics.Raycast(transform.position, Vector3.down, Constants.tinyHalfHeight, _groundMask))
            {
                SetState(States.Idling);
            }
        }

        private void Idling()
        {
            if (Time.time > _timer && !ScanActivities())
            {
                SetState(States.Wandering);
            }
        }

        private void Wandering()
        {
            RaycastHit hit;
            float newY = 0;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, _groundMask))
            {
                newY = transform.position.y - (hit.distance - Constants.tinyHalfHeight);
            }
            
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(_goalWayPoint.x, newY, _goalWayPoint.z), _curWalkSpeed);

            if (_goalWayPoint.x == transform.position.x && _goalWayPoint.z == transform.position.z && !ScanActivities())
            {
                SetState(States.Idling);
            }
        }

        private bool ScanActivities()
        {
            int curTime = _inGameTime.GetTime();
            
            if (curTime >= times[(int)TimesSave.WakeUp] && curTime < times[(int)TimesSave.StartWork])
            {
                //leavehome
                //setstate
                return true;
            }
            else if (curTime >= times[(int)TimesSave.StartWork] && curTime < times[(int)TimesSave.StopWork])
            {
                //gotowork
                //setstate
                return true;
            }
            else if (curTime >= times[(int)TimesSave.StopWork] && curTime < times[(int)TimesSave.Sleep])
            {
                //exitwork
                //setstate
                return true;
            }
            else if (curTime >= times[(int)TimesSave.Sleep] && curTime < times[(int)TimesSave.WakeUp])
            {
                //gohome
                //setstate
                return true;
            }

            int count = Physics.OverlapBoxNonAlloc(transform.position, Vector3.one, _buffer, Quaternion.identity, _activitiesMask);
            
            for (int i = 0; i < count; i++)
            {
                if (_buffer[i].gameObject != gameObject)
                {
                    //get type of activity and use random and stats to determine whether to do
                    //if doing then return true;
                }
            }

            return false;
        }

        private void Editing()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,270,0), 10);
        }
    }
}