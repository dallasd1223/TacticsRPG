using Sandbox;

namespace TacticsRPG;
public static class PlayerEvents
{
	public static event Action<FocusMode?, Unit> FocusModeChange;
	public static event Action<CommandMode?> CommandModeChange;

	public static void OnFocusModeChange(FocusMode? mode, Unit u)
	{
		FocusModeChange?.Invoke(mode, u);
	}

	public static void OnCommandModeChange(CommandMode? mode)
	{
		CommandModeChange?.Invoke(mode);
	}
	
}
public static class BattleEvents
{
	public static event Action<Battlestate> StateChanged;

	public static event Action<Unit> TurnStart;
	public static event Action<Unit> TurnEnd;
	public static event Action CombatStart;
	public static event Action CombatEnd;

	public static void StateHasChanged(Battlestate state)
	{
		StateChanged?.Invoke(state);
		Log.Info($"State Changed Event: {state}");
	}

	public static void OnTurnStart(Unit u)
	{
		TurnStart?.Invoke(u);
	}

	public static void OnTurnEnd(Unit u)
	{
		TurnStart?.Invoke(u);
	}

}
public static class UnitEvents
{
	public static event Action<Unit, Unit> Attacked;
	public static event Action<Unit, TileData> Moved;
	public static event Action<Unit> Died;
	public static event Action<Unit, IAbilityItem> UsedAbility;
	public static event Action<Unit, Item> UsedItem;
	public static event Action<Unit> LeveledUp;

	public static void UnitAttacked(Unit u, Unit t)
	{
		Attacked?.Invoke(u,t);
	}

	public static void UnitLeveledUp(Unit u)
	{
		LeveledUp?.Invoke(u);
	}

	public static void OnUnitMoved(Unit u, TileData t)
	{
		Moved?.Invoke(u, t);
	}

	public static void OnUnitDied(Unit u)
	{
		Died?.Invoke(u);
	}

	public static void OnUsedAbility(Unit u, IAbilityItem a)
	{
		UsedAbility?.Invoke(u, a);
	}

	public static void OnUsedItem(Unit u, Item i )
	{
		UsedItem?.Invoke(u, i);
	}
}


