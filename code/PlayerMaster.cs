using Sandbox;

namespace TacticsRPG;
public partial class PlayerMaster : SingletonComponent<PlayerMaster>
{
	//URGENT
	//REFACTOR THIS CLASS: SPLIT RESPONSABILITIES WITH NEW BATTLEMANAGER STATES 
	//& NEW SELECTOR MANAGER, AND MORE IF NEEDED
	//URGENT
	[Property] public Unit CurrentUnit {get; set;} = null;
	[Property] public FocusMode? LastMode {get; set;} = null;
	[Property] public FocusMode? Mode {get; set;} = FocusMode.NA;
	[Property] public SelectorManager Selector {get; set;}
	[Property] public InventoryManager Inventory {get; set;}
	[Property] public CommandMode? LastcMode {get; set;} = null;
	[Property] public CommandMode? cMode {get; set;} = CommandMode.NA;
	[Property] public CommandType? CurrentSelectedCommand {get; set;} = null;
	[Property] public IAbilityItem CurrentCommandAbility {get; set;} = null;
	[Property] public ActionMenu Menu {get; set;}
	[Property] public ConfirmUI ConfirmMenu {get; set;}
	[Property] public ConfirmManager Confirm {get; set;}
	[Property] public SoundEvent Error {get; set;}
	[Property] public SoundEvent ConfirmSound {get; set;}
	
	protected override void OnDestroy()
	{
		Log.Info("PlayerMaster Destroyed");
		BattleEvents.TurnEvent -= OnTurnEvent;
		InputEvents.ActionSelectInputPressed -= HandleInput;	
	}
	protected override void OnAwake()
	{
		base.OnAwake();
		/*if(Instance is null)
		{
			Instance = this;
		}
		else
		{
			Log.Info("Yes We Breaking shit");
		BattleEvents.TurnEvent -= OnTurnEvent;
		InputEvents.ActionSelectInputPressed -= HandleInput;
			Instance = null;
			Instance = this;
		}*/
		BattleEvents.TurnEvent += OnTurnEvent;
		InputEvents.ActionSelectInputPressed += HandleInput;

	}

	public void OnTurnEvent(TurnEventArgs args)
	{
		switch(args.state)
		{
			case TurnState.Active:
				if(args.team == TeamType.Alpha)
				{
					Log.Info("Turn initiate");
					InitiatePlayerMaster(args.unit);
				}
				return;
			default:
				return;
		}

	}
	
	public void InitiatePlayerMaster(Unit u)
	{
		Log.Info($"Yes U {u}");
			CurrentUnit = u;
			Mode = FocusMode.Menu;
			PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
	
	}

	public void CancelCommand()
	{
		if(Mode == FocusMode.FreeLook && (cMode == CommandMode.MoveSelect || cMode == CommandMode.AttackSelect || cMode == CommandMode.AbilitySelect))
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
			Menu.DoReset();

			GameCursor.Instance.DeactivateCursor();
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
					Mode = FocusMode.FreeLook;
					cMode = CommandMode.MoveSelect;
					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
					PlayerEvents.OnCommandModeChange(cMode);
					CurrentUnit.Interact.FindMoveableTiles();
					GameCursor.Instance.ActivateCursor();
					//Menu.DeactivateMenu();
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
					Mode = FocusMode.FreeLook;
					cMode = CommandMode.AttackSelect;
					PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
					PlayerEvents.OnCommandModeChange(cMode);
					GameCursor.Instance.ActivateCursor();
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
				Mode = FocusMode.FreeLook;
				cMode = CommandMode.AbilitySelect;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);
				GameCursor.Instance.ActivateCursor();
				CurrentUnit.Interact.IsAbilitySelecting = true;
				Log.Info("Were Before Data Part");
				var item = (Item)cItem.AbilityItem;
				if(item is not null)
				{
					Log.Info("Were inside data part");
					CurrentUnit.Interact.FindAbilityTilesFromRange(item.Data.BaseRange, item.Data.CanUseOnSelf);
					CurrentSelectedCommand = command;
					CurrentCommandAbility = item;
					PlayerEvents.OnAbilitItemSelected(CurrentCommandAbility);
					break;
				}
				else
				{
					Log.Info("Bad Data");
					break;
				}
	
			case CommandType.Magic:
				if(cItem is null) return;
				Mode = FocusMode.FreeLook;
				cMode = CommandMode.AbilitySelect;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);
				GameCursor.Instance.ActivateCursor();
				break;
			case CommandType.Skill:
				if(cItem is null) return;
				Mode = FocusMode.FreeLook;
				cMode = CommandMode.AbilitySelect;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);
				GameCursor.Instance.ActivateCursor();
	
				break;
			case CommandType.Wait:
				if(CurrentUnit.Turn.CommandIsActive("WAIT"))
				{
					cMode = CommandMode.WaitSelect;
					PlayerEvents.OnCommandModeChange(cMode);
					CurrentSelectedCommand = command;
					ActivateConfirmMenu();
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

	//THIS IS TERRIBLE, KEEP THIS IN THE UI
	public void AbilityMenuSelect(string text)
	{
		switch(text)
		{
			case "Item":
				Menu.ChangeMenuState(MenuState.Item);
				break;
			case "Magic":
				Menu.ChangeMenuState(MenuState.Magic);
				break;
			case "Skill":
				Menu.ChangeMenuState(MenuState.Skill);
				break;
		}
	}

	public void ConfirmCommand(Unit unit, TileData tile, List<TileData> tileList)
	{
		switch(CurrentSelectedCommand)
		{
			case CommandType.Move:
				BattleManager.Instance.commandHandler.AddCommand(new MoveCommand(unit, tile));
				break;
			case CommandType.Attack:
				CurrentUnit.Attack.EndAttackSelection();
				BattleManager.Instance.commandHandler.AddCommand(new AttackCommand(unit, UnitManager.Instance.GetUnitFromTile(tile)));
				break;
			case CommandType.Item:
				CurrentUnit.Ability.EndAbilitySelect();
				Mode = FocusMode.NA;
				cMode = CommandMode.NA;
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				PlayerEvents.OnCommandModeChange(cMode);
				Menu.DoReset();
				GameCursor.Instance.DeactivateCursor();
				Log.Info($"{tileList.Count()} TileList Count in Confirm Command");
				BattleManager.Instance.commandHandler.AddCommand(new AbilityCommand(CurrentCommandAbility, unit, UnitManager.Instance.GetUnitFromTile(tile), CurrentUnit.Ability.FinalTiles));
				CurrentUnit.Ability.ResetRemoveFinalTiles();
				break;
			case CommandType.Magic:
				break;
			case CommandType.Skill:
				break;
			case CommandType.Wait:
				BattleManager.Instance.commandHandler.AddCommand(new WaitCommand(unit));
				break;
		}

		Mode = FocusMode.NA;
		cMode = CommandMode.NA;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		PlayerEvents.OnCommandModeChange(cMode);
		GameCursor.Instance.DeactivateCursor();
	}

	public void ActivateConfirmMenu()
	{
		LastMode = Mode;
		Mode = FocusMode.ConfirmMenu;
		LastcMode = cMode;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		PlayerEvents.OnCommandModeChange(cMode);
		ConfirmMenu.Activate();
	}
	
	public void DeactivateConfirmMenu()
	{
		Log.Info("Deactivating Confirm Menu");
		ConfirmMenu.IsActive = false;
		Mode = LastMode;
		cMode = LastcMode;
		PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
		PlayerEvents.OnCommandModeChange(cMode);
		Log.Info(ConfirmMenu.IsActive);

	}

	public void ConfirmFinish()
	{
		Sound.Play(ConfirmSound);
		PlayerMaster.Instance.DeactivateConfirmMenu();
		switch(CurrentSelectedCommand)
		{
			case CommandType.Move:
				GameCursor.Instance.SendConfirmData();
				break;
			case CommandType.Attack:
				GameCursor.Instance.SendConfirmData();
				break;
			case CommandType.Item:
				CurrentUnit.Ability.MoveTempToFinalTiles();
				GameCursor.Instance.SendConfirmData();
				break;
			case CommandType.Wait:
				ConfirmCommand(CurrentUnit, CurrentUnit.Interact.UnitTile, null);
				break;
		}
	}

	public void Cancel()
	{
		Sound.Play(Error);
		PlayerMaster.Instance.DeactivateConfirmMenu();
	}
	
	public void SwitchFocusMode()
	{
		switch(Mode)
		{
			case FocusMode.Menu:
				Mode = FocusMode.FreeLook;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				GameCursor.Instance.ActiveToggle();
				return;
			case FocusMode.FreeLook:
				Mode = FocusMode.Menu;
				Log.Info(Mode);
				PlayerEvents.OnFocusModeChange(Mode, CurrentUnit);
				GameCursor.Instance.ActiveToggle();
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
}

public enum CommandMode
{
	NA,
	MoveSelect,
	AttackSelect,
	AbilitySelect,
	WaitSelect,
}
