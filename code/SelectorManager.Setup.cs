using Sandbox;
namespace TacticsRPG;

public partial class SelectorManager
{
	protected override void OnAwake()
	{
		PlayerEvents.FocusModeChange += HandleFocusMode;
		PlayerEvents.CommandModeChange += HandleCommandMode;
		PlayerEvents.CancelSelection += CancelSelection;
		InputEvents.ActionSelectInputPressed += HandleInput;
	}

	protected override void OnDestroy()
	{
		PlayerEvents.FocusModeChange -= HandleFocusMode;
		PlayerEvents.CommandModeChange -= HandleCommandMode;
		InputEvents.ActionSelectInputPressed -= HandleInput;		
	}

	protected override void OnUpdate()
	{
		//Protect Frame One Input
		if(JustTransitioned)
		{
			JustTransitioned = false;
			return;
		}
	}
	public override void TransitionState(State state)
	{
		base.TransitionState(state);
		State = (SelectorState)ActiveState;
		JustTransitioned = true;
		Log.Info($"JustTransitioned: {JustTransitioned}");
	}
	
	public void HandleFocusMode(FocusMode? f, Unit u)
	{
		if(f == FocusMode.FreeLook)
		{
			ActivateSelector(u);
			ChangeState<FreeLookSelectState>();
			return;
		}
		else if(f == FocusMode.CommandSelectLook)
		{
			ActivateSelector(u);
			return;
		}
		else if(f == FocusMode.ConfirmMenu)
		{
			return;
		}
		else if(f == FocusMode.Menu)
		{
			DeactivateSelector();
			NullState();
			return;
		}
	}

	public void HandleCommandMode(CommandMode? c)
	{
		switch(c)
		{
			case CommandMode.NA:
				NullState();
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
				NullState();
				break;
		}
	}

	public void ActivateSelector(Unit? u)
	{
		IsActive = true;
		IsConfirming = false;
		CurrentUnit = u;

		TileData StartTile = TileMapManager.Instance.GetTileFromUnit(u);
		Vector2 StartVec = TileMapManager.Instance.GetVector2FromTile(StartTile);
		
		Log.Info(StartVec);
		TrySetAll(StartVec, true);
		Log.Info("Selector Activated");
	}

	public void DeactivateSelector()
	{
		IsActive = false;
		IsConfirming = false;
		Deactivate?.Invoke();
		Log.Info("Selector Deactivated");
	}

	public void BuildCursor()
	{
		Log.Info("Building Cursor");
		var cursor = CursorPrefab.Clone();
		ActiveCursorObject = cursor;
		SelectCursor comp = cursor.GetComponent<SelectCursor>();
		ActiveCursor = comp;
		ActiveCursor.Selector = this;
		ActiveCursor.Activate();
	}

	public void DestroyCursor()
	{
		if(ActiveCursor.IsValid())
		{
			Log.Info("Destorying Cursor");
			ActiveCursor.RemoveListeners();
			ActiveCursorObject.Destroy();
			ActiveCursor = null;
		}
	}

	public void HandleInput(InputKey key)
	{
		//Prevent Past State First Frame Input Capture
		if(JustTransitioned)
		{
			JustTransitioned = false;
			Log.Info($"JustTransitioned: {JustTransitioned}");
			return;
		}
		if(!IsActive) return;
		if(IsConfirming) return;
		switch(key)
		{
			case InputKey.ENTER:
				TrySelect();
				break;
			case InputKey.LEFT:
				TrySetAll(new Vector2(-1,0), false);
				break;
			case InputKey.RIGHT:
				TrySetAll(new Vector2(1,0), false);
				break;
			case InputKey.FORWARD:
				TrySetAll(new Vector2(0,1), false);
				break;
			case InputKey.BACKWARD:
				TrySetAll(new Vector2(0,-1), false);
				break;
		}
	}
}
