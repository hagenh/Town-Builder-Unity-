using DefaultNamespace.NPC;

public enum TaskStatus { Running, Success, Failure }

public interface ITask
{
    /// <summary>
    /// Called each Update by the NPC’s TaskRunner.
    /// </summary>
    TaskStatus Execute(NpcController npc);
}