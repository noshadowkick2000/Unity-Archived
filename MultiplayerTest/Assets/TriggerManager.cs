using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
  // Array met al je triggers, sleep elke trigger die je heb in de inspector hier naar toe
  [SerializeField] private TriggerScript[] _triggers;

  // Check elke keer dat er een trigger aan gaat of je een paar hebt die aan staat 
  public void UpdateTriggers()
  {
    /*
     if _triggers[x].isOn && _trigers[y].isOn
    {
      roep de functie van wat je wil laten doen bijv MoveIsland()
    }
    */
  }
  
  // Alles hieronder wordt aangeroepen in de if statements van UpdateTriggers

  private void MoveIsland()
  {
    // doe spul
  }

  private void IlluminatePath()
  {
    // doe spul
  }
}