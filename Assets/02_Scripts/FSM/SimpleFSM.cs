using Unity.VisualScripting;
using UnityEngine;

public class SimpleFSM : MonoBehaviour
{

    public enum State
    {
        Patrol,
        Chase,
        Flee
    }

    [SerializeField] private State currentState = State.Patrol;
    private SteeringBehaviour _motion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _motion = GetComponent<SteeringBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTransitions(currentState);
        ExecuteState(currentState);
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
        
        Debug.Log("Current State : " + currentState);
        
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
        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }
    private void EnterState(State currentState)
    {
        switch (currentState)
        {
            case State.Patrol:
                _motion.WanderFactor = 1;
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
                _motion.WanderFactor = 0;
                break;
            case State.Chase:
                break;
            case State.Flee:
                break;
        }
    }

}
