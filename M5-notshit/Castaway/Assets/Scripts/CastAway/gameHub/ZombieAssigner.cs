using System;
using System.Collections.Generic;
using System.Linq;
using gameObjects;
using UnityEngine;

namespace gameHub
{
    public class ZombieAssigner : MonoBehaviour
    {
        private Player[] _players;
        private ZombiesHolder[] _assignedZombies;
        private List<ZombiesHolder>[] _zombiesHolders;

        [SerializeField] private float inputRange;

        private void Awake()
        {
            _players = FindObjectsOfType<Player>(); //prob should make sure that all scripts are in sorted order
            //_players = temp.Reverse().ToArray();
            _zombiesHolders = new List<ZombiesHolder>[_players.Length];
            for (int i = 0; i<_zombiesHolders.Length; i++)
                _zombiesHolders[i] = new List<ZombiesHolder>();
            _assignedZombies = new ZombiesHolder[_players.Length];
        }

        public void ZombieSpawned(SharedZombie newZombie)
        {
            for (int i = 0; i < _zombiesHolders.Length; i++)
            {
                ZombiesHolder temp = new ZombiesHolder();
                _zombiesHolders[i].Add(temp);
                temp.SharedZombie = newZombie;
            }
        }

        public void ZombieExited(SharedZombie deadZombie, bool defeatedByPlayer)
        {
            if (!defeatedByPlayer)
            {
                for (int i = 0; i < _zombiesHolders.Length; i++)
                { 
                    for (int f = 0; f < _zombiesHolders[i].Count; f++) 
                    {
                        if (_zombiesHolders[i][f].SharedZombie == deadZombie)
                            _zombiesHolders[i].RemoveAt(f);
                    }
                    _zombiesHolders[i].Sort();
                }
            }
            else
            {
                int temp = deadZombie._curPlayer.ID;
                
                //this works for some reason
                if (_assignedZombies[temp] != null)
                {
                    _assignedZombies[temp].SharedZombie.RelaySetIsFocus(false, _players[temp]);
                    _assignedZombies[temp].SharedZombie.RelayDestroyUI();
                }

                _players[temp].zombieHasFocus = false;

                for (int i = 0; i < _zombiesHolders.Length; i++)
                {
                    for (int f = 0; f < _zombiesHolders[i].Count; f++)
                    {
                        if (_zombiesHolders[i][f].SharedZombie == deadZombie)
                        {
                            _zombiesHolders[i].RemoveAt(f);
                            _assignedZombies[i] = null;
                        }
                        _zombiesHolders[i].Sort();
                    }
                }
            }
        }

        private void Update()
        {
            CalcDistances();
            DeAssignZombies();
            AssignZombies();
        }

        private void CalcDistances()
        {
            for (int i = 0; i < _zombiesHolders.Length; i++)
            { 
                for (int f = 0; f < _zombiesHolders[i].Count; f++) 
                {
                    float p2zDistance = Vector3.Distance(_players[i].transform.position, 
                        _zombiesHolders[i][f].SharedZombie.transform.position);

                    _zombiesHolders[i][f].Distance = p2zDistance;
                }
                
                _zombiesHolders[i].Sort();
            }
        }

        private void DeAssignZombies()
        {
            for (int i = 0; i < _assignedZombies.Length; i++)
            {
                if (_assignedZombies[i] != null)
                {
                    if (_assignedZombies[i].Distance > inputRange)
                    {
                        _assignedZombies[i].SharedZombie.RelaySetIsFocus(false, _players[i]);
                        _assignedZombies[i].SharedZombie.RelayDestroyUI();
                        _players[i].zombieHasFocus = false;
                        _assignedZombies[i] = null;
                        print("deassigned");
                    }
                }
            }
        }

        private void AssignZombies()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                if (!_players[i].zombieHasFocus)
                {
                    for (int f = 0; f < _zombiesHolders[i].Count; f++)
                    {
                        if (_zombiesHolders[i][f].Distance <= inputRange)
                        {
                            if (!_zombiesHolders[i][f].SharedZombie.isFocus)
                            {
                                _assignedZombies[i] = _zombiesHolders[i][f];
                                _assignedZombies[i].SharedZombie.RelaySetIsFocus(true, _players[i]);
                                _assignedZombies[i].SharedZombie.RelayCreateUI();
                                _players[i].FocusZombie = _assignedZombies[i].SharedZombie;
                                _players[i].zombieHasFocus = true;
                                print("assigned");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}