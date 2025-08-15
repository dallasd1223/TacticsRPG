using Sandbox;

namespace TacticsRPG;

public sealed class CommandHandler : Component
{
	[Property] public BattleManager Manager {get; set;}
	[Property] public Queue<Command> CommandList {get; set;}
	[Property] public HandlerState CurrentState {get; set;} = HandlerState.WaitForCommands;
	[Property] public Command CurrentCommand {get; set;}
	[Property] public bool IsProcessing {get; set;}

	protected override void OnAwake()
	{
		Manager = this.GetComponent<BattleManager>();
	}

	public void CommandUpdate()
	{

	}

	public void AddCommand(Command command)
	{
		Log.Info($"{command} Added");
		CommandList.Enqueue(command);
		CheckCommandList();
	}
	
	public void StartProcessing()
	{
		ChangeHandlerState(HandlerState.ProcessCommands);
		Manager.ChangeCurrentState(BattleState.ProcessCommands);
		IsProcessing = true;
		CurrentCommand = CommandList.Dequeue();
		CurrentCommand.Execute();

	}
	public void ProcessCommands()
	{
		if(!CurrentCommand.IsFinished)
		{
			CurrentCommand.Tick();
		}
		else
		{
			CheckCommandList();
		}
	}
	public void ProcessCommand(Command command)
	{
		command.Execute();
	}

	public void EndProcessing()
	{
		IsProcessing = false;
		CommandList.Clear();
		Manager.DecideTurnState();
	}

	public void CheckCommandList()
	{
		if(CurrentCommand is not null)
		{
			if(!CurrentCommand.IsFinished)
			{
				return;
			}
		}
		if(CommandList.Any())
		{
			Log.Info("Commands Found, Processing Begins");
			StartProcessing();	
		}
		else
		{
			EndProcessing();
		}
	}
	public void ChangeHandlerState(HandlerState state)
	{
		Log.Info($"Handler {CurrentState} State Changing To {state}");
		CurrentState = state;
	}
}

public enum HandlerState
{
	WaitForCommands,
	ProcessCommands,
	EndHandler,
}
