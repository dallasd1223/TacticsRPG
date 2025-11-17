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

	private static void OnUnitLevelUp(BattleUnit u)
	{
		Add("LevelUp", u, null, $"{u.CoreData.Name} Has Leveled Up", null);
	}
	
	private static void OnUnitMoved(BattleUnit u, TileData t)
	{
		Add("Moved", u, null, $"{u.CoreData.Name} Has Moved To Tile {t.TileIndex} **LOG", null);
	}

	public static void DestoryEventHooks()
	{
		UnitEvents.LeveledUp -= OnUnitLevelUp;
		UnitEvents.Moved -= OnUnitMoved;		
	}
}
