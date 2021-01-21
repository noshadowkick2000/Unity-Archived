using System;
using System.Collections;
using System.Collections.Generic;
using SCEngine;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace SCEngine
{
    public class TrollMovement : BaseMovement
    {
        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            PerlinScuttle();
        }
    }
}