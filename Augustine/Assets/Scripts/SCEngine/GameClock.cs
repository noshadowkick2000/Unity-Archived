using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{
    public class GameClock : Singleton<GameClock>
    {
        [SerializeField] private float gameTickInSeconds = .5f; // Amount of seconds it takes for game to count one tick

        public event Action Ticked;
        
        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating(nameof(Tick), 0, gameTickInSeconds);
        }

        private void Tick()
        {
            Ticked?.Invoke();
        }
    }
}