using DefaultNamespace.NPC;
using UnityEngine;

public class SleepTask : ITask
{
    public TaskStatus Execute(NpcController npc)
    {
        npc.energy = Mathf.Min(npc.maxEnergy, npc.energy + npc.energyRecoveryRate * Time.deltaTime);

        if (npc.energy >= npc.maxEnergy)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
