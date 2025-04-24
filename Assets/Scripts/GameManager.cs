using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Items;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public InventorySystem Inventory { get; private set; }
    [CanBeNull] public Item CurrentItem { get; set; }

    private void Awake()
    {
        // If another instance already exists, destroy this one:
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // initialize your systems here
        Inventory = new InventorySystem();
        
        Instance.Inventory.Resources.Add(ItemDatabase.Instance.GetItemById(3), 1);
    }
}

public class GridCell
{
    public Vector2Int Position { get; }
    public bool IsWalkable { get; }
    public GameObject ObjectInCell { get; }
    public TileBase GroundTile { get; }

    public GridCell(Vector2Int position, bool isWalkable, GameObject objectInCell, TileBase groundTile)
    {
        Position = position;
        IsWalkable = isWalkable;
        ObjectInCell = objectInCell;
        GroundTile = groundTile;
    }
}

// Inventory System
public class InventorySystem
{
    public readonly Dictionary<Item, int> Resources = new();

    public void AddResource(Item type, int amount)
    {
        Debug.Log("ContainsKey: " + Resources.ContainsKey(type));
        Debug.Log("Resource: " + type.itemName + ", amount: " + amount);
        if(Resources.TryGetValue(type, out var currentAmount))
        {
            Resources[type] = currentAmount + amount;
        }
        else
        {
            Resources.Add(type, amount);
        }
        
        Debug.Log("Added " + amount + " " + type.itemName);
    }

    public bool HasResources(Dictionary<Item, int> cost)
    {
        foreach (var resourceCost in cost)
        {
            if(Resources.TryGetValue(resourceCost.Key, out var amount) && amount >= resourceCost.Value)
            {
                Debug.Log("Has enough " + resourceCost.Key.itemName);
                continue;
            }
            
            Debug.Log("Not enough " + resourceCost.Key.itemName);
            return false;
        }
        
        return true;
    }

    public void RemoveResources(Dictionary<Item, int> cost)
    {
        foreach (var kv in cost)
        {
            Debug.Log("Removed " + kv.Value + " " + kv.Key.itemName);
            Resources[kv.Key] -= kv.Value;
        }
    }
}
