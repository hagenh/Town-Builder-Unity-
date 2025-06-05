using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AStarAlgorithm
{
    public List<AStarNode> OpenList = new();
    public List<AStarNode> ClosedList = new();
    
    public AStarNode StartNode;
    public AStarNode TargetNode;
    
    public AStarAlgorithm(Vector2 start, Vector2 target)
    {
        var (startNode, targetNode) = GetAStarNodesForStartAndTarget(start, target);
        StartNode = startNode;
        TargetNode = targetNode;
    }
    
    public (AStarNode start, AStarNode target) GetAStarNodesForStartAndTarget(Vector2 startPos, Vector2 targetPos)
    {
        var start = new AStarNode(new Vector2Int((int)startPos.x, (int)startPos.y), null, 0, GetCost(startPos, targetPos), true);
        var target = new AStarNode(new Vector2Int((int)targetPos.x, (int)targetPos.y), null, 0, 0, true);

        return (start, target);
    }

    public List<AStarNode> FindPath()
    {
        OpenList.Add(StartNode);

        while (OpenList.Any())
        {
            var current = OpenList.OrderBy(x => x.FCost).First();
            ClosedList.Add(current);
            
            if(current.Position == TargetNode.Position){
                // Path found
                return RetracePath(current);
            }
            
            var neighbours = FindNeighbors(current).ToList();
            neighbours.ForEach(x =>
            {
                if(!x.IsWalkable || ClosedList.Select(y => y.Position).Contains(x.Position)) // TODO: Can nodes for the same position be different?
                {
                    return;
                }
                
                OpenList.Add(x);
            });
            
            OpenList.Remove(current);
            OpenList = OpenList.DistinctBy(x => x.Position).ToList();
        }

        Debug.LogError("No viable path was found :(");
        return new List<AStarNode>();
    }

    private AStarNode CreateNeighbor(AStarNode current, Vector2Int direction)
    {
        var neighborPos = current.Position + direction;
        return new AStarNode(
            neighborPos,
            current,
            GetCost(StartNode.Position, neighborPos),
            GetCost(neighborPos, TargetNode.Position),
            true
        );
    }

    public List<AStarNode> FindNeighbors(AStarNode currentNode)
    {
        var directions = new[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        return directions.Select(d => CreateNeighbor(currentNode, d)).ToList();
    }

    public List<AStarNode> RetracePath(AStarNode node)
    {
        List<AStarNode> path = new List<AStarNode>();
        AStarNode currentNode = node;
        while (currentNode.Parent != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        
        return path;
    }


    public float GetCost(Vector2 currentPos, Vector2 target)
    {
        return Mathf.Abs(currentPos.x - target.x) + Mathf.Abs(currentPos.y - target.y);
    }
}

public class AStarNode
{
    public Vector2Int Position;
    public AStarNode Parent;
    public float GCost; // Cost from start to this node
    public float HCost; // Heuristic cost to target
    public float FCost => GCost + HCost; // Total cost
    public bool IsWalkable; // Indicates if the node is walkable

    public AStarNode(Vector2Int position, AStarNode parent, float gCost, float hCost, bool isWalkable)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
        IsWalkable = isWalkable;
    }
}
