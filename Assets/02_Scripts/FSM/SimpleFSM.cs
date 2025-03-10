using Unity.VisualScripting;
using UnityEngine;

public class SimpleFSM : MonoBehaviour
{

    enum State
    {
        Patrol,
        Chase,
        Flee
    }

    private State _currentState = State.Patrol;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckTransitions(_currentState);
        ExecuteState(_currentState);
    }
    // FSM Scripts
    private void CheckTransitions(State currentState)
    {
        switch (currentState)
        {
            case State.Patrol:
                break;
            case State.Chase:
                break;
            case State.Flee:
                break;
        }
    }
    private void ExecuteState(State currentState)
    {
        switch (currentState)
        {
            case State.Patrol:
                break;
            case State.Chase:
                break;
            case State.Flee:
                break;
        }
    }
    
    private void ChangeState(State newState)
    {
        ExitState(_currentState);
        _currentState = newState;
        EnterState(_currentState);
    }
    private void EnterState(State currentState)
    {
        switch (currentState)
        {
            case State.Patrol:
                break;
            case State.Chase:
                break;
            case State.Flee:
                break;
        }
    }
    private void ExitState(State currentState)
    {
        switch (currentState)
        {
            case State.Patrol:
                break;
            case State.Chase:
                break;
            case State.Flee:
                break;
        }
    }

}
