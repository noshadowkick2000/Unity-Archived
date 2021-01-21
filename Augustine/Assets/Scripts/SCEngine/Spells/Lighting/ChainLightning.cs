using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCEngine.Effects.Lightning;
using UnityEngine;

namespace SCEngine.Spells.Lighting
{
  public class ChainLightning : BaseSpell
  {
    [SerializeField] private float baseDamage; // Damage to the first hit entity, subsequent enemies are damaged less
    [SerializeField] private float damageAttenuation; // Percentage of damage which the subsequent entity will receive
    [SerializeField] private int delay; // Amount of ticks until lighting strikes the next entity
    [SerializeField] private float range; // Range of the spell
    [SerializeField] private int maxStrikes; // Max amount spell will chain strike enemies
    private int _counter; // How many ticks have passed in between lightning strikes
    private int _strikeCounter; // How many lighting strikes have occured
    private List<Transform> _flaggedTransforms = new List<Transform>(); // Transforms of entities which have already been hit by this spell 
    
    private void Awake()
    {
      InitCooldown();
    }

    public override bool CastSpell()
    {
      if (BasicCoolDown() && _strikeCounter == 0)
      {
        Transform enemyTransform = MousePosition.Instance.GetMouseCharacter();
        if (enemyTransform != null)
        {
          if (GetEnemiesInRange(true).Contains(enemyTransform))
          {
            Strike(enemyTransform);
            GameClock.Instance.Ticked += TimeAndJump;
            return true; 
          }
        }
      }

      return false;
    }

    private Transform[] GetEnemiesInRange(bool firstStrike)
    {
      Vector3 origin;
      if (firstStrike)
      {
        origin = transform.position;
      }
      else
      {
        origin = _flaggedTransforms[_flaggedTransforms.Count - 1].position;
      }
      Collider[] buffer = Physics.OverlapSphere(origin, range, TestingManager.Instance.enemyMask);
      Transform[] output = new Transform[buffer.Length];
      for (int i = 0; i < output.Length; i++)
      {
        output[i] = buffer[i].transform;
      }
      return output;
    }

    private void TimeAndJump()
    {
      if (_strikeCounter == maxStrikes)
      {
        _strikeCounter = 0;
        GameClock.Instance.Ticked -= TimeAndJump;
      }
      else
      {
        _counter++;
        if (_counter == delay)
        {
          _strikeCounter++;
          _counter = 0;
          Transform[] buffer = GetEnemiesInRange(false);

          if (buffer.Length > 0)
          {
            foreach (var enemy in buffer)
            {
              if (!_flaggedTransforms.Contains(enemy))
              {
                Strike(enemy);
                break;
              }
            }
          }
          else
          {
            _strikeCounter = maxStrikes;
          }
        }
      }
    }

    private void Strike(Transform enemyTransform)
    {
      enemyTransform.GetComponent<Health>().Damage(spellElement, baseDamage*Mathf.Pow(damageAttenuation, _strikeCounter));
      enemyTransform.gameObject.AddComponent<Stun>();
      _flaggedTransforms.Add(enemyTransform);
    }

    private void OnDrawGizmos()
    {
      Gizmos.DrawWireSphere(transform.position, range);
    }
  }
}