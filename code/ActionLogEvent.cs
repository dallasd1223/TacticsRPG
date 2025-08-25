using Sandbox;
using System;
namespace TacticsRPG;

public static partial class ActionLog
{
	public static void SetupEventHooks()
	{
		UnitEvents.LeveledUp += OnUnitLevelUp;
		UnitEvents.Moved += OnUnitMoved;
	}

	private static void OnUnitLevelUp(Unit u)
	{
		Add("LevelUp", u, null, $"{u.Data.Name} Has Leveled Up", null);
	}
	
	private static void OnUnitMoved(Unit u, TileData t)
	{
		Add("Moved", u, null, $"{u.Data.Name} Has Moved To Tile {t.TileIndex} **LOG", null);
	}
}
