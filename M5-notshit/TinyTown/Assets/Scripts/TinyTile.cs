using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyTile : MonoBehaviour
{
    public enum TileTypes
    {
        House = 0,
        Workplace = 1,
        Recreation = 2,
        None = 3
    }
    
    [SerializeField] private TileTypes tileType;
    [SerializeField] private int tileSize; //length of lot in tiles
    
    //location of tile, 0-tile is at 0, 0, 0
    //all other tiles are added behind in negative z with a space of 10 per tileSize
    //adress denotes order of tiles, not absolute positions
    [SerializeField] private int adress; 
    
    [SerializeField] private Vector3 entryPoint; //point to which the tiny walks before starting tile animations

    private void OnDrawGizmosSelected()
    {
        //draw entryPoint
        Gizmos.DrawCube(entryPoint + transform.position, Vector3.one);
    }

    public int GetSize()
    {
        return tileSize;
    }
}
