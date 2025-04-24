using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;           // Reference to the Tilemap
    public List<TileBase> grassTile;        // The tile to place (assign in the Inspector)
    public List<TileBase> waterTile;        // Another type of tile

    public int width = 100;            // Width of the map
    public int height = 100;           // Height of the map

    public float noiseOffset = 1000f;
    public float noiseScale = 1f;
    public int updateMapAfterFrames = 0;
    
    void Start()
    {
        GenerateMap();
        noiseOffset = UnityEngine.Random.Range(0, 1000000);
    }

    private void Update()
    {
        if(updateMapAfterFrames > 100)
        {
            GenerateMap();
            updateMapAfterFrames = 0;
            Debug.Log("Regenerating map!");
        }
        else
        {
            updateMapAfterFrames++;
        }
    }

    void GenerateMap()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var noiseValue = Mathf.PerlinNoise(noiseOffset + x * noiseScale, noiseOffset + y * noiseScale);
                // Example: Randomly place tiles
                if (noiseValue >= 0.3f)
                {
                    PlaceGrassTile(x, y);
                }
                else
                {
                    PlaceWaterTile(x, y);
                }
            }
        }
    }
    
    void PlaceGrassTile(int x, int y)
    {
        var grassIndex = UnityEngine.Random.Range(0, grassTile.Count);
        var randomGrassTile = grassTile[grassIndex];
        
        // Set the tile at the current position
        tilemap.SetTile(new Vector3Int(x, y, 0), randomGrassTile);
    }
    
    void PlaceWaterTile(int x, int y)
    {
        var waterIndex = UnityEngine.Random.Range(0, waterTile.Count);
        var randomWaterTile = grassTile[waterIndex];
        
        // Set the tile at the current position
        tilemap.SetTile(new Vector3Int(x, y, 0), randomWaterTile);
        var tile = tilemap.GetTile(new Vector3Int(x, y, 0)).AddComponent<Collider2D>();
    }
}
