using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using SCEngine.Spells;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SCEngine
{

    // Listens to input from player and casts associated spell
    public class InputToSpell : MonoBehaviour
    {
        private AugustineControls _input;
        
        private BaseSpell[] _attachedSpells; // Stores ref to all assigned spells in order of input

        private void Awake()
        {
            _input = new AugustineControls();
            
            Init();
        }

        void Init()
        {
            GetAttachedSpells();
            AssignSpells();
        }

        void CastSpell(int spell)
        {
            if (spell < _attachedSpells.Length)
            {
                if(_attachedSpells[spell].CastSpell())
                    print("Casted spell " + _attachedSpells[spell].spellName);
            }
            else
            {
                print("Spell not assigned");
            }
        }

        void Term()
        {
            _input.Player.Disable();
            
            // Dunno if necessary
            /*_input.Player.Spell0.performed -= spl0 => CastSpell(_attachedSpells[0]);
            _input.Player.Spell1.performed -= spl0 => CastSpell(_attachedSpells[1]);
            _input.Player.Spell2.performed -= spl0 => CastSpell(_attachedSpells[2]);
            _input.Player.Spell3.performed -= spl0 => CastSpell(_attachedSpells[3]);*/
        }

        private void GetAttachedSpells()
        {
            _attachedSpells = GetComponents<BaseSpell>();
        }

        private void AssignSpells()
        {
            // TODO: Sort _attachedSpells using player input for mappings
            
            _input.Player.Enable();
            
            _input.Player.Spell0.performed += spl0 => CastSpell(0);
            _input.Player.Spell1.performed += spl0 => CastSpell(1);
            _input.Player.Spell2.performed += spl0 => CastSpell(2);
            _input.Player.Spell3.performed += spl0 => CastSpell(3);
            
            string debug = "Spells in inventory:\n";
            foreach (var spell in _attachedSpells)
            {
                debug += "-" + spell.spellName + "\n";
            }
            print(debug);
        }

        private void OnDestroy()
        {
            Term();
        }
    }
}