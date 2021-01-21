using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Testing());
    }

    IEnumerator Testing()
    {
        while (true)
        {
            print(Time.time);
            if (Time.time > 100000)
                break;

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
