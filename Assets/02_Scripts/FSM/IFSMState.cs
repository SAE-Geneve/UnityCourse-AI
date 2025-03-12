using UnityEngine;

public interface IFSMState
{
    void Tick();
    void OnEnter();
    void OnExit();
}
