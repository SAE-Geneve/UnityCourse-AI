using System;
using System.Collections.Generic;
using System.Linq;
using Unity.IntegerTime;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

// ReSharper disable once InconsistentNaming
public class FSMMachine
{

    private IFSMState _currentState;
    private Dictionary<Type, List<FSMTransition>> _allTransitions = new Dictionary<Type, List<FSMTransition>>();
    private List<FSMTransition> _currentStateTransitions = new List<FSMTransition>();
    private List<FSMTransition> _anyTransitions = new List<FSMTransition>();
    private static List<FSMTransition> _emptyTransitions = new List<FSMTransition>();

    public void Tick()
    {
        FSMTransition transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        _currentState?.Tick();
        
        Debug.Log("Current state : " + (_currentState == null ? "null" : _currentState.GetType().Name));

    }

    public void SetState(IFSMState toState)
    {
        if (toState != null && toState == _currentState)
            return;

        _currentState?.OnExit();
        _currentState = toState;

        // ReSharper disable once PossibleNullReferenceException
        _allTransitions.TryGetValue(_currentState.GetType(), out _currentStateTransitions);
        if (_currentStateTransitions == null)
            _currentStateTransitions = _emptyTransitions;

        _currentState?.OnEnter();

    }

    private FSMTransition GetTransition()
    {
        foreach (FSMTransition transition in _anyTransitions)
        {
            if (transition.Condition()) return transition;
        }
        foreach (FSMTransition transition in _currentStateTransitions)
        {
            if (transition.Condition()) return transition;
        }

        return null;
    }

    public void AddTransition(IFSMState from, IFSMState to, Func<bool> condition)
    {
        if (_allTransitions.TryGetValue(from.GetType(), out var existingTransitions) == false)
        {
            existingTransitions = new List<FSMTransition>();
            _allTransitions[from.GetType()] = existingTransitions;
        }
        
        existingTransitions.Add(new FSMTransition(to, condition));
        
    }
    public void AddAnyTransition(IFSMState state, Func<bool> condition)
    {
        _anyTransitions.Add(new FSMTransition(state, condition));
    }
}

// ReSharper disable once InconsistentNaming
public class FSMTransition
{
    public IFSMState To;
    public Func<bool> Condition;

    public FSMTransition(IFSMState to, Func<bool> condition)
    {
        To = to;
        Condition = condition;
    }
}
