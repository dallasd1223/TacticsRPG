using Sandbox;
namespace TacticsRPG;

public partial class SelectorManager
{
	protected override void OnAwake()
	{
		PlayerEvents.FocusModeChange += HandleFocusMode;
		PlayerEvents.CommandModeChange += HandleCommandMode;
		InputEvents.ActionSelectInputPressed += HandleInput;
	}

	protected override void OnDestroy()
	{
		PlayerEvents.FocusModeChange -= HandleFocusMode;
		PlayerEvents.CommandModeChange -= HandleCommandMode;
		InputEvents.ActionSelectInputPressed -= HandleInput;		
	}

	protected override void OnStart()
	{
		ChangeState<FreeLookSelectState>();
	}

	public override void TransitionState(State state)
	{
		base.TransitionState(state);
		State = (SelectorState)ActiveState;
	}
	
	public void HandleFocusMode(FocusMode? f, Unit u)
	{
		if(f == FocusMode.FreeLook)
		{
			ActivateSelector();
		}
		else
		{
			DeactivateSelector();
		}
	}

	public void HandleCommandMode(CommandMode? c)
	{
		switch(c)
		{
			case CommandMode.NA:
				ChangeState<FreeLookSelectState>();
				break;
			case CommandMode.MoveSelect:
				ChangeState<MoveSelectState>();
				break;
			case CommandMode.AttackSelect:
				ChangeState<AttackSelectState>();
				break;
			case CommandMode.AbilitySelect:
				ChangeState<AbilitySelectState>();
				break;
			case CommandMode.WaitSelect:
				ChangeState<WaitSelectState>();
				break;
			default:
				break;
		}
	}

	public void ActivateSelector()
	{
		IsActive = true;
		TrySetAll(HoveredVector);
		Log.Info("Selector Activated");
	}

	public void DeactivateSelector()
	{
		IsActive = false;
		Log.Info("Selector Deactivated");
	}
	
	public void HandleInput(InputKey key)
	{
		switch(key)
		{
			case InputKey.ENTER:
				break;
			case InputKey.LEFT:
				TrySetAll(new Vector2(-1,0));
				break;
			case InputKey.RIGHT:
				TrySetAll(new Vector2(1,0));
				break;
			case InputKey.FORWARD:
				TrySetAll(new Vector2(0,1));
				break;
			case InputKey.BACKWARD:
				TrySetAll(new Vector2(0,-1));
				break;
		}
	}
}
