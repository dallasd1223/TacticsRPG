using Sandbox;

namespace TacticsRPG;

public sealed class CommandHandler : Component
{
	[Property] public BattleMachine Machine {get; set;}
	[Property] public Queue<Command> CommandList {get; set;}
	[Property] public HandlerState CurrentState {get; set;} = HandlerState.WaitForCommands;
	[Property] public Command CurrentCommand {get; set;}
	[Property] public bool IsProcessing {get; set;}

	public event Action ProcessComplete;

	protected override void OnAwake()
	{
		PlayerEvents.ConfirmAction += AddCommand;
		Machine = this.GetComponent<BattleMachine>();
	}

	protected override void OnDestroy()
	{
		PlayerEvents.ConfirmAction -= AddCommand;		
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
		if(Machine.IsValid())
		{
			Machine.ChangeState<ExecuteActionState>();
		}

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
		ProcessComplete?.Invoke();
		//Manager.DecideTurnState();
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
