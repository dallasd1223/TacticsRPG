using Sandbox;
using System;

namespace TacticsRPG;

public sealed class SelectorManager : Component
{
	public ISelectorState State;

	public BattleMachine Machine;

	protected override void OnAwake()
	{
		Machine.Input.InputPressed += HandleInput;
	}

	protected override void OnUpdate()
	{

	}

	public void HandleInput(InputKey key)
	{

	}
}

public interface ISelectorState
{

}

public class FreeLookState : ISelectorState
{

}

public class AttackSelectState : ISelectorState
{

}

public class AbilitySelectState : ISelectorState
{
	
}
