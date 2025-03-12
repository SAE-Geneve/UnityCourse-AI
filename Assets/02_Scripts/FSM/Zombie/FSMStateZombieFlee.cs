using UnityEngine;

public class FSMStateZombieFlee : IFSMState
{

    private readonly SteeringBehaviour _steering;
    private readonly RadiusSensor _sensor;
    private readonly float _timerDuration;

    private float _timerTime;
    
    public bool TimerDone => _timerTime >= _timerDuration;
    
    public FSMStateZombieFlee(SteeringBehaviour steering, RadiusSensor sensor, float timerDuration)
    {
        _steering = steering;
        _sensor = sensor;
        _timerDuration = timerDuration;
    }

    #region IFSMState Implementation

    public void Tick()
    {
        Debug.Log("Flee");
        _steering.SteeringTarget = _sensor.Center;
        _timerTime += Time.deltaTime;
    }
    public void OnEnter()
    {
        _steering.FleeFactor = 1;
        _timerTime = 0;
    }
    public void OnExit()
    {
        _steering.FleeFactor = 0;
    }

    #endregion

}
