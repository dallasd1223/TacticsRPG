using Sandbox;

namespace TacticsRPG;
public sealed class PlayerMaster : Component
{
	public static PlayerMaster Instance {get; set;}
	[Property] public Unit CurrentUnit {get; set;} = null;
	[Property] public FocusMode? LastMode {get; set;} = null;
	[Property] public FocusMode? Mode {get; set;} = FocusMode.NA;
	[Property] public InventoryManager Inventory {get; set;}
	[Property] public CommandMode? LastcMode {get; set;} = null;
	[Property] public CommandMode? cMode {get; set;} = CommandMode.NA;
	[Property] public CommandType? CurrentSelectedCommand {get; set;} = null;
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
		if(Mode == FocusMode.FreeLook && (cMode == CommandMode.MoveSelect || cMode == CommandMode.AttackSelect))
		{
			CurrentUnit.Move.RemoveMoveableTiles();
			CurrentUnit.Attack.RemoveAttackableTiles();
			CurrentUnit.Attack.EndAttackSelection();
			CurrentUnit.Move.IsMoveSelecting = false;
			CurrentUnit.Attack.IsAttackSelecting = false;
			Mode = FocusMode.Menu;
			GameCursor.Instance.DeactivateCursor();
			CurrentSelectedCommand = null;
			cMode = CommandMode.NA;
			Sound.Play(Error);
		}
	}

	public void SelectedCommand(CommandType? command)
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
		}
	}

	public void ConfirmCommand(Unit unit, TileData tile)
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
			case CommandType.Wait:
				ConfirmCommand(CurrentUnit, CurrentUnit.Move.UnitTile);
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
	WaitSelect,
}
