using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private int gridSize;

    private int _worldLength = 0; //get from file, amount of tiles in world
    private int[][] _relations;
    
    [SerializeField] private GameObject[] tileHolder;
    [SerializeField] private Transform buildingsRoot;
    [SerializeField] private Transform tinyRoot;
    [SerializeField] private GameObject prefabTiny;

    /*void FindTiles() //temp function for preset worlds without being loaded from save
    {
        TinyTile[] temp = FindObjectsOfType<TinyTile>();
        
        foreach (var plot in temp)
        {
            _worldLength += plot.GetSize();
        }
    }*/

    public void SetTiles(int[] tileIDs)
    {
        _worldLength = 0;
        
        foreach (var tileTransform in buildingsRoot.GetComponentsInChildren<Transform>())
        {
            if (tileTransform!=buildingsRoot)
                Destroy(tileTransform.gameObject);
        }

        float prevOffset = 0;
        for (int i = 0; i < tileIDs.Length; i++)
        {
            GameObject temp = Instantiate(tileHolder[i], buildingsRoot);
            temp.transform.position = (new Vector3(0, 0, -prevOffset));
            
            prevOffset = temp.GetComponent<TinyTile>().GetSize() * 10;
            _worldLength += temp.GetComponent<TinyTile>().GetSize();
        }
    }

    public void CreateTiny(string tinyName, int id, int adress, int[] bodyParts, int[] stats, int[] times)
    {
        GameObject temp = Instantiate(prefabTiny, tinyRoot);
        temp.transform.position = RandomLocation();
        Skeleton initObject = temp.GetComponent<Skeleton>();
        initObject.tinyName = tinyName;
        initObject.id = id;
        initObject.homeAdress = adress;
        initObject.bodyParts = bodyParts;
        initObject.stats = stats;
        initObject.times = times;
        initObject.CreateObjects();
    }
    
    public Vector3 RandomLocation() 
    { 
        //get world length from world manager
        //use as range for z
        //use random function for x
        float maxRange = -_worldLength*5;
            
        //minRange is always 5
        float z = Random.Range(maxRange, 5);

        float x = Random.Range(Constants.minimumX, Constants.maximumX);
            
        Vector3 finalPosition = Vector3.zero;
        finalPosition += new Vector3(x, 5, z);
            
        //print(finalPosition);

        return finalPosition;
    }

    public void SetRelations(int[][] relationData)
    {
        _relations = relationData;
    }
}
