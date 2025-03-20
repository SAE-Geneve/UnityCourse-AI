using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;


public class Pathfinding : MonoBehaviour
{

    [SerializeField] private Tilemap groundMap;
    [SerializeField] private Tilemap debugMap;
    [SerializeField] private TileBase debugTile;

    [SerializeField] private Door[] doors;

    

    [SerializeField] private Transform entry;
    [SerializeField] private Transform treasure;

    private Vector3Int[] _neighbours =
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0)
    };

    [Header("Research Debug stuff")]
    [SerializeField] private bool bfsSearch;
    [SerializeField][Range(0f, 1f)] private float coroutineDelay;
    [SerializeField][Range(0f, 0.1f)] private float coroutineTimer;
    [SerializeField][Tooltip("check the check box to start coroutines.")] private bool coroutineDone = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (groundMap.HasTile(groundMap.WorldToCell(entry.position)))
            Debug.Log("entry stairs on the map");
        else
            Debug.Log("entry stairs NOT on the map");

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (groundMap.HasTile(groundMap.WorldToCell(treasure.position)))
            Debug.Log("Treasure on the map");
        else
            Debug.Log("Treasure NOT on the map");

        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!coroutineDone)
            return;

        debugMap.ClearAllTiles();
        StartCoroutine(bfsSearch ? "BFS_Search" : "DFS_Search");
        
    }

    private IEnumerator BFS_Search()
    {

        yield return new WaitForSeconds(coroutineDelay);
        
        Vector3Int startPosition = groundMap.WorldToCell(entry.position);
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        List<Vector3Int> visited = new List<Vector3Int>();
        
        bool found = false;
        
        coroutineDone = false;
        
        q.Enqueue(startPosition);

        do
        {

            startPosition = q.Dequeue();

            // Debug
            debugMap.SetTile(startPosition, debugTile);
            yield return new WaitForSeconds(coroutineTimer);

            if (!visited.Contains(startPosition))
            {
                visited.Add(startPosition);
            }

            foreach (Vector3Int neighbour in _neighbours)
            {
                
                Vector3Int newPos = startPosition + neighbour;

                if (newPos == groundMap.WorldToCell(treasure.position))
                {
                    Debug.Log($"Trouvé : {newPos}");
                    found = true;
                }

                if (!q.Contains(newPos))
                {
                    if (!visited.Exists(v => v.x == newPos.x && v.y == newPos.y && v.z == newPos.z))
                    {
                        if(groundMap.HasTile(newPos))
                        {
                            Door oneDoor = doors.FirstOrDefault(d => d.GetComponent<SpriteRenderer>().bounds.Contains(newPos));
                            if (oneDoor != null)
                            {
                                if (oneDoor.IsOpen) q.Enqueue(newPos);
                            }
                            else
                            {
                                q.Enqueue(newPos);
                            }
                        }
                    }
                }
            }

        } while (q.Count > 0 && !found);

        Debug.Log($"Pas Trouvé :(");

    }
    
    private IEnumerator DFS_Search()
    {
        
        yield return new WaitForSeconds(coroutineDelay);
        
        Vector3Int startPosition = groundMap.WorldToCell(entry.position);
        Stack<Vector3Int> q = new Stack<Vector3Int>();
        List<Vector3Int> visited = new List<Vector3Int>();

        bool found = false;
        
        coroutineDone = false;
        
        q.Push(startPosition);

        do
        {

            startPosition = q.Pop();

            // Debug
            debugMap.SetTile(startPosition, debugTile);
            yield return new WaitForSeconds(coroutineTimer);

            if (!visited.Contains(startPosition))
            {
                visited.Add(startPosition);
            }

            foreach (Vector3Int neighbour in _neighbours)
            {
                Vector3Int newPos = startPosition + neighbour;

                if (newPos == groundMap.WorldToCell(treasure.position))
                {
                    Debug.Log($"Trouvé : {newPos}");
                    found = true;
                }

                if (!q.Contains(newPos))
                {
                    if (!visited.Exists(v => v.x == newPos.x && v.y == newPos.y && v.z == newPos.z))
                    {
                        if (groundMap.HasTile(newPos))
                        {
                            Door oneDoor = doors.FirstOrDefault(d => d.GetComponent<SpriteRenderer>().bounds.Contains(newPos));
                            if (oneDoor != null)
                            {
                                if(oneDoor.IsOpen) q.Push(newPos);
                            }
                            else
                            {
                                q.Push(newPos);
                            }
                        }
                    }
                }
            }

        } while (q.Count > 0 && !found);

        Debug.Log($"Pas Trouvé :(");

    }



}
