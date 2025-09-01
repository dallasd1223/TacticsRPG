using Sandbox;
using System;

namespace TacticsRPG;

partial class BattleMachine
{
	protected override void OnAwake()
	{
		if(Instance is null)
		{
			Instance = this;
		}
		else
		{
			Instance = null;
			Instance = this;
		}
		
		base.OnAwake();
		Log.Info("Machine Awake");
		Turn = GetComponent<TurnManager>();
		TurnQueue = GetComponent<TurnQueueController>();
		WinCondition = GetComponent<WinController>();
		commandHandler = GetComponent<CommandHandler>();

	}
}
