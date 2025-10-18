using Sandbox;
using System;

namespace TacticsRPG;
public abstract class State : Component
{
	public virtual StateMachine stateMachine {get; set;}

	public virtual void Enter()
	{
		AddListeners();
	}
	public virtual void Update(){}

	public virtual void Exit()
	{
		RemoveListeners();
		this.Destroy();
	}

	protected virtual void AddListeners(){}
	protected virtual void RemoveListeners(){}

	protected override void OnDestroy()
	{
		RemoveListeners();
	}

}

public class StateMachine : Component
{
	[Property] public State ActiveState {
		get {return _activeState;}
		set {TransitionState(value);}
	}

	protected State _activeState;
	public bool InTransition;

	public virtual void TransitionState(State state)
	{
		Log.Info($"Transitioning To: {state}");
		Log.Info($"InTransition: {InTransition}");
		
		if(_activeState == state || InTransition)
		{
			Log.Info($"{this} Transition Null From {_activeState}");
			InTransition = false;
			return;
		}

		InTransition = true;

		if(_activeState != null)
		{
			_activeState.Exit();
		}

		_activeState = state;

		//Hook Event Into State Transition

		if(_activeState != null)
		{

			_activeState.Enter();
		}
		
		InTransition = false;

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
			target.stateMachine = this;
		}
		return target;
	}

	public virtual void ChangeState<T> () where T : State, new()
	{
		ActiveState = GetState<T>();
	}

	public virtual void NullState()
	{
		if(_activeState != null)
		{
			_activeState.Exit();
			_activeState = null;
		}
	}
}
