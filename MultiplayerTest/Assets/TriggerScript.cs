using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
  public bool isOn = false;
  // bewaar reference naar triggermanager zodat je die kan laten weten dat de trigger is geactiveerd
  private TriggerManager _triggerManager;
  private void Start()
  {
    // vind de triggermanager met
    _triggerManager = FindObjectOfType<TriggerManager>();
  }

  // als er iets in de triggerarea komt
  private void OnTriggerEnter(Collider other)
  {
    // check of het de player is
    if (other.CompareTag("Player"))
    {
      isOn = true;
      
    }
  }
}