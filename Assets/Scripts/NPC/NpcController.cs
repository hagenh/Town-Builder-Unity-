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

        // Basic needs
        public float energy = 100f;
        public float hunger = 0f;
        public float maxEnergy = 100f;
        public float maxHunger = 100f;
        public float energyDecayRate = 5f;
        public float hungerIncreaseRate = 5f;
        public float energyRecoveryRate = 20f;
        public float hungerDecreaseAmount = 40f;

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
            // update needs
            hunger = Mathf.Min(maxHunger, hunger + hungerIncreaseRate * Time.deltaTime);
            if (Tasks.Count == 0 || Tasks.First() is not SleepTask)
            {
                energy = Mathf.Max(0f, energy - energyDecayRate * Time.deltaTime);
            }

            // insert high priority tasks based on needs
            if (energy <= 20f && (Tasks.Count == 0 || Tasks.First() is not SleepTask))
            {
                Tasks.Insert(0, new SleepTask());
            }
            else if (hunger >= 80f && (Tasks.Count == 0 || Tasks.First() is not EatTask))
            {
                Tasks.Insert(0, new EatTask());
            }

            if (!Tasks.Any())
            {
                AddDefaultTask();
            }

            var current = Tasks.First();
            var status  = current.Execute(this);

            if (status == TaskStatus.Success)
            {
                Tasks.Remove(current);
            }
            else if (status == TaskStatus.Failure)
            {
                Tasks.Clear();
            }

            if (!Tasks.Any())
            {
                AddDefaultTask();
            }
        }

        private void AddDefaultTask()
        {
            if (npcJob == NpcJob.Woodcutter)
            {
                Tasks.Add(new ChopTreeTask());
            }
            else if (npcJob == NpcJob.Chicken)
            {
                var newPosition = new Vector2(transform.position.x + Random.Range(-10, 10), transform.position.y + Random.Range(-10, 10));
                Tasks.Add(new MoveToTask(newPosition));
            }
            else
            {
                Tasks.Add(new SocializeTask());
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