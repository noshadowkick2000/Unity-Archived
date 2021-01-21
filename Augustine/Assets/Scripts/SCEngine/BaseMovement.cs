using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{
    public class BaseMovement : MonoBehaviour
    {
        protected Vector3 BasePosition;
        protected bool Active = true;

        protected void Init()
        {
            BasePosition = transform.position;
            GameClock.Instance.Ticked += ResetActive;
        }

        protected void ResetPosition()
        {
            BasePosition = transform.position;
        }

        private void ResetActive()
        {
            Active = true;
        }

        /// <summary>
        /// Damages attached character
        /// </summary>
        /// <param name="stop">True if movement needs to be disabled</param>
        public void LockMovement(bool stop)
        {
            Active = !stop;
            BasePosition = transform.position;
        }
        
        protected void PerlinScuttle()
        {
            if (Active)
                transform.position = BasePosition + new Vector3( Mathf.PerlinNoise(Time.time, 0) - .5f ,0, Mathf.PerlinNoise(Time.time * 3, 0) - .5f);
        }

        private void OnDestroy()
        {
            GameClock.Instance.Ticked -= ResetActive;
        }
    }
}