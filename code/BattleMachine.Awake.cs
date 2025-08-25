using Sandbox;
using System;

namespace TacticsRPG;

public partial class BattleMachine : StateMachine
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
		Log.Info("Machine Awake");
		Turn = GetComponent<TurnManager>();
		TurnQueue = GetComponent<TurnQueueController>();
		WinCondition = GetComponent<WinController>();
		commandHandler = GetComponent<CommandHandler>();

	}
}
