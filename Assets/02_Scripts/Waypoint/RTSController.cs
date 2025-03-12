using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class RTSController : MonoBehaviour
{
    [SerializeField] private GameObject rtsPoint;
    [SerializeField] private Transform characterOriented;
    [SerializeField] private MolotovThrower thrower;
    [SerializeField] private LayerMask clickLayerMask;

    private NavMeshAgent _agent;
    private Animator _animator;

    // ReSharper disable once MemberCanBePrivate.Global
    public readonly Queue<Transform> PathPoints = new Queue<Transform>();

    private GameObject _oldPoint;

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
        Vector2 mousePos = mouse.position.value;


        if (mousePos.x >= 0 && mousePos.x <= Screen.width &&
            mousePos.y >= 0 && mousePos.y <= Screen.height)
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickLayerMask))
            {
                // Orient character AND throw point
                characterOriented.rotation = Quaternion.LookRotation(new Vector3(hit.point.x, 0, hit.point.z));
                
                if (mouse.leftButton.wasPressedThisFrame)
                {
                    // Place a point
                    if (CheckProximity(hit.point) == false)
                    {
                        GameObject newPoint = Instantiate(rtsPoint, hit.point, Quaternion.identity);
                        AddPoint(newPoint.transform);
                    }
                }
                
                if (mouse.rightButton.wasPressedThisFrame)
                {
                    thrower.ThrowMolotov();
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
        return PathPoints.ToList().Exists(p => Vector3.Distance(p.position, mouseClick) < 5f);
    }

    private void AddPoint(Transform pathPoint)
    {
        PathPoints.Enqueue(pathPoint);
    }

    private Transform GetNextPoint()
    {
        if (PathPoints.Count == 0)
        {
            Destroy(_oldPoint);
            return transform;
        }

        GameObject newPoint = PathPoints.Dequeue().gameObject;

        Destroy(_oldPoint);
        _oldPoint = newPoint;

        return newPoint.transform;

    }
    //
    // private void DeletePoint()
    // {
    //     Destroy(pathPoints[0].gameObject);
    //     pathPoints.RemoveAt(0);
    // }

}
