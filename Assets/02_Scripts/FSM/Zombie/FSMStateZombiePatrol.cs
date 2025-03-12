using System;
using UnityEngine;
using UnityEngine.AI;

public class FSMStateZombiePatrol : IFSMState
{
    private SteeringBehaviour _steering;

    public FSMStateZombiePatrol(SteeringBehaviour steering)
    {
        _steering = steering;
    }

    #region IFSMState Implementation

    public void Tick()
    {
        Debug.Log("Patrol");
    }
    public void OnEnter()
    {
        _steering.WanderFactor = 1;
    }
    public void OnExit()
    {
        _steering.WanderFactor = 0;
    }

    #endregion

}
