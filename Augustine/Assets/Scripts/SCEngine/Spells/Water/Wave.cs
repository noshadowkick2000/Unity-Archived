using System.Collections;
using System.Collections.Generic;
using SCEngine.Spells;
using UnityEngine;

namespace SCEngine.Spells.Water
{
    public class Wave : BaseSpell
    {
        private void Awake()
        {
            InitCooldown();
        }

        public override bool CastSpell()
        {
            if (BasicCoolDown())
            {
                SingleShot(0,(MousePosition.Instance.GetMouseGeneral()-transform.position).normalized, MousePosition.Instance.GetMouseGeneral());
                return true;
            }

            return false;
        }
    }
}