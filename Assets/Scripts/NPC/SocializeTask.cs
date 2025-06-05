using DefaultNamespace.NPC;
using UnityEngine;
using Random = UnityEngine.Random;

public class SocializeTask : ITask
{
    private MoveToTask moveTask;
    private float socializeDuration = 2f;
    private float startTime = -1f;

    public TaskStatus Execute(NpcController npc)
    {
        if (moveTask == null)
        {
            var offset = new Vector2(Random.Range(-3f, 3f), Random.Range(-3f, 3f));
            moveTask = new MoveToTask((Vector2)npc.transform.position + offset);
        }

        var status = moveTask.Execute(npc);
        if (status != TaskStatus.Success)
        {
            return status;
        }

        if (startTime < 0f)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime >= socializeDuration)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
}
