using UnityEngine;

public class FSMStateZombieChase : IFSMState
{

    private SteeringBehaviour _steering;
    private ForwardSensor _sensor;

    public FSMStateZombieChase(SteeringBehaviour steering, ForwardSensor sensor)
    {
        _steering = steering;
        _sensor = sensor;
    }

    #region IFSMState Implementation

    public void Tick()
    {
        Debug.Log("Chase");
        _steering.SteeringTarget = _sensor.Target.position;
    }
    public void OnEnter()
    {
        _steering.SeekFactor = 1;
    }
    public void OnExit()
    {
        _steering.SeekFactor = 0;
    }

    #endregion

}
