using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace.NPC
{
    public class ChopTreeTask : ITask
    {
        private GameObject _tree;
        private float threshold = 0.2f;

        private bool _isCloseEnoughToTree = false;
        
        public TaskStatus Execute(NpcController npc)
        {
            if (_tree.IsUnityNull())
            {
                _tree = FindTree(npc);
            }

            // Not close enough to the tree. Add MoveToTask to get close
            if (!IsCloseEnoughToTree(npc))
            {
                var position = _tree.transform.position.ToVector2() + new Vector2(0, -0.75f);
                var movetoTask = new MoveToTask(position);
                var tasks = new List<ITask>() { movetoTask }.Concat(npc.Tasks).ToList();
                npc.Tasks = tasks;
                
                return TaskStatus.Running;
            }
            
            var status = _tree.GetComponent<Interactable>().Interact(npc);
            if (status == TaskStatus.Success)
            {
                return status;
            }
            
            return TaskStatus.Running;
        }

        [CanBeNull]
        GameObject FindTree(NpcController npc)
        {
            var trees = GameObject.FindGameObjectsWithTag("Tree");
            var gameObjectInRadius = trees
                .OrderBy(x => Vector3.Distance(x.transform.position, npc.transform.position)).ToArray();

            if (!gameObjectInRadius.Any())
            {
                return null;
            }
            
            return gameObjectInRadius.First();
        }

        public bool IsCloseEnoughToTree(NpcController npc)
        {
            var treeCollider = npc.GetComponent<Collider2D>();
            
            ContactFilter2D filter = new ContactFilter2D();
            filter.NoFilter(); // Accept all collisions

            Collider2D[] results = new Collider2D[2]; // Adjust size as needed

            int count = treeCollider.Overlap(filter, results);

            for (int i = 0; i < count; i++)
            {
                if (results[i] != null)
                {
                    // Check if this collider belongs to the target GameObject
                    if (results[i].gameObject == _tree || results[i].transform.root.gameObject == _tree)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}