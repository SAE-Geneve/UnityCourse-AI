using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class SimpleFSM : MonoBehaviour
{

    private enum State
    {
        Patrol,
        Chase,
        Flee
    }

    [SerializeField] private ForwardSensor playerSensor;
    [SerializeField] private RadiusSensor molotovSensor;
    
    private State _currentState = State.Patrol;
    private SteeringBehaviour _motion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _motion = GetComponent<SteeringBehaviour>();
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
                if(playerSensor.Target) ChangeState(State.Chase);
                break;
            case State.Chase:
                if(!playerSensor.Target) ChangeState(State.Patrol);
                if(molotovSensor.HasDetected) ChangeState(State.Flee);
                break;
            case State.Flee:
                if(!molotovSensor.HasDetected) ChangeState(State.Patrol);
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
                _motion.SteeringTarget = playerSensor.Target.position;
                break;
            case State.Flee:
                _motion.SteeringTarget = molotovSensor.Center;
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
                _motion.WanderFactor = 1;
                break;
            case State.Chase:
                _motion.SeekFactor = 1;
                break;
            case State.Flee:
                _motion.FleeFactor = 1;
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
                _motion.SeekFactor = 0;
                break;
            case State.Flee:
                _motion.FleeFactor = 0;
                break;
        }
    }

}
