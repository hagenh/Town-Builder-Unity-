using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NpcScriptTest : MonoBehaviour
{
    public NpcJob job;
    public float treeRadius = 99999f;

    private List<AStarNode> path;
    private AStarNode currentNode;
    private AStarNode targetNode;

    private float reachThreshold = 2f;
    private float movementSpeed = 1f;

    public Tilemap Tilemap;

    public string Task;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ExecuteWoodcutterJob();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteTask()
    {
        
    }
    
    public void ExecuteWoodcutterJob()
    {
        Debug.Log("Woodcutter job is being executed!");
        
        var trees = GameObject.FindGameObjectsWithTag("Tree");
        var gameObjectInRadius = trees
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToArray();

        if (!gameObjectInRadius.Any())
        {
            Debug.Log("No Tree");
            return;
        }

        path = new AStarAlgorithm(transform.position, gameObjectInRadius.First().transform.position).FindPath();

        StartCoroutine(nameof(FollowPath));
        Thread.Sleep(1000);
        
        // Chop tree (add cooldown)
        // Keep going until tired or other need arises, then return to base 
    }

    private IEnumerator FollowPath()
    {
        int currentIndex = 0;
        while (currentIndex < path.Count)
        {
            var targetPosition = path[currentIndex].Position;
            
            while (Vector2.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    movementSpeed * Time.deltaTime
                );
                yield return null;
            }
            
            Debug.Log("Reached node: " + targetPosition);

            currentIndex++;
            yield return null;
        }

        Debug.Log("Reached target tree!");
        // Implement tree chopping logic here
    }
}

public enum NpcJob
{
    Woodcutter,
    Chicken
}


