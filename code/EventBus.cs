using Sandbox;

namespace TacticsRPG;

public static class InputEvents
{
	//Specific To Each BattleState
	public static event Action<InputKey> BattleStartInputPressed;
	public static event Action<InputKey> TurnStartInputPressed;
	public static event Action<InputKey> ActionSelectInputPressed;
	public static event Action<InputKey> ExecuteActionInputPressed;
	public static event Action<InputKey> PostActionInputPressed;
	public static event Action<InputKey> TurnEndInputPressed;
	public static event Action<InputKey> BattleEndInputPressed;

	public static void OnBattleStartInputPressed(InputKey key)
	{
		BattleEndInputPressed?.Invoke(key);
	}

	public static void OnTurnStartInputPressed(InputKey key)
	{
		TurnStartInputPressed?.Invoke(key);
	}

	public static void OnActionSelectPressed(InputKey key)
	{
		Log.Info($"Action Select Key Event {key}");
		ActionSelectInputPressed?.Invoke(key);
	}

	public static void OnExecuteActionPressed(InputKey key)
	{
		ExecuteActionInputPressed?.Invoke(key);
	}

	public static void OnPostActionPressed(InputKey key)
	{
		PostActionInputPressed?.Invoke(key);
	}

	public static void OnTurnEndInputPressed(InputKey key)
	{
		TurnEndInputPressed?.Invoke(key);
	}

	public static void OnBattleEndPressed(InputKey key)
	{
		BattleEndInputPressed?.Invoke(key);
	}
}

public static class PlayerEvents
{
	public static event Action<FocusMode?, BattleUnit> FocusModeChange;
	public static event Action<CommandMode?> CommandModeChange;
	public static event Action<Ability> AbilitySelected;
	public static event Action<SelectorState> ValidSelection;
	public static event Action<BattleUnit, TileData> TileHovered;
	public static event Action<BattleUnit> UnitSelected;
	public static event Action<BattleUnit> CancelCommand;
	public static event Action CancelSelection;
	public static event Action<Command> ConfirmAction;
	public static event Action ActionConfirmed;

	public static void OnUnitSelected(BattleUnit u)
	{
		UnitSelected?.Invoke(u);
		Log.Info($"Unit Selected Event: {u?.CoreData.Name ?? "null"}" );
	}
	public static void OnTileHovered(BattleUnit u, TileData t)
	{
		TileHovered?.Invoke(u, t);
		Log.Info($"Tile Hovered Event: {u?.CoreData.Name ?? "null"} {t?.TileIndex.ToString() ?? "null"}" );
	}

	public static void OnCancelCommand(BattleUnit u)
	{
		CancelCommand?.Invoke(u);
	}
	public static void OnActionConfirmed()
	{
		ActionConfirmed?.Invoke();
	}
	public static void OnConfirmAction(Command command)
	{
		ConfirmAction?.Invoke(command);
	}

	public static void OnCancelSelection()
	{
		CancelSelection?.Invoke();
	}
	public static void OnValidSelection(SelectorState state)
	{
		ValidSelection?.Invoke(state);
		Log.Info("Valid Selection Event");
	}

	public static void OnAbilitySelected(Ability ability)
	{
		AbilitySelected?.Invoke(ability);
		Log.Info("Ability Selected Event: " + ability.Data.Name);
	}

	public static void OnFocusModeChange(FocusMode? mode, BattleUnit u)
	{
		FocusModeChange?.Invoke(mode, u);
		Log.Info("Focus Mode Change Event: " + mode.ToString());
	}

	public static void OnCommandModeChange(CommandMode? mode)
	{
		CommandModeChange?.Invoke(mode);
	}
	
}
public static class BattleEvents
{
	public static event Action<Battlestate> StateChanged;

	public static event Action<BattleUnit> TurnStart;
	public static event Action<TurnEventArgs> TurnEvent;
	public static event Action<BattleUnit> TurnEnd;
	public static event Action<BattleUnit> ActionSelectStart;
	public static event Action CombatStart;
	public static event Action CombatEnd;

	public static void OnActionSelectStart(BattleUnit u)
	{
		ActionSelectStart?.Invoke(u);
	}
	
	public static void StateHasChanged(Battlestate state)
	{
		StateChanged?.Invoke(state);
		Log.Info($"State Changed Event: {state}");
	}

	public static void OnTurnStart(BattleUnit u)
	{
		TurnStart?.Invoke(u);
	}

	public static void OnTurnEvent(TurnEventArgs args)
	{
		Log.Info(args);
		TurnEvent?.Invoke(args);
	}
	public static void OnTurnEnd(BattleUnit u)
	{
		TurnStart?.Invoke(u);
	}

}
public static class UnitEvents
{
	public static event Action<BattleUnit, BattleUnit> Attacked;
	public static event Action<BattleUnit, TileData> Moved;
	public static event Action<BattleUnit> Died;
	public static event Action<BattleUnit, Ability> UsedAbility;
	public static event Action<BattleUnit, Item> UsedItem;
	public static event Action<BattleUnit> LeveledUp;

	public static void UnitAttacked(BattleUnit u, BattleUnit t)
	{
		Attacked?.Invoke(u,t);
	}

	public static void UnitLeveledUp(BattleUnit u)
	{
		LeveledUp?.Invoke(u);
	}

	public static void OnUnitMoved(BattleUnit u, TileData t)
	{
		Moved?.Invoke(u, t);
	}

	public static void OnUnitDied(BattleUnit u)
	{
		Died?.Invoke(u);
	}

	public static void OnUsedAbility(BattleUnit u, Ability a)
	{
		UsedAbility?.Invoke(u, a);
	}

	public static void OnUsedItem(BattleUnit u, Item i )
	{
		UsedItem?.Invoke(u, i);
	}
}


