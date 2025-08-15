using Sandbox;

namespace TacticsRPG;

public sealed class UnitManager : Component
{
	public static UnitManager Instance {get; set;}
	[Property] public List<Unit> UnitList {get; set;}

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
		foreach(Unit unit in UnitList)
		{
			if(!unit.Move.IsValid()) break;
			if(unit.Move.UnitTile == tile)
			{
				Log.Info("Unit Found");
				return true;
			}
		}
		Log.Info("Unit Not Found");
		return false;
	}

	public List<Unit> GetAllUnits()
	{
		return Scene.GetAll<Unit>().ToList();
	}

	public Unit GetUnitFromTile(TileData data)
	{
		foreach(Unit unit in UnitList)
		{
			if(unit.Move.UnitTile == data)
			{
				Log.Info($"{unit.Data.Name} Found At Tile {data.TileIndex}");
				return unit;
			}
		}
		Log.Info($"No Unit Found At Tile {data.TileIndex}");
		return null;
	}
}
