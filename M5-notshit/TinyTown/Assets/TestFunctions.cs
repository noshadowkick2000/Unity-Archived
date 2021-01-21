using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFunctions : MonoBehaviour
{
    [SerializeField] private GameObject tinyPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnTiny()
    {
        Instantiate(tinyPrefab, transform);
    }
}
