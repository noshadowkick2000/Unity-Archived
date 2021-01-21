using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCEngine
{

    public class Elemental : MonoBehaviour
    {
        public enum Element
        {
            Fire,
            Water,
            Earth,
            Lightning,
            None
        }

        public Element characterElement;
    }
}