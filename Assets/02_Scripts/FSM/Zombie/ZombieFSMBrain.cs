using UnityEngine;

public class ZombieFSMBrain : MonoBehaviour
{
    [SerializeField] private ForwardSensor playerSensor;
    [SerializeField] private RadiusSensor molotovSensor;
    
    private SteeringBehaviour _motion;
    
    // States
    private FSMMachine _stateMachine;
    
    private FSMStateZombiePatrol _patrol;
    private FSMStateZombieChase _chase;
    private FSMStateZombieFlee _flee;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _motion = GetComponent<SteeringBehaviour>();

        _stateMachine = new FSMMachine();
        
        _patrol = new FSMStateZombiePatrol(_motion);
        _chase = new FSMStateZombieChase(_motion, playerSensor);
        _flee = new FSMStateZombieFlee(_motion, molotovSensor, 5.0f);
        
        _stateMachine.AddTransition(_patrol, _chase, () => playerSensor.Target);
        _stateMachine.AddTransition(_chase, _patrol, () => !playerSensor.Target);
        _stateMachine.AddTransition(_chase, _flee, () => molotovSensor.HasDetected);
        _stateMachine.AddTransition(_flee, _patrol, () => !molotovSensor.HasDetected && _flee.TimerDone);

        _stateMachine.SetState(_patrol);
        
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Tick();
    }
}
