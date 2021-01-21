using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SCEngine
{
    public class DebugDisplayer : MonoBehaviour
    {
        private Health _health;
        private TextMeshPro _textMeshPro;

        private void Start()
        {
            GameClock.Instance.Ticked += UpdateText;
            _health = GetComponentInParent<Health>();
            _textMeshPro = GetComponent<TextMeshPro>();
        }

        void UpdateText()
        {
            _textMeshPro.text = _health.MHealth.ToString();
        }
    }
}