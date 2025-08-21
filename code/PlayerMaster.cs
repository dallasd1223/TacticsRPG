using Sandbox;

namespace TacticsRPG;
public sealed class PlayerMaster : Component
{
	//URGENT
	//REFACTOR THIS CLASS: SPLIT RESPONSABILITIES WITH NEW BATTLEMANAGER STATES 
	//& NEW SELECTOR MANAGER, AND MORE IF NEEDED
	//URGENT

	public static PlayerMaster Instance {get; set;}
	[Property] public Unit CurrentUnit {get; set;} = null;
	[Property] public FocusMode? LastMode {get; set;} = null;
	[Property] public FocusMode? Mode {get; set;} = FocusMode.NA;
	[Property] public InventoryManager Inventory {get; set;}
	[Property] public CommandMode? LastcMode {get; set;} = null;
	[Property] public CommandMode? cMode {get; set;} = CommandMode.NA;
	[Property] public CommandType? CurrentSelectedCommand {get; set;} = null;
	[Property] public IAbilityItem CurrentCommandAbility {get; set;} = null;
	[Property] public ActionMenu Menu {get; set;}
	[Property] public ConfirmUI ConfirmMenu {get; set;}
	[Property] public SoundEvent Error {get; set;}
	[Property] public SoundEvent ConfirmSound {get; set;}
	

	protected override void OnAwake()
	{
		Instance = this;
	}

	protected override void OnUpdate()
	{

	}

	public void CancelCommand()
	{
		if(Mode == FocusMode.FreeLook && (cMode == CommandMode.MoveSelect || cMode == CommandMode.AttackSelect || cMode == CommandMode.AbilitySelect))
		{
			CurrentUnit.Move.IsMoveSelecting = false;
			CurrentUnit.Move.RemoveMoveableTiles();
			CurrentUnit.Attack.IsAttackSelecting = false;
			CurrentUnit.Attack.RemoveAttackableTiles();
			CurrentUnit.Attack.EndAttackSelection();
			CurrentUnit.Ability.IsAbilitySelecting = false;
			CurrentUnit.Ability.EndAbilitySelect();

			Mode = FocusMode.Menu;
			Menu.Reset();
			GameCursor.Instance.DeactivateCursor();
			CurrentSelectedCommand = null;
			cMode = CommandMode.NA;
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
					CurrentUnit.Move.FindMoveableTiles();
					GameCursor.Instance.ActivateCursor();

					CurrentUnit.Move.IsMoveSelecting = true;
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
					GameCursor.Instance.ActivateCursor();
					CurrentUnit.Attack.IsAttackSelecting = true;
					CurrentUnit.Attack.FindAttackableTiles();
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
				GameCursor.Instance.ActivateCursor();
				CurrentUnit.Ability.IsAbilitySelecting = true;
				Log.Info("Were Before Data Part");
				var item = (Item)cItem.AbilityItem;
				if(item is not null)
				{
					Log.Info("Were inside data part");
					CurrentUnit.Ability.FindAbilityTilesFromRange(item.Data.BaseRange, item.Data.CanUseOnSelf);
					CurrentSelectedCommand = command;
					CurrentCommandAbility = item;
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
				GameCursor.Instance.ActivateCursor();
				break;
			case CommandType.Skill:
				if(cItem is null) return;
				Mode = FocusMode.FreeLook;
				cMode = CommandMode.AbilitySelect;
				GameCursor.Instance.ActivateCursor();
	
				break;
			case CommandType.Wait:
				if(CurrentUnit.Turn.CommandIsActive("WAIT"))
				{
					cMode = CommandMode.WaitSelect;
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
				Menu.Reset();
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
		GameCursor.Instance.DeactivateCursor();
	}

	public void ActivateConfirmMenu()
	{
		LastMode = Mode;
		Mode = FocusMode.ConfirmMenu;
		LastcMode = cMode;
		ConfirmMenu.Activate();
	}
	
	public void DeactivateConfirmMenu()
	{
		Log.Info("Deactivating Confirm Menu");
		ConfirmMenu.IsActive = false;
		Mode = LastMode;
		cMode = LastcMode;
		Log.Info(ConfirmMenu.IsActive);

	}

	public void Confirm()
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
				ConfirmCommand(CurrentUnit, CurrentUnit.Move.UnitTile, null);
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
				GameCursor.Instance.ActiveToggle();
				break;
			case FocusMode.FreeLook:
				Mode = FocusMode.Menu;
				GameCursor.Instance.ActiveToggle();
				break;
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
