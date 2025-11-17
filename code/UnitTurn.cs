using Sandbox;
using System;

namespace TacticsRPG;

public sealed class UnitTurn : Component
{
	[Property] public bool HasMoved {get; set;} = false;
	[Property] public bool HasActed {get; set;}= false;
	public event Action TurnStarted;
	public event Action TurnEnded;

	protected override void OnAwake()
	{
		TurnStarted += OnTurnStart;
		TurnEnded += OnTurnEnd;

	}

	public void StartTurn()
	{
		TurnStarted?.Invoke();
	}

	public void EndTurn()
	{
		var unit = GetComponent<BattleUnit>();
		unit.Interact.UnitTile.current = false;
		TurnEnded?.Invoke();
	}

	private void OnTurnStart()
	{
		HasActed = false;
		HasMoved = false;
		var commands = GetComponent<UnitCommand>();
		foreach(CommandItem com in commands.CommandItems)
		{
			com.Active = true;
		}
		Log.Info("Turn Started");
	}

	private void OnTurnEnd()
	{

		Log.Info("Turn Ended");
	}
}
