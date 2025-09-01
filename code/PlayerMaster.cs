using Sandbox;

namespace TacticsRPG;
public partial class PlayerMaster : SingletonComponent<PlayerMaster>
{
	//URGENT REFACTOR --MISSION ACCOMPLISHED--

	[Property] public Unit CurrentUnit {get; set;} = null;

	[Property] public FocusMode? LastMode {get; set;} = null;
	[Property] public FocusMode? Mode {get; set;} = FocusMode.NA;

	[Property] public InventoryManager Inventory {get; set;}

	[Property] public CommandMode? LastcMode {get; set;} = null;
	[Property] public CommandMode? cMode {get; set;} = CommandMode.NA;
	[Property] public CommandType? CurrentSelectedCommand {get; set;} = null;

	[Property] public IAbilityItem CurrentCommandAbility {get; set;} = null;

	[Property] public ActionMenu Menu {get; set;}
	[Property] public ConfirmManager Confirm {get; set;}

	[Property] public SoundEvent Error {get; set;}
	[Property] public SoundEvent ConfirmSound {get; set;}
	
	protected override void OnDestroy()
	{
		Log.Info("PlayerMaster Destroyed");
		BattleEvents.ActionSelectStart -= OnActionSelectStart;
		InputEvents.ActionSelectInputPressed -= HandleInput;

		Confirm.ConfirmStart -= OnConfirmStart;
		Confirm.ConfirmEnd -= OnConfirmEnd;
		Confirm.ConfirmCancel -= OnCancelConfirm;	
	}
	protected override void OnAwake()
	{
		base.OnAwake();

		BattleEvents.ActionSelectStart += OnActionSelectStart;
		InputEvents.ActionSelectInputPressed += HandleInput;

		Confirm.ConfirmStart += OnConfirmStart;
		Confirm.ConfirmEnd += OnConfirmEnd;
		Confirm.ConfirmCancel += OnCancelConfirm;
	}

	public void OnActionSelectStart(Unit u)
	{
		InitiatePlayerMaster(u);
	}
	
	public void InitiatePlayerMaster(Unit u)
	{
		CurrentUnit = u;
		Mode = FocusMode.Menu;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		Menu.DoReset();

		Log.Info("PlayerMaster Initiated");
	}

	public void CancelCommand()
	{
		if(Mode == FocusMode.CommandSelectLook && (cMode == CommandMode.MoveSelect || cMode == CommandMode.AttackSelect || cMode == CommandMode.AbilitySelect || cMode == CommandMode.WaitSelect))
		{
			CurrentUnit.Interact.IsMoveSelecting = false;
			CurrentUnit.Move.RemoveMoveableTiles();
			CurrentUnit.Interact.IsAttackSelecting = false;
			CurrentUnit.Attack.RemoveAttackableTiles();
			CurrentUnit.Attack.EndAttackSelection();
			CurrentUnit.Interact.IsAbilitySelecting = false;
			CurrentUnit.Ability.EndAbilitySelect();

			Mode = FocusMode.Menu;
			cMode = CommandMode.NA;

			PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
			PlayerEvents.OnCommandModeChange(cMode);
			PlayerEvents.OnCancelCommand(CurrentUnit);

			Menu.DoReset();

			//GameCursor.Instance.DeactivateCursor();
			CurrentSelectedCommand = null;


			Sound.Play(Error);
		}
	}

	public void SelectedCommand(CommandType? command, CommandItem cItem = null)
	{
		switch(command)
		{
			case CommandType.Move:
				if(CurrentUnit.Turn.CommandIsActive("MOVE"))
				{
					Mode = FocusMode.CommandSelectLook;
					cMode = CommandMode.MoveSelect;
					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
					PlayerEvents.OnCommandModeChange(cMode);
					CurrentUnit.Interact.FindMoveableTiles();
					CurrentUnit.Interact.IsMoveSelecting = true;
					CurrentSelectedCommand = command;
				}
				else
				{
					Sound.Play(Error);
				}
				break;
			case CommandType.Attack:
				if(CurrentUnit.Turn.CommandIsActive("ATTACK"))
				{
					Mode = FocusMode.CommandSelectLook;
					cMode = CommandMode.AttackSelect;
					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
					PlayerEvents.OnCommandModeChange(cMode);
					CurrentUnit.Interact.IsAttackSelecting = true;
					CurrentUnit.Interact.FindAttackableTiles();
					CurrentSelectedCommand = command;
				}
				else
				{
					Sound.Play(Error);
				}
				break;
			case CommandType.Ability:
				if(CurrentUnit.Turn.CommandIsActive("ABILITY"))
				{
					Menu.ChangeMenuState(MenuState.Ability);
				}				
				break;
			case CommandType.Item:
				if(cItem is null) return;
				Mode = FocusMode.CommandSelectLook;
				cMode = CommandMode.AbilitySelect;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);
				CurrentUnit.Interact.IsAbilitySelecting = true;
				var item = (Item)cItem.AbilityItem;
				if(item is not null)
				{
					CurrentUnit.Interact.FindAbilityTilesFromRange(item.Data.BaseRange, item.Data.CanUseOnSelf);
					CurrentSelectedCommand = command;
					CurrentCommandAbility = item;
					PlayerEvents.OnAbilitItemSelected(CurrentCommandAbility);
					Log.Info($"{CurrentCommandAbility} Ability Item Event");
					break;
				}
				else
				{
					Log.Info("Bad Data");
					break;
				}
	
			case CommandType.Magic:
				if(cItem is null) return;
				Mode = FocusMode.CommandSelectLook;
				cMode = CommandMode.AbilitySelect;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);
				break;
			case CommandType.Skill:
				if(cItem is null) return;
				Mode = FocusMode.CommandSelectLook;
				cMode = CommandMode.AbilitySelect;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);	
				break;
			case CommandType.Wait:
				if(CurrentUnit.Turn.CommandIsActive("WAIT"))
				{
					Mode = FocusMode.CommandSelectLook;
					cMode = CommandMode.WaitSelect;
					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
					PlayerEvents.OnCommandModeChange(cMode);
					CurrentSelectedCommand = command;
				}
				else
				{
					Sound.Play(Error);
				}

				break;
			case null:
				Log.Info("Error With Command");
				break;
		}
	}

	public void OnConfirmStart()
	{
		LastMode = Mode;
		Mode = FocusMode.ConfirmMenu;
		LastcMode = cMode;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		PlayerEvents.OnCommandModeChange(cMode);
	}

	public void OnCancelConfirm()
	{
		Mode = LastMode;
		cMode = LastcMode;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		PlayerEvents.OnCommandModeChange(cMode);
		Sound.Play(Error);
	}
	public void OnConfirmEnd()
	{
		Mode = FocusMode.NA;
		cMode = CommandMode.NA;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		PlayerEvents.OnCommandModeChange(cMode);		
	}
	
	public void SwitchFocusMode()
	{
		switch(Mode)
		{
			case FocusMode.Menu:
				Mode = FocusMode.FreeLook;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				//GameCursor.Instance.ActiveToggle();
				return;
			case FocusMode.FreeLook:
				Mode = FocusMode.Menu;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				//GameCursor.Instance.ActiveToggle();
				return;
		}
	}
}

public enum FocusMode
{
	NA,
	Menu,
	ConfirmMenu,
	FreeLook,
	CommandSelectLook,
}

public enum CommandMode
{
	NA,
	MoveSelect,
	AttackSelect,
	AbilitySelect,
	WaitSelect,
}
