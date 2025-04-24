using System.Linq;
using DefaultNamespace.Items;
using DefaultNamespace.NPC;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    // Interactable Type Definition
    public enum ResourceType { Wood, Stone, Ore, Food }

    public class Interactable : MonoBehaviour
    {
        public int ItemId;
        
        public Item resourceYield;
        public int yieldAmount = 1;
        public float interactionTime = 2f;
        public float health = 1;
    
        [HideInInspector] public bool isDepleted = false;
        
        void Start()
        {
            if (ItemId != null)
            {
                resourceYield = ItemDatabase.Instance.GetItemById(ItemId);
            }
            
            yieldAmount = Random.Range(1, 5);
            health = 2;
        }

        public void Interact(PlayerController player)
        {
            // Base interaction logic
            if (health <= 0)
            {
                GameManager.Instance.Inventory.AddResource(resourceYield, yieldAmount);
                
                isDepleted = true;
                Destroy(gameObject);
            }
        }
        
        public TaskStatus Interact(NpcController npc)
        {
            health -= Time.deltaTime;
            
            // Base interaction logic
            if (health <= 0)
            {
                // TODO: find out if the NPC already has the item
                npc.Inventory.Add(new NpcInventoryItem(resourceYield, yieldAmount));
                
                isDepleted = true;
                Destroy(gameObject);
                return TaskStatus.Success;
            }
            
            return TaskStatus.Running;
        }
    }
    
    public class Tree : Interactable
    {
        void Start()
        {
            resourceYield = ItemDatabase.Instance.GetItemById(0);
            yieldAmount = Random.Range(3, 6);
            health = 150;
        }
    }

    public class Rock : Interactable
    {
        void Start()
        {
            resourceYield = ItemDatabase.Instance.GetItemById(1);
            yieldAmount = Random.Range(2, 4);
            health = 200;
        }
    }
}