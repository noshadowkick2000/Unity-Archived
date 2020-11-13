using System.Collections;
using FMODUnity;
using gameCore;
using gameObjects;
using ui;
using UnityEngine;

namespace gameHub
{
    public class LevelHandler : MonoBehaviour
    {
        [SerializeField] public bool multiPlayerLevel;

        private HealthBar[] _healthBars;

        public int state = 0; // 0: before level start, 1: in level, 2: points grabbing sequence, 3: endscreen

        [SerializeField] private StudioEventEmitter fmodMusic;
        
        [SerializeField] private StudioEventEmitter pauseSound;

        [SerializeField] private WaveHolder[] waves;

        private int _waveHolderIndex;

        private bool _won = false;

        //spawn attributes
        private int _zombieAmount;
        private int _specialZombieAmount;
        private int _minCombo;
        private int _maxCombo;
        private int _minWave;
        private int _maxWave;
        private float _minTime;
        private float _maxTime;
        private float _speed;
        private float _comboInterval;
        private float _waveInterval;

        //zombies
        private int _amountToBeSpawnedZombies;
        private int _amountOfAliveZombies = 0;
        //special zombies
        private int _amountOfAliveSpecialZombies = 1;

        //to set when level starts spawning
        private float _startTime;
        [SerializeField] private float startDelay;
        
        //to set when last intervals were
        private float _lastTimeCombo;
        private float _lastTimeWave;

        //gamecore
        private GameCore _gameCore;
        
        //connects handler to spawner
        [SerializeField] private Spawner spawner;
        [SerializeField] private int maxAliveZombies;

        //difficulty curve
        private int _curMaxCombo;
        private int _curMaxWave;
        
        [SerializeField] public float maxHealth;
        [SerializeField] public int dmgPerAttack;
        [SerializeField] public int perHeal;

        private bool _readyToSpawn = true;

        //ui interface
        private UI _uiTarget;
        public bool pausing = false;
        
        //point grab sequence
        [SerializeField] private float lengthOfPointsGrabSequence;
        private float endTime;
        public bool grabbingPoints = false;

        private Player _player;
        
        private float waitedDelay;

        void Start()
        {
            if (!multiPlayerLevel)
            {
                _waveHolderIndex = GameObject.FindWithTag("gamecore").GetComponent<GameCore>().progress;
            }
            else if (multiPlayerLevel)
            {
                _waveHolderIndex = 0;
            }

            _gameCore = GameObject.FindGameObjectWithTag("gamecore").GetComponent<GameCore>();

            _uiTarget = GameObject.FindGameObjectWithTag("ui").GetComponent<UI>();

            _player = GameObject.FindWithTag("Player").GetComponent<Player>();

            _healthBars = FindObjectsOfType<HealthBar>();

            _zombieAmount = waves[_waveHolderIndex].amountOfEnemies;
            _specialZombieAmount = waves[_waveHolderIndex].amountOfSpecialEnemies;
            _minCombo = waves[_waveHolderIndex].minComboAmount;
            _maxCombo = waves[_waveHolderIndex].maxComboAmount;
            _minWave = waves[_waveHolderIndex].minWaveSize;
            _maxWave = waves[_waveHolderIndex].maxWaveSize;
            _minTime = waves[_waveHolderIndex].minTimeBetweenSpawns;
            _maxTime = waves[_waveHolderIndex].maxTimeBetweenSpawns;
            _speed = waves[_waveHolderIndex].zombieSpeed;
            _comboInterval = waves[_waveHolderIndex].intervalComboUp;
            _waveInterval = waves[_waveHolderIndex].intervalWaveUp;

            _amountToBeSpawnedZombies = _zombieAmount;

            _maxCombo += 1;
            _curMaxCombo = _minCombo;
            _curMaxWave = _minWave;

            _startTime = Time.time;
            _lastTimeCombo = _startTime;
            _lastTimeWave = _startTime;

            StartCoroutine(StartLevel());
        }

        private IEnumerator StartLevel()
        {
            yield return new WaitForSecondsRealtime(startDelay);
            state = 1;
        }

        private void Update()
        {
            PauseGameHandler();
            if (!pausing)
            {
                if (state==1)
                {
                    SpawnHandler();
                    SetFmodParameters();
                    ShortcutSkip();
                    WinCondition();

                    if (_curMaxCombo != _maxCombo)
                    {
                        if (Time.time > _lastTimeCombo + _comboInterval)
                        {
                            _curMaxCombo++;
                            _lastTimeCombo += _comboInterval;
                        }
                    }

                    if (_curMaxWave != _maxWave)
                    {
                        if (Time.time > _lastTimeWave + _waveInterval)
                        {
                            _curMaxWave++;
                            _lastTimeWave += _waveInterval;
                        }
                    }
                }
                else if (state == 2)
                {
                    PointsGrabSequence();
                }
                else if (state==3)
                {
                    LevelOverHandler();
                }
            }
        }

        private void ShortcutSkip()
        {
            if (Input.GetKeyDown(KeyCode.F12))
            {
                state = 3;
                _uiTarget.EndScreen(false);
                fmodMusic.SetParameter("State", 2, true);
            }
        }

        private IEnumerator Spawner()
        {
            float waitTime = Random.Range(_minTime, _maxTime);

            int comboAmount = Random.Range(_minCombo, _curMaxCombo);

            yield return new WaitForSecondsRealtime(waitTime);
                
            if (!pausing)
            {
                for (int i = 0; i < _curMaxWave; i++)
                {
                    if (_amountOfAliveZombies < maxAliveZombies)
                    {
                        spawner.spawnZombie(comboAmount, _speed);
                        _amountOfAliveZombies++;
                        _amountToBeSpawnedZombies--;
                    }
                }

                if (_zombieAmount - _amountToBeSpawnedZombies > (_zombieAmount / _specialZombieAmount) * _amountOfAliveSpecialZombies)
                {
                    int specialComboAmount = 2 * _minCombo + 2;
                    float specialSpeed = _speed * 1.8f;
                    spawner.spawnSpecialZombie(specialComboAmount, specialSpeed);
                    _amountOfAliveSpecialZombies++;
                    print("specialZombie");
                }
            }

            _readyToSpawn = true;
        }

        private void SpawnHandler()
        {
            if (!pausing)
            {
                if (_amountToBeSpawnedZombies > 0 && _readyToSpawn)
                {
                    _readyToSpawn = false;
                    StartCoroutine(Spawner());
                }
            }
        }

        private void PointsGrabSequence()
        {
            if (grabbingPoints)
            {
                string stringTime = Mathf.RoundToInt(endTime - Time.time).ToString();
                _uiTarget.AnimateCountDown(stringTime);
                if (Time.time > endTime)
                {
                    state = 3;
                    _uiTarget.EndScreen(true);
                    fmodMusic.SetParameter("State", 1, true);
                    waitedDelay = Time.time + 2;
                }
            }
        }

        private IEnumerator SpawnBigZombie()
        {
            spawner.spawnBigZombie();
            yield return new WaitForSeconds(lengthOfPointsGrabSequence*.1f);
            grabbingPoints = true;
            endTime = Time.time + lengthOfPointsGrabSequence;
            _player.bigZombieTime = true;

        }

        private void LevelOverHandler()
        {
            if (Time.time > waitedDelay)
            {
                if (Input.GetButtonDown("down"))
                {
                    if (!multiPlayerLevel)
                    {
                        if (_won)
                            _gameCore.ProgressToNextLevel();
                        _gameCore.ChangeScene(0);
                    }
                    else if (multiPlayerLevel)
                    {
                        _gameCore.ChangeScene(0);
                    }
                }
            }
        }

        private void WinCondition()
        {
            if (_amountToBeSpawnedZombies <= 0 && _amountOfAliveZombies <= 0)
            {
                state = 2;
                _won = true;
                StartCoroutine(SpawnBigZombie());
                fmodMusic.SetParameter("health", 0, true);
            }
            else if (multiPlayerLevel)
            {
                bool end = false;
                
                for (int i = 0; i < _healthBars.Length; i++)
                {
                    if (_healthBars[i].curHealth <= 0)
                        end = true;
                }
                
                if (end)
                {
                    state = 3;
                    _uiTarget.EndScreen(false);
                    fmodMusic.SetParameter("State", 2, true);
                }
            }
            else if (!multiPlayerLevel)
            {
                bool end = false;
                
                for (int i = 0; i < _healthBars.Length; i++)
                {
                    if (_healthBars[i].curHealth <= 0)
                        end = true;
                }
                
                if (end)
                {
                    waitedDelay = Time.time + 2;
                    state = 3;
                    _uiTarget.EndScreen(false);
                    fmodMusic.SetParameter("State", 2, true);
                }
            }
        }

        public void lowerAmountOfAliveZombies()
        {
            _amountOfAliveZombies--;
        }

        private void SetFmodParameters()
        {
            float lowestHealth = maxHealth;
            int lowestID = 0;
            
            for (int i = 0; i < _healthBars.Length; i++)
            {
                if (_healthBars[i].curHealth < lowestHealth)
                {
                    lowestHealth = _healthBars[i].curHealth;
                    lowestID = i;
                }
            }
            
            fmodMusic.SetParameter("Amount of Zombies", _amountOfAliveZombies, true);
            fmodMusic.SetParameter("Health", _healthBars[lowestID].curHealth/maxHealth, true);
        }

        private void PauseGameHandler()
        {
            if (state == 1 || state == 2)
            {
                if (Input.GetButtonDown("pause"))
                {
                    Time.timeScale = pausing ? 1 : 0;
                    pausing = !pausing;
                    for (int i = 0; i < _healthBars.Length; i++)
                    {
                        _healthBars[i].pausing = pausing;
                    }
                    if (pausing)
                        fmodMusic.SetParameter("Volume", 0, true);
                    else
                        fmodMusic.SetParameter("Volume", 1, true);
                    _uiTarget.Pause(pausing);
                    pauseSound.SetParameter("PauseSound", 1);
                    pauseSound.SetParameter("PauseSound", 0);
                }
            }
        }
    }
}