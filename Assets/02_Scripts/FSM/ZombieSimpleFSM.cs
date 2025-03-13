using System;
using UnityEngine;

public class ZombieSimpleFSM : MonoBehaviour
{

    enum FSM_State
    {
        Empty,
        Patrol,
        Chase,
        Attack,
        Flee
    }
    
    [SerializeField] private ForwardSensor playerSensor;
    [SerializeField] private ForwardSensor molotovSensor;
    [SerializeField] private float timerDuration = 5;
    
    private FSM_State _currentState = FSM_State.Empty;
    private SteeringBehaviour _motion;
    private float timer;
    

    private void Start()
    {
        _motion = GetComponent<SteeringBehaviour>();
        
        SetState(FSM_State.Patrol);
    }

    private void Update()
    {
        CheckTransitions(_currentState);
        OnStateUpdate(_currentState);
    }
    private void CheckTransitions(FSM_State state)
    {
        // Any state transitions
        if(molotovSensor.HasDetected)
        {
            SetState(FSM_State.Flee);
            return;
        }
        
        switch (state)
        {
            case FSM_State.Patrol:
                if(playerSensor.HasDetected) SetState(FSM_State.Chase);
                break;
            case FSM_State.Chase:
                if(!playerSensor.HasDetected)
                    SetState(FSM_State.Patrol);
                break;
            case FSM_State.Flee:
                if(timer > timerDuration && !molotovSensor.HasDetected)
                    SetState(FSM_State.Patrol);
                break;
            case FSM_State.Attack:
                // Nothing to do yet
                break;
            case FSM_State.Empty:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    private void OnStateEnter(FSM_State state)
    {
        Debug.Log($"OnEnter : {state}");
        
        switch (state)
        {
            case FSM_State.Patrol:
                _motion.WanderFactor = 1;
                break;
            case FSM_State.Chase:
                _motion.SeekFactor = 1;
                break;
            case FSM_State.Flee:
                _motion.FleeFactor = 1;
                timer = 0;
                break;
            case FSM_State.Attack:
                break;
            case FSM_State.Empty:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }

    }
    private void OnStateExit(FSM_State state)
    {
        Debug.Log($"OnExit : {state}");
        
        switch (state)
        {
            case FSM_State.Patrol:
                _motion.WanderFactor = 0;
                break;
            case FSM_State.Chase:
                _motion.SeekFactor = 0;
                break;
            case FSM_State.Flee:
                _motion.FleeFactor = 0;
                break;
            case FSM_State.Attack:
                // Nothing to do yet
                break;
            case FSM_State.Empty:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    private void OnStateUpdate(FSM_State state)
    {
        Debug.Log($"OnUpdate : {state}");

        switch (state)
        {
            case FSM_State.Chase:
                _motion.SteeringTarget = playerSensor.TargetPos;
                break;
            
            case FSM_State.Flee:
                _motion.SteeringTarget = molotovSensor.TargetPos;
                timer += Time.deltaTime;
                break;
            
            case FSM_State.Patrol:
            case FSM_State.Attack: 
                // Nothing to do yet
                break;
            case FSM_State.Empty:
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void SetState(FSM_State newState)
    {
        if (newState == FSM_State.Empty) return;
        if(_currentState != FSM_State.Empty) OnStateExit(_currentState);
        
        _currentState = newState;
        OnStateEnter(_currentState);
        
    }
    

}
