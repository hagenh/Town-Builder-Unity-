using System;
using UnityEngine;

namespace DefaultNamespace.Items
{
    public enum ItemType
    {
        Consumable,
        Weapon,
        Armor,
        Quest,
        Misc
    }

    [Serializable]
    public class Item
    {
        public string itemName;
    
        [TextArea(2,5)]
        public string description;

        public ItemType itemType;

        public Sprite icon;         // for UI
        public GameObject prefab;   // the world/GameObject representation

        public int id;

        public Item(int id, string name, string desc, ItemType type, Sprite icon, GameObject prefab)
        {
            this.id = id;
            itemName = name;
            description = desc;
            itemType = type;
            this.icon = icon;
            this.prefab = prefab;
        }
    }
}