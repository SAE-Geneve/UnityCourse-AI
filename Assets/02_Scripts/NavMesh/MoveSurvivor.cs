using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MoveSurvivor : MonoBehaviour
{

    [SerializeField] private Transform cursor;
    [SerializeField] private Transform character;
    
    private NavMeshAgent _agent;
    private Animator _animator;

    private Vector2 _mousePosition;
    private bool _movePressed;
    private bool _shootPressed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseWorldPosition = transform.position;
        
        Ray ray = Camera.main.ScreenPointToRay(_mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mouseWorldPosition = hit.point;
        }
        cursor.position = new Vector3(mouseWorldPosition.x, 0, mouseWorldPosition.z);
        
        // Turn on mouse
        Vector3 playerDirection = mouseWorldPosition - transform.position;
        if (playerDirection.magnitude > _agent.stoppingDistance)
        {
            character.rotation = Quaternion.LookRotation(playerDirection);
        }
        
        // Move on click
        if (_movePressed)
        {
            _agent.SetDestination(mouseWorldPosition);
        }

        
        if (_agent.velocity.magnitude > Mathf.Epsilon)
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
        // if(_animator.GetBool("IsRunning"))
        // {
        //     _agent.speed = _animator.deltaPosition.magnitude / Time.deltaTime;
        // }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movePressed = context.ReadValueAsButton();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        _shootPressed = context.ReadValueAsButton();
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
    }

}
