using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{
    public class TestingManager : Singleton<TestingManager>
    {
        public LayerMask playerMask;
        public LayerMask enemyMask;
        public LayerMask groundMask;
        
        [Serializable]
        private struct Interaction
        {
            public Elemental.Element attacking; // Attacking element
            public Elemental.Element defending; // Defending element
            public float damageFactor; // Factor of the total damage that will be passed
        }

        [SerializeField] private Interaction[] interactions;

        void Start()
        {
            playerMask = LayerMask.GetMask("Player");
            enemyMask = LayerMask.GetMask("Enemy");
            groundMask = LayerMask.GetMask("Ground");
        }

        /// <summary>
        /// Returns adjusted damage taking elemental effects into account
        /// </summary>
        public float ReturnEffectiveDamage(float baseDamage, Elemental.Element attack, Elemental.Element defense)
        {
            if (attack == Elemental.Element.None || defense == Elemental.Element.None)
                return baseDamage;
            else
            {
                foreach (var combination in interactions)
                {
                    if (attack == combination.attacking && defense == combination.defending)
                        return baseDamage * combination.damageFactor;
                }

                // Exception case
                print("Interaction is not set in TestingManager script");
                print(attack + " - " + defense);
                return baseDamage;
            }
        }

        /*/// <summary>
        /// Returns adjusted damage taking elemental effects into account
        /// Use when attack/spell has single element and hit character has multiple elements
        /// </summary>*/
        /*public float ReturnEffectiveDamage(float baseDamage, Elemental.Element attack, Elemental.Element[] defense, float[] percentageDefense)
        {
            if (attack == Elemental.Element.None)
                return baseDamage;
        }*/
    }
}