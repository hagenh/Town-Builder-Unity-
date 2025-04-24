using System.Linq;
using System.Numerics;
using DefaultNamespace;
using DefaultNamespace.NPC;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MoveToTask : ITask
{
    private readonly Vector2 _target;
    private readonly float _arrivalThreshold;
    
    private AStarNode[] _path;
    private AStarNode _currentTarget;

    public MoveToTask(Vector2 target, float threshold = 0.1f)
    {
        _target = target;
        _arrivalThreshold = threshold;
    }

    public TaskStatus Execute(NpcController npc)
    {
        if (Vector2.Distance(npc.transform.position.ToVector2(), _target) <= _arrivalThreshold)
        {
            return TaskStatus.Success;
        }

        if (_path.IsUnityNull())
        {
            var aStar = new AStarAlgorithm(npc.transform.position.ToVector2(), _target);
            _path = aStar.FindPath().ToArray();

            if (_path.IsUnityNull() || _path.Length == 0)
            {
                return TaskStatus.Failure;    
            }
            
            _currentTarget = _path.FirstOrDefault();
        }
        
        if (Vector2.Distance(npc.transform.position.ToVector2(), _currentTarget.Position) <= _arrivalThreshold)
        {
            var indexOfNext = _path.ToList().IndexOf(_currentTarget) + 1;

            if (indexOfNext == _path.Length)
            {
                return TaskStatus.Success;
            }
            
            _currentTarget = _path[indexOfNext];
        }
        
        var moveTowards = Vector2.MoveTowards(npc.transform.position, _currentTarget.Position, npc.movementSpeed * Time.deltaTime);
        npc.transform.position = moveTowards;

        return TaskStatus.Running;
    }
}