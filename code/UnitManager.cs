using Sandbox;

namespace TacticsRPG;

[Category("Manager")]
public sealed class UnitManager : Component
{
	public static UnitManager Instance {get; set;}
	[Property] public List<BattleUnit> UnitList {get; set;}

	protected override void OnAwake()
	{
		Instance = this;
	}
	
	protected override void OnStart()
	{
		UnitList = GetAllUnits();
	}

	public bool TileHasUnit(TileData tile)
	{
		foreach(BattleUnit unit in UnitList)
		{
			if(!unit.Interact.IsValid()) break;
			if(unit.Interact.UnitTile == tile)
			{
				Log.Info("Unit Found");
				return true;
			}
		}
		Log.Info("Unit Not Found");
		return false;
	}

	public List<BattleUnit> GetAllUnits()
	{
		return Scene.GetAll<BattleUnit>().ToList();
	}

	public BattleUnit GetUnitFromTile(TileData data)
	{
		foreach(BattleUnit unit in UnitList)
		{
			if(unit.Interact.UnitTile == data)
			{
				Log.Info($"{unit.CoreData.Name} Found At Tile {data.TileIndex}");
				return unit;
			}
		}
		Log.Info($"No Unit Found At Tile {data.TileIndex}");
		return null;
	}

}
