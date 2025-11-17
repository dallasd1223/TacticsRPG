using Sandbox;

namespace TacticsRPG;
public partial class PlayerMaster : SingletonComponent<PlayerMaster>
{
	//URGENT REFACTOR --MISSION ACCOMPLISHED--

	[Property] public BattleUnit CurrentUnit {get; set;} = null;
	[Property] public BattleUnit SelectedUnit {get; set;} = null;

	[Property] public FocusMode? LastMode {get; set;} = null;
	[Property] public FocusMode? Mode {get; set;} = FocusMode.NA;

	[Property] public InventoryManager Inventory {get; set;}

	[Property] public CommandMode? LastcMode {get; set;} = null;
	[Property] public CommandMode? cMode {get; set;} = CommandMode.NA;
	[Property] public CommandType? CurrentSelectedCommand {get; set;} = null;

	[Property] public Ability CurrentCommandAbility {get; set;} = null;

	[Property] public BattleMenu Menu {get; set;}
	[Property] public FreeLookScreen FreeLookScreen {get; set;}
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

		PlayerEvents.UnitSelected += OnUnitSelected;	
	}
	protected override void OnAwake()
	{
		base.OnAwake();

		BattleEvents.ActionSelectStart += OnActionSelectStart;
		InputEvents.ActionSelectInputPressed += HandleInput;

		Confirm.ConfirmStart += OnConfirmStart;
		Confirm.ConfirmEnd += OnConfirmEnd;
		Confirm.ConfirmCancel += OnCancelConfirm;

		PlayerEvents.UnitSelected += OnUnitSelected;
	}

	public void OnActionSelectStart(BattleUnit u)
	{
		InitiatePlayerMaster(u);
	}
	
	public void OnUnitSelected(BattleUnit u)
	{
		SelectedUnit = u;
		if(Mode == FocusMode.FreeLook)
		{
			Mode = FocusMode.FreeUnitSelectMenu;
			PlayerEvents.OnFocusModeChange(Mode, SelectedUnit);
		}
	}

	public void InitiatePlayerMaster(BattleUnit u)
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
				if(CurrentUnit.Command.CommandIsActive(cItem))
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
				if(CurrentUnit.Command.CommandIsActive(cItem))
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
			case CommandType.Action:
				if(CurrentUnit.Command.CommandIsActive(cItem))
				{
					Menu.ChangeMenuState(MenuState.Action);
				}				
				break;
			case CommandType.Ability:
				if(CurrentUnit.Command.CommandIsActive(cItem))
				{
					Mode = FocusMode.CommandSelectLook;
					cMode = CommandMode.AbilitySelect;

					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
					PlayerEvents.OnCommandModeChange(cMode);

					CurrentUnit.Interact.IsAbilitySelecting = true;

					var ability = cItem.AbilityIns;
					CurrentUnit.Interact.FindAbilityTilesFromRange(ability.Data.BaseRange, ability.Data.CanUseOnSelf);

					CurrentSelectedCommand = command;
					CurrentCommandAbility = ability;

					PlayerEvents.OnAbilitySelected(CurrentCommandAbility);

				}				
				break;
/*		case CommandType.Item:
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
*/
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
				if(CurrentUnit.Command.CommandIsActive(cItem))
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
			case CommandType.Status:

				if(Mode == FocusMode.Menu)
				{	
					LastMode = Mode;
					Mode = FocusMode.StatusMenu;
					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				}
				else if(Mode == FocusMode.FreeUnitSelectMenu)
				{
					LastMode = Mode;
					Mode = FocusMode.StatusMenu;
					PlayerEvents.OnFocusModeChange(Mode, SelectedUnit);
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
	
	public void LastFocusMode()
	{
		Mode = LastMode;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
	}
	public void SwitchFocusMode()
	{
		switch(Mode)
		{
			case FocusMode.Menu:
				LastMode = Mode;
				Mode = FocusMode.FreeLook;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				//GameCursor.Instance.ActiveToggle();
				return;
			case FocusMode.FreeLook:
				LastMode = Mode;
				Mode = FocusMode.Menu;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				//GameCursor.Instance.ActiveToggle();
				return;
			case FocusMode.FreeUnitSelectMenu:
				LastMode = Mode;
				Mode = FocusMode.FreeLook;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, SelectedUnit);
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
	StatusMenu,
	FreeLook,
	CommandSelectLook,
	FreeUnitSelectMenu,
}

public enum CommandMode
{
	NA,
	MoveSelect,
	AttackSelect,
	AbilitySelect,
	WaitSelect,
}
