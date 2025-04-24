using System;
using System.Collections.Generic;
using DefaultNamespace.Items;
using UnityEngine;

[Serializable]
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }
    
    [Tooltip("Assign all your items here via Inspector")]
    public List<Item> items = new ();

    // Optional lookup by ID or name
    private Dictionary<int, Item> itemsById;
    private Dictionary<string, Item> itemsByName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        
        Instance = this;
        
        // Build fast lookups
        itemsById = new Dictionary<int, Item>();
        itemsByName = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (!itemsById.ContainsKey(item.id))
            {
                itemsById.Add(item.id, item);
            }

            if (!itemsByName.ContainsKey(item.itemName))
            {
                itemsByName.Add(item.itemName, item);
            }
        }
    }

    // Public API to fetch items
    public Item GetItemById(int id)
    {
        itemsById.TryGetValue(id, out var item);
        return item;
    }

    public Item GetItemByName(string name)
    {
        itemsByName.TryGetValue(name, out var item);
        return item;
    }
}
