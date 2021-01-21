using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SCEngine
{
    public class MousePosition : Singleton<MousePosition>
    {
        /// <summary>
        /// Returns the flattened (y=0) vector3 of the world position of the mouse on screen
        /// Use for when needing to detect general aim and clicking on floors
        /// Returns 0 vector if no ground is found
        /// </summary>
        public Vector3 GetMouseGeneral()
        {
            Vector3 aimPosition = new Vector3();
            
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50, TestingManager.Instance.groundMask))
            {
                aimPosition.x = hit.point.x;
                aimPosition.z = hit.point.z;
            }

            return aimPosition;
        }
        
        /// <summary>
        /// Returns enemy Transform at the mouse position on screen
        /// Use for when needing to detect enemy characters
        /// Returns null if no enemy character is found
        /// </summary>
        public Transform GetMouseCharacter()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50, TestingManager.Instance.enemyMask))
            {
                return hit.transform;
            }

            return null;
        }
    }
}