using Sandbox;
using System;

namespace TacticsRPG;
public abstract class State : Component
{
	protected StateMachine stateMachine;

	public virtual async void Enter()
	{
		AddListeners();
	}
	public virtual void Update(){}

	public virtual async void Exit()
	{
		RemoveListeners();
	}

	protected virtual void AddListeners(){}
	protected virtual void RemoveListeners(){}

	public State() {}

}

public class StateMachine : Component
{
	public State ActiveState {
		get {return _activeState;}
		set {TransitionState(value);}
	}

	protected State _activeState;
	public bool InTransition;

	public void TransitionState(State state)
	{
		if(_activeState == state || InTransition)
		{
			return;
		}

		InTransition = true;

		if(_activeState != null)
		{
			_activeState.Exit();
		}

		_activeState = state;

		if(_activeState != null)
		{
			_activeState.Enter();
		}

	}

	public void Update()
	{

	}

	public virtual T GetState<T>() where T : State, new()
	{
		T target = GetComponent<T>();
		if(target == null)
		{
			target = AddComponent<T>();
		}
		return target;
	}

	public virtual void ChangeState<T> () where T : State, new()
	{
		ActiveState = GetState<T>();
	}
}
