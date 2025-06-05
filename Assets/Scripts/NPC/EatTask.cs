using System.Linq;
using DefaultNamespace.Items;
using DefaultNamespace.NPC;
using UnityEngine;

public class EatTask : ITask
{
    public TaskStatus Execute(NpcController npc)
    {
        var food = npc.Inventory.FirstOrDefault(i => i.Item.itemType == ItemType.Consumable);
        if (food == null)
        {
            return TaskStatus.Failure;
        }

        food.Amount--;
        if (food.Amount <= 0)
        {
            npc.Inventory.Remove(food);
        }

        npc.hunger = Mathf.Max(0f, npc.hunger - npc.hungerDecreaseAmount);
        return TaskStatus.Success;
    }
}
