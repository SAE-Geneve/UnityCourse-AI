using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Pathfinding : MonoBehaviour
{

    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap debug;
    [SerializeField] private TileBase debugTile;

    [SerializeField] private Transform entry;
    [SerializeField] private Transform treasure;

    private Vector3Int[] _neighbours =
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(-1, 0, 0)
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (ground.HasTile(ground.WorldToCell(entry.position)))
            Debug.Log("entry stairs on the map");
        else
            Debug.Log("entry stairs NOT on the map");

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (ground.HasTile(ground.WorldToCell(treasure.position)))
            Debug.Log("Treasure on the map");
        else
            Debug.Log("Treasure NOT on the map");
        
        BFS_Search();
        
    }

    // Update is called once per frame
    void Update()
    {

        

    }
    
    private void BFS_Search()
    {

        Vector3Int startPosition = ground.WorldToCell(entry.position);
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        List<Vector3Int> visited = new List<Vector3Int>();

        q.Enqueue(startPosition);
        
        do
        {

            startPosition = q.Dequeue();
            
            // Debug
            debug.SetTile(startPosition, debugTile);
            
            if(!visited.Contains(startPosition))
            {
                visited.Add(startPosition);
            }

            foreach (Vector3Int neighbour in _neighbours)
            {
                Vector3Int newPos = startPosition + neighbour;
                
                if (newPos == ground.WorldToCell(treasure.position))
                {
                    Debug.Log($"Trouvé : {newPos}");
                    return;
                }
                
                if(!q.Contains(newPos))
                {
                    if (!visited.Exists(v => v.x == newPos.x && v.y == newPos.y && v.z == newPos.z))
                    {
                        if(ground.HasTile(newPos))
                        {
                            q.Enqueue(newPos);
                        }
                    }
                }
            }

        } while (q.Count > 0);
        
        Debug.Log($"Pas Trouvé :(");
        
    }
}
