using UnityEngine;
using UnityEngine.Serialization;

public class Patrol : MonoBehaviour
{
    
    [SerializeField] private PatrolPoint[] waypoints;
    private int _patrolPointIndex = 0;
    
    public bool IsValid => waypoints != null && waypoints.Length > 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waypoints = GetComponentsInChildren<PatrolPoint>();
        _patrolPointIndex = 0;
    }

    public PatrolPoint GetNextPoint()
    {
        _patrolPointIndex++;
        if (_patrolPointIndex >= waypoints.Length)
        {
            _patrolPointIndex = 0;
        }
        return waypoints[_patrolPointIndex];
        
    }
    
}
