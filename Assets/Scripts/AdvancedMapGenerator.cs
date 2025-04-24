using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.NPC;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class AdvancedMapGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Tilemap tilemap;
    public List<TileBase> grassTiles;
    public List<TileBase> waterTiles;
    public List<GameObject> decorations;

    [Header("Generation Settings")]
    public float noiseScale = 0.1f;
    public int renderDistance = 5;
    public int chunkSize = 3;

    [Header("Biome Settings")]
    public float lakeThreshold = 0.3f;
    public float riverThreshold = 0.8f;
    public float grassVariationScale = 50f;
    
    private const float TreeThreshold = 0.03f;
    private const float StoneThreshold = 0.01f;

    private Vector2Int _currentPlayerChunk;
    private readonly HashSet<Vector2Int> loadedChunks = new();
    private float _seed;
    
    private GameObject _treePrefab;
    private GameObject _stonePrefab;

    [Header("Transitions")]
    public TileBase topLeftTransition;
    public TileBase topTransition;
    public TileBase topRightTransition;
    public TileBase leftTransition;
    public TileBase rightTransition;
    public TileBase bottomLeftTransition;
    public TileBase bottomTransition;
    public TileBase bottomRightTransition;
    public TileBase surroundedTopLeftTransition;
    public TileBase surroundedTopRightTransition;
    public TileBase surroundedBottomLeftTransition;
    public TileBase surroundedBottomRightTransition;
    
    public GameObject textMeshPrefab;

    private Dictionary<Vector3Int, TileType> _tileTypeMap = new();
    private List<GameObject> _decorationInstances = new();
    
    public List<NpcController> npcs;

    private bool _hasRan = false;

    void Start()
    {
        _seed = Random.Range(0, 1000000);
        
        _treePrefab = Resources.Load<GameObject>("TreeInteractable Variant");
        _stonePrefab = Resources.Load<GameObject>("StoneInteractable");
        
        UpdateChunks();
    }

    void Update()
    {
        Vector2Int playerChunk = GetChunkPosition(player.position);
        if(playerChunk != _currentPlayerChunk)
        {
            _currentPlayerChunk = playerChunk;
            UpdateChunks();
        }
    }
    
    void DisplayTileType(int x, int y, TileType tileType)
    {
        var position = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
        
        if(position.x < player.position.x - renderDistance || position.x > player.position.x + renderDistance ||
           position.y < player.position.y - renderDistance || position.y > player.position.y + renderDistance)
        {
            return;
        }
        
        var textMeshInstance = Instantiate(textMeshPrefab, position, Quaternion.identity);
        var textMesh = textMeshInstance.GetComponent<TextMeshPro>();
        textMesh.text = tileType.ToString();
        textMesh.sortingOrder = 10;
        _decorationInstances.Add(textMeshInstance);

        for (int i = 0; i < _decorationInstances.Count; i++)
        {
            if (_decorationInstances[i].transform.position.x < player.position.x - renderDistance || _decorationInstances[i].transform.position.x > player.position.x + renderDistance ||
                _decorationInstances[i].transform.position.y < player.position.y - renderDistance || _decorationInstances[i].transform.position.y > player.position.y + renderDistance)
            {
                Destroy(_decorationInstances[i]);
                _decorationInstances.RemoveAt(i);
            }
        }
    }

    void AddWaterTransitions(int startX, int startY)
    {
        for (int x = startX; x < startX + chunkSize; x++)
        {
            for (int y = startY; y < startY + chunkSize; y++)
            {
                DisplayTileType(x, y, _tileTypeMap[new Vector3Int(x, y, 0)]);
                
                var currentTile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (!currentTile || _tileTypeMap[new Vector3Int(x, y, 0)] == TileType.Water)
                {
                    continue;
                }

                var topLeft = _tileTypeMap.GetValueOrDefault(new Vector3Int(x - 1, y + 1, 0), TileType.Grass);
                var top = _tileTypeMap.GetValueOrDefault(new Vector3Int(x, y + 1, 0), TileType.Grass);
                var topRight = _tileTypeMap.GetValueOrDefault(new Vector3Int(x + 1, y + 1, 0), TileType.Grass);
                var left = _tileTypeMap.GetValueOrDefault(new Vector3Int(x - 1, y, 0), TileType.Grass);
                var right = _tileTypeMap.GetValueOrDefault(new Vector3Int(x + 1, y, 0), TileType.Grass);
                var bottomLeft = _tileTypeMap.GetValueOrDefault(new Vector3Int(x - 1, y - 1, 0), TileType.Grass);
                var bottom = _tileTypeMap.GetValueOrDefault(new Vector3Int(x, y - 1, 0), TileType.Grass);
                var bottomRight = _tileTypeMap.GetValueOrDefault(new Vector3Int(x + 1, y - 1, 0), TileType.Grass);

                if (IsSurroundedOnSidesByNonWater(top, left, right, bottom))
                {
                    if (bottomRight == TileType.Water)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), topLeftTransition);
                        _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                    }
                    else if (bottomLeft == TileType.Water)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), topRightTransition);
                        _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                    }
                    else if (topRight == TileType.Water)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), bottomLeftTransition);
                        _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                    }
                    else if (topLeft == TileType.Water)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), bottomRightTransition);
                        _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                    }
                }
                else if (right == TileType.Water && bottomRight == TileType.Water && bottom == TileType.Water && topLeft == TileType.Grass)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), surroundedTopLeftTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (left == TileType.Water && bottomLeft == TileType.Water && bottom == TileType.Water && topRight == TileType.Grass)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), surroundedTopRightTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (top == TileType.Water && topRight == TileType.Water && right == TileType.Water && bottomLeft == TileType.Grass)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), surroundedBottomLeftTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (top == TileType.Water && topLeft == TileType.Water && left == TileType.Water && bottomRight == TileType.Grass)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), surroundedBottomRightTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (top == TileType.Grass && IsNonWater(left) && IsNonWater(right) && bottom == TileType.Water)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), topTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (IsNonWater(top) && left == TileType.Grass && IsNonWater(bottom) && right == TileType.Water)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), leftTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (IsNonWater(top) && right == TileType.Grass && IsNonWater(bottom) && left == TileType.Water)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), rightTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
                else if (IsNonWater(left) && IsNonWater(right) && bottom == TileType.Grass && top == TileType.Water)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), bottomTransition);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Transition;
                }
            }
        }
    }

    bool IsSurroundedOnSidesByNonWater(TileType top, TileType left, TileType right, TileType bottom)
    {
        return IsNonWater(top) && IsNonWater(left) && IsNonWater(right) && IsNonWater(bottom);
    }
    
    bool IsNonWater(TileType tileType)
    {
        return tileType is TileType.Grass or TileType.Transition;
    }

    void UpdateChunks()
    {
        HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();

        for(int x = -renderDistance; x <= renderDistance; x++)
        {
            for(int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int chunk = new Vector2Int(
                    _currentPlayerChunk.x + x,
                    _currentPlayerChunk.y + y
                );

                chunksToKeep.Add(chunk);

                if(!loadedChunks.Contains(chunk))
                {
                    GenerateChunk(chunk);
                    loadedChunks.Add(chunk);
                }
            }
        }

        // Remove old chunks (optional)
        // foreach(Vector2Int chunk in new HashSet<Vector2Int>(loadedChunks))
        // {
        //     if(!chunksToKeep.Contains(chunk))
        //     {
        //         ClearChunk(chunk);
        //         loadedChunks.Remove(chunk);
        //     }
        // }
    }

    Vector2Int GetChunkPosition(Vector3 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.y / chunkSize)
        );
    }

    void GenerateChunk(Vector2Int chunk)
    {
        int startX = chunk.x * chunkSize;
        int startY = chunk.y * chunkSize;

        for(int x = startX; x < startX + chunkSize; x++)
        {
            for(int y = startY; y < startY + chunkSize; y++)
            {
                // Base terrain noise
                float baseNoise = Mathf.PerlinNoise(
                    (x + _seed) * noiseScale, 
                    (y + _seed) * noiseScale
                );

                // Water features
                float lakeNoise = Mathf.PerlinNoise(
                    (x + _seed + 1000) * 0.05f, 
                    (y + _seed + 1000) * 0.05f
                );

                float riverNoise = Mathf.PerlinNoise(
                    (x + _seed + 2000) * 0.1f, 
                    (y + _seed + 2000) * 0.1f
                );

                // Grass variation
                float grassPattern = Mathf.PerlinNoise(
                    x * grassVariationScale + _seed,
                    y * grassVariationScale + _seed
                );
                
                var npcSpawningChance = Random.Range(0f, 1000f);
                if (npcSpawningChance > 999f)
                {
                    Instantiate()
                }

                if(lakeNoise < lakeThreshold)
                {
                    PlaceWaterTile(x, y);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Water;
                }
                else if(riverNoise > riverThreshold)
                {
                    PlaceWaterTile(x, y);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Water;
                }
                else if(baseNoise > 0.3f)
                {
                    PlaceGrassTile(x, y);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Grass;
                }
                else
                {
                    PlaceWaterTile(x, y);
                    _tileTypeMap[new Vector3Int(x, y, 0)] = TileType.Water;
                }
            }
        }

        // Adds transitions between water and grass
        AddWaterTransitions(startX, startY);

        // Add decorations after lands, water, and transitions are placed
        for (int x = startX; x < startX + chunkSize; x++)
        {
            for (int y = startY; y < startY + chunkSize; y++)
            {
                if (PlaceTree(x, y)) { continue; }
                if (PlaceStone(x, y)) { continue; }
                if (PlaceDecoration(x, y)) { continue; }
            }
        }
    }

    void PlaceGrassTile(int x, int y)
    {
        var rnd = Random.Range(0f, 1f);
        TileBase selectedTile;
        if(rnd < 0.90f)
        {
            selectedTile = grassTiles[0];
        }
        else if(rnd < 0.95f)
        {
            selectedTile = grassTiles[1];
        }
        else
        {
            selectedTile = grassTiles[2];
        }
        
        tilemap.SetTile(new Vector3Int(x, y, 0), selectedTile);
    }

    void PlaceWaterTile(int x, int y)
    {
        var waterIndex = Random.Range(0, waterTiles.Count);
        tilemap.SetTile(new Vector3Int(x, y, 0), waterTiles[waterIndex]);
    }

    // Optional chunk clearing method
    void ClearChunk(Vector2Int chunk)
    {
        int startX = chunk.x * chunkSize;
        int startY = chunk.y * chunkSize;

        for(int x = startX; x < startX + chunkSize; x++)
        {
            for(int y = startY; y < startY + chunkSize; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }
    
    private bool PlaceTree(int x, int y)
    {
        var treeNoise = Random.Range(0f, 1f);
        if(treeNoise < TreeThreshold && _tileTypeMap[new Vector3Int(x, y, 0)] == TileType.Grass)
        {
            var position = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            Instantiate(_treePrefab, position + new Vector3(0, 1.5f, 0), Quaternion.identity);
            return true;
        }

        return false;
    }
    
    private bool PlaceStone(int x, int y)
    {
        var stoneNoise = Random.Range(0f, 1f);

        if(stoneNoise < StoneThreshold && _tileTypeMap[new Vector3Int(x, y, 0)] == TileType.Grass)
        {
            var position = tilemap.CellToWorld(new Vector3Int(x, y, 0)) + new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
            Instantiate(_stonePrefab, position, Quaternion.identity);
            return true;
        }

        return false;
    }
    
    private bool PlaceDecoration(int x, int y)
    {
        var decorationNoise = Random.Range(0f, 1f);
        
        if(decorationNoise < 0.1f && _tileTypeMap[new Vector3Int(x, y, 0)] == TileType.Grass)
        {
            var decorationIndex = Random.Range(0, decorations.Count);
            Instantiate(decorations[decorationIndex], new Vector3(x, y, 0), Quaternion.identity);
            return true;
        }

        return false;
    }
    
}

public enum TileType
{
    Grass,
    Water,
    Transition
}