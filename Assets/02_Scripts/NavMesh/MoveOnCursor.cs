using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MoveOnCursor : MonoBehaviour
{
    
    private NavMeshAgent _agent;
    private Animator _animator;
    
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
        if (mouse.leftButton.isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(mouse.position.value);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                _agent.destination = hit.point;
            }
        }
        
        if ((_agent.destination - transform.position).magnitude > _agent.stoppingDistance)
        {
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }

    }

    private void OnAnimatorMove()
    {
        _agent.speed = _animator.deltaPosition.magnitude / Time.deltaTime;
    }

}
