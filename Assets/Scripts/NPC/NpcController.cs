using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Items;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace DefaultNamespace.NPC
{
    public class NpcController : MonoBehaviour
    {
        public float movementSpeed = 0.01f;
        
        public List<ITask> Tasks = new List<ITask>();
        public NpcJob npcJob;
        
        public List<NpcInventoryItem> Inventory = new List<NpcInventoryItem>();

        private void Start()
        {
            if (npcJob == NpcJob.Woodcutter)
            {
                Tasks.Add(new ChopTreeTask());
            }
            else
            {
                var newPosition = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
                Tasks.Add(new MoveToTask(newPosition));
            }
            
        }

        private void Update()
        {
            if(npcJob == NpcJob.Woodcutter)
            {
                Debug.Log("Woodcutter job: " + Tasks.First().GetType().Name + " | " + Tasks.Count);
            }
            
            if (Tasks.Count == 0)
            {
                return;
            }
            
            var current = Tasks.First();
            var status  = current.Execute(this);

            if (status == TaskStatus.Success)
            {
                if (npcJob == NpcJob.Woodcutter)
                {
                    Tasks.Remove(current);
                    if (!Tasks.Any())
                    {
                        Tasks.Add(new ChopTreeTask());
                    }
                }
                else if (npcJob == NpcJob.Chicken && !Tasks.Any())
                {
                    Tasks.Remove(current);

                    if (!Tasks.Any())
                    {
                        var newPosition = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
                        Tasks.Add(new MoveToTask(newPosition));
                    }
                }
            }
            else if (status == TaskStatus.Failure)
            {
                Tasks.Clear();
            }
        }
    }
}

public class NpcInventoryItem
{
    public Item Item;
    public int Amount;
    
    public NpcInventoryItem(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}