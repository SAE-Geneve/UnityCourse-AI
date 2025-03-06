using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class RTSController : MonoBehaviour
{
    [SerializeField] private GameObject point;

    private NavMeshAgent _agent;
    private Animator _animator;

    public Queue<Transform> pathPoints = new Queue<Transform>();
    private GameObject oldPoint;
    public int _patrolPointIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(mouse.position.value);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (CheckProximity(hit.point) == false)
                {
                    GameObject newPoint = Instantiate(point, hit.point, Quaternion.identity);
                    AddPoint(newPoint.transform);
                }
            }
        }

        if ((_agent.destination - transform.position).magnitude > _agent.stoppingDistance)
        {
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _agent.destination = GetNextPoint().position;
            _animator.SetBool("IsRunning", false);
        }

    }

    private void OnAnimatorMove()
    {
        //_agent.speed = _animator.deltaPosition.magnitude / Time.deltaTime;
    }

    private bool CheckProximity(Vector3 mouseClick)
    {
        return pathPoints.ToList().Exists(p => Vector3.Distance(p.position, mouseClick) < 5f);
    }

    private void AddPoint(Transform pathPoint)
    {
        pathPoints.Enqueue(pathPoint);
    }

    private Transform GetNextPoint()
    {
        if (pathPoints.Count == 0)
        {
            Destroy(oldPoint);
            return transform;
        }
        
        GameObject newPoint = pathPoints.Dequeue().gameObject;
        
        Destroy(oldPoint);
        oldPoint = newPoint;
        
        return newPoint.transform;

    }
    //
    // private void DeletePoint()
    // {
    //     Destroy(pathPoints[0].gameObject);
    //     pathPoints.RemoveAt(0);
    // }

}
