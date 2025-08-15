using Sandbox;

namespace TacticsRPG;

public static class UnitEvents
{
	public static event Action<Unit, Unit> Attacked;
	public static event Action<Unit, TileData> Moved;
	public static event Action<Unit> Died;
	public static event Action<Unit, Ability> UsedAbility;
	public static event Action<Unit, Item> UsedItem;
	public static event Action<Unit> LeveledUp;

	public static void UnitAttacked(Unit u, Unit t)
	{
		Attacked?.Invoke(u,t);
	}
}
