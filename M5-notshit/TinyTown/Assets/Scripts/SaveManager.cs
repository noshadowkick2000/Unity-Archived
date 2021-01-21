using System;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private WorldManager _worldManager;
    
    public int[] tiles; //id = adress
    
    public string[] names; //holds names
    public int[] identifier; // holds unique id
    //id's are assigned when created and never changed
    //if an id frees up due to deletion, that id will be assigned see also world manager
    public int[] tinyAdress; //holds tiny house adress
    public int[][] bodyParts; //holds bodyparts
    public int[][] stats; //holds statistics
    public int[][] times; //holds times for waking up etc

    public int[][] relations;

    private string _tileSavePath;
    private string _tinySavePath;
    private string _relationSavePath;

    // Start is called before the first frame update
    void Awake()
    {
        _worldManager = FindObjectOfType<WorldManager>();
        
        _tileSavePath = Application.persistentDataPath + "/tiles.txt";
        _tinySavePath = Application.persistentDataPath + "/tiny.txt";
        _relationSavePath = Application.persistentDataPath + "/relation.txt";
        
        try
        {
            //loading tiles
            if (!File.Exists(_tileSavePath))
            {
                //temp, template for debugging: 1 house plot
                tiles = new int[1];
                tiles[0] = 0;

                SaveTiles();
            }
            else
            {
                LoadTiles();
            }
            
            if (!File.Exists(_tinySavePath))
            {
                //temp, can later change to redirect to new game start or something
                names = new string[1] {"Testboi"};
                identifier = new int[1];
                tinyAdress = new int[1];
                bodyParts = new int[1][]; 
                bodyParts[0] = new int[5] {0, 1, 0, 0, 0};
                stats = new int[1][];
                stats[0] = new int[2] {5, 5};
                times = new int[1][]; 
                times[0] = new int[4] {420, 600, 960, 1380};
                _worldManager.CreateTiny(names[0], identifier[0], tinyAdress[0], bodyParts[0], stats[0], times[0]);

                SaveTinies();
            }
            else
            {
                LoadTinies();
                if (names.Length > 1)
                    LoadRelations();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #region TileSaving
    private void LoadTiles()
    {
        string[] tileLoad = File.ReadAllLines(_tileSavePath);

        tiles = tileLoad.Select(i => int.Parse(i)).ToArray();
                
        _worldManager.SetTiles(tiles);
    }

    private void SaveTiles()
    {
        string[] tileSave = tiles.Select(i => i.ToString()).ToArray();
                
        _worldManager.SetTiles(tiles);
                
        File.WriteAllLines(_tileSavePath, tileSave);
    }
    
    #endregion

    #region TinySaving

    private void LoadTinies()
    {
        string[] tinyLoad = File.ReadAllLines(_tinySavePath); //each line is a different tiny, attributes are csv

        names = new string[tinyLoad.Length];
        identifier = new int[tinyLoad.Length];
        tinyAdress = new int[tinyLoad.Length];
        bodyParts = new int[tinyLoad.Length][];
        stats = new int[tinyLoad.Length][];
        times = new int[tinyLoad.Length][];

        for (int i = 0; i < tinyLoad.Length; i++)
        {
            string[] subString = tinyLoad[i].Split(',');
            
            //assign values
            names[i] = subString[0];
            identifier[i] = int.Parse(subString[1]);
            tinyAdress[i] = int.Parse(subString[2]);
            bodyParts[i] = new int[5] {int.Parse(subString[3]), int.Parse(subString[4]), 
                int.Parse(subString[5]), int.Parse(subString[6]), int.Parse(subString[7])};
            stats[i] = new int[2] {int.Parse(subString[8]), int.Parse(subString[9])};
            times[i] = new int[4] {int.Parse(subString[10]), int.Parse(subString[11]), int.Parse(subString[11]), int.Parse(subString[12])};
            
            //create object
            _worldManager.CreateTiny(names[i], identifier[i], tinyAdress[i], bodyParts[i], stats[i], times[i]);
        }
    }

    private void SaveTinies()
    {
        string[] allLines = new string[names.Length];
        
        for (int i = 0; i < names.Length; i++)
        {
            string singleAttributes = names[i] + "," + identifier[i].ToString() + "," + tinyAdress[i].ToString();
            string bodyAttributes = string.Join(",", bodyParts[i]);
            string statAttributes = string.Join(",", stats[i]);
            string timeAttributes = string.Join(",", times[i]);

            allLines[i] = singleAttributes + "," + bodyAttributes + "," + statAttributes + "," + timeAttributes;
            
            //print(writeLine);
        }
        
        File.WriteAllLines(_tinySavePath, allLines);
    }

    #endregion

    #region RelationshipSaving

    private void LoadRelations()
    {
        string[] relationLoad = File.ReadAllLines(_relationSavePath);
        
        relations = new int[relationLoad.Length][];

        for (int i = 0; i < relations.Length; i++)
        {
            string[] subString = relationLoad[i].Split(',');

            relations[i][0] = int.Parse(subString[0]); //id 1
            relations[i][1] = int.Parse(subString[1]); //id 2
            relations[i][2] = int.Parse(subString[2]); //relationship value
        }
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
    }
}
