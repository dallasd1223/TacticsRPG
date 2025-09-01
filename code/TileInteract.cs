using Sandbox;

namespace TacticsRPG;

//Somewhat Fixed After Decoupling, NOW just clean up reduntant tilefinding functions

public class TileInteract: Component
{
	public List<TileData> moveableTiles = new List<TileData>();
	public List<TileData> attackableTiles = new List<TileData>();
	public List<TileData> abilityTiles = new List<TileData>();
	public List<TileData> occupiedTiles = new List<TileData>();

	[Property] public TileData UnitTile;

	public bool IsMoveSelecting = false;
	public bool IsAttackSelecting = false;
	public bool IsAbilitySelecting = false;

	public bool moving = false;
	public bool attacking = false;
	public int attackrange = 1;
	public int move = 5;

	[Property] public Unit ThisUnit;

	public Stack<TileData> path = new Stack<TileData>();

	protected override void OnAwake()
	{
		ThisUnit = GetComponent<Unit>();
		Log.Info(ThisUnit);
	}

	protected override void OnStart()
	{
		GetUnitTile();
	}
	
	//Get Units Occupied Tile
	public void GetUnitTile()
	{
		Log.Info("Getting Current Unit Tile");
		UnitTile = GetTargetTile(this.GameObject);
		if(UnitTile is null)
		{
			Log.Info("No Current Tile");
			return;
		}
		SetTileOccupied(UnitTile);
	}
	
	//Set Tile As Current (Active Turn Unit's Tile)
	public void SetTileCurrent(TileData t, bool b)
	{
		t.current = b;
		Log.Info($"Tile {t.TileIndex} Is Current {t.current}");
	}

	//Gets Tile From GameObject (Probably Better In Another Class)
	public TileData GetTargetTile(GameObject target)
	{
		foreach(TileData tile in TileMapManager.Instance.TileList)
		{
			if(tile.TilePosition.x == target.WorldPosition.x && tile.TilePosition.y == target.WorldPosition.y)
			{
				Log.Info($"{tile.TileIndex} Tile Set Current");
				return tile;
			}
		}
		Log.Info("No Target Tile Found");
		return null;
	}

	//Granular Set Tile As Occupied
	public void SetTileOccupied(TileData t)
	{
		t.IsOccupied = true;
		Log.Info($"Tile {t.TileIndex} Is Occupied");
	}

	//Reset And Remove Tiles Inside moveableTiles List<TileData>


	//Reset And Falsify IsOccupied on All Touched OccupiedTiles
	public void ResetRemoveOccupiedTiles()
	{
		if(!occupiedTiles.Any()) return;
		foreach(TileData t in occupiedTiles)
		{
			t.ResetTile();
			t.IsOccupied = true;
		}
	}

	//Remove & Reset This Units Current Tile
	public void RemoveUnitTile()
	{
		Log.Info("Removing Current Tile");
		
		if(UnitTile != null)
		{
			UnitTile.ResetTile();
			UnitTile.visited = false;
			UnitTile.current = false;
			UnitTile = null;
		}
	}

	public void ResetTile(TileData t)
	{
		Log.Info($"Tile {t.TileIndex} Reset");
		t.ResetTile();
	}

	public void RemoveTileHighlights(List<TileData> tiles)
	{
		foreach(TileData tile in tiles)
		{
			tile.selectable = false;
			tile.ApplyHighlightMat(HighlightType.None);
		}
	}

	public void HighlightTilesFromList(List<TileData> list, HighlightType Type)
	{
		foreach(TileData t in list)
		{

			t.ApplyHighlightMat(Type);
		}
	}
	//Finds Moveable Tiles from CurrentTile Using Move Distance As Range
	public void FindMoveableTiles()
	{
		GetUnitTile();
		SetTileCurrent(UnitTile, true);
	
		if(UnitTile is null)
		{
			Log.Info("No Current Tile");
			return;
		}
		Queue<TileData> process = new Queue<TileData>();

		process.Enqueue(UnitTile);
		UnitTile.visited = true;
		while (process.Count > 0)
		{
			TileData t = process.Dequeue();
			Log.Info($"Tile {t.TileIndex} Dequeud");
			if(t.current == false && !t.IsOccupied)
			{
				Log.Info("Moveable tile added");
				moveableTiles.Add(t);
				t.selectable = true;
				t.ApplyHighlightMat(HighlightType.Move);
			}
				else if(t.IsOccupied && t != UnitTile)
			{
				occupiedTiles.Add(t);
			}
			if(t.distance < move)
			{
				foreach(TileData tile in t.neighborsList)
				{
					if(!tile.visited)
					{
						Log.Info($"Tile {tile.TileIndex} Parent Set To Tile {t.TileIndex}");
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						Log.Info($"{tile.TileIndex} Tile Distance {tile.distance}");
						process.Enqueue(tile);
					}
				}
			}

		}
	}

	//Returns A List Of MoveableTiles From Units Current Tile
	public List<TileData> ForceFindTempMoveableTiles()
	{
		GetUnitTile();
		SetTileCurrent(UnitTile, true);
		if(UnitTile is null)
		{
			Log.Info("No Current Tile");
			return null;
		}

		List<TileData> tempMoveableTiles = new List<TileData>(); 
		Queue<TileData> process = new Queue<TileData>();

		process.Enqueue(UnitTile);
		UnitTile.visited = true;
		
		while (process.Count > 0)
		{
			TileData t = process.Dequeue();
			Log.Info($"Tile {t.TileIndex} Occupied: {t.IsOccupied}");
			if(t.current == false)
			{
				Log.Info($"Moveable Tile Added {t.TileIndex}");
				tempMoveableTiles.Add(t);
			}
			else if(t.IsOccupied && t != UnitTile)
			{
				occupiedTiles.Add(t);
			}
			if(t.distance < move)
			{
				foreach(TileData tile in t.neighborsList)
				{
					if(!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						Log.Info($"Distance Added To Tile {tile.TileIndex} In TempMove");
						tile.distance = 1 + t.distance;
						Log.Info($"{tile.TileIndex} Tile Distance {tile.distance}");
						process.Enqueue(tile);
					}
				}
			}

		}
		return tempMoveableTiles;
	}	
	
	public void ResetTilesFromList(List<TileData> tiles)
	{
		foreach(TileData tile in tiles)
		{
			Log.Info($"Tile {tile.TileIndex} Reset");
			tile.ResetTile();
		}
	}

	public List<TileData> GetMoveableTiles()
	{
		if(moveableTiles.Any())
		{
			return moveableTiles;
		}
		else
		{
			Log.Info("No Walkable Tiles In List");
			return null;
		}
	}

	//Finds AttackableTiles From Current Tile
	public void FindAttackableTiles()
	{
		GetUnitTile();
		SetTileCurrent(UnitTile, true);
		if(UnitTile is null)
		{
			Log.Info("No Current Tile");
			return;
		}
		Queue<TileData> process = new Queue<TileData>();

		process.Enqueue(UnitTile);
		UnitTile.visited = true;
		
		while (process.Count > 0)
		{
			TileData t = process.Dequeue();

			if(t.current == false)
			{
				Log.Info($"Tile {t.TileIndex} Added To Attackable Tiles");
				attackableTiles.Add(t);
				t.selectable = true;
				t.ApplyHighlightMat(HighlightType.Attack);
			}
			if(t.distance < attackrange)
			{
				foreach(TileData tile in t.neighborsList)
				{
					if(!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						Log.Info($"Distance Added To Tile {tile.TileIndex} In TempMove");
						tile.distance = 1 + t.distance;
						Log.Info($"{tile.TileIndex} Tile Distance {tile.distance}");
						process.Enqueue(tile);
					}
				}
			}
		}
	}


	//Finds AttackableTiles From Start Tile & Range
	public void FindAbilityTilesFromRange(int range, bool SelectCenter = false)
	{
		GetUnitTile();
		SetTileCurrent(UnitTile, true);

		if(UnitTile is null)
		{
			Log.Info("No Start Tile");
			return;
		}
		
		List<TileData> ATiles = new List<TileData>();
		Queue<TileData> process = new Queue<TileData>();


		process.Enqueue(UnitTile);
		UnitTile.visited = true;
		
		while (process.Count > 0)
		{
			TileData t = process.Dequeue();

			if(SelectCenter)
			{
				Log.Info($"Tile {t.TileIndex} Added To Ability Tiles");
				ATiles.Add(t);
				t.selectable = true;
				t.ApplyHighlightMat(HighlightType.Attack);			
			}
			else if(t.current == false)
			{
				Log.Info($"Tile {t.TileIndex} Added To Ability Tiles");
				ATiles.Add(t);
				t.selectable = true;
				t.ApplyHighlightMat(HighlightType.Attack);
			}
			if(t.distance < range)
			{
				foreach(TileData tile in t.neighborsList)
				{
					if(!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						Log.Info($"Distance Added To Tile {tile.TileIndex} In TempMove");
						tile.distance = 1 + t.distance;
						Log.Info($"{tile.TileIndex} Tile Distance {tile.distance}");
						process.Enqueue(tile);
					}
				}
			}
		}
		abilityTiles = ATiles;
		return;
	}

	public List<TileData> GetTilesInRangeFromShape(TileData TargetTile, AOEData AOE)
	{
		List<TileData> inRangeTiles = new();
		if(AOE.Range == 0)
		{
			inRangeTiles.Add(TargetTile);
			return inRangeTiles;
		}
		for(int x = -AOE.Range; x <= AOE.Range; x++)
		{

			for(int y = -AOE.Range; y <= AOE.Range; y++)
			{
				int tx = TargetTile.XIndex + x;
				int ty = TargetTile.YIndex + y;

				if(!TileMapManager.Instance.TileIsValid(new Vector2(tx, ty))) continue;
				if(!AOE.KeepCenter && x == 0 && y == 0) continue;

				switch(AOE.Shape)
				{
					case RangeShape.Line:
						break;
					case RangeShape.Square:
						inRangeTiles.Add(TileMapManager.Instance.GetTileFromVector2(new Vector2(tx, ty)));
						break;
					case RangeShape.Cross:
						if(x == 0 || y == 0)
							inRangeTiles.Add(TileMapManager.Instance.GetTileFromVector2(new Vector2(tx, ty)));					
						break;
					case RangeShape.Donut:
						break;
					case RangeShape.Diagonal:
						if(Math.Abs(x) + Math.Abs(y) <= AOE.Range)
							inRangeTiles.Add(TileMapManager.Instance.GetTileFromVector2(new Vector2(tx, ty)));
						break;
				}
			}
		}


		return inRangeTiles;
	}

	//Returns List Of Attackable Tiles From Current Tile
	public List<TileData> FindTempAttackableTilesFromTile(TileData startTile)
	{
		if(startTile is null)
		{
			Log.Info("No Start Tile");
			return null;
		}
		List<TileData> attackTiles = new List<TileData>();
		Queue<TileData> process = new Queue<TileData>();

		process.Enqueue(startTile);
		
		while (process.Count > 0)
		{
			TileData t = process.Dequeue();
			Log.Info($"Tile {t.TileIndex} Occupied: {t.IsOccupied}");
			Log.Info($"Attackable Tile Added {t.TileIndex}");
			if(t != startTile)
			{
				attackTiles.Add(t);
			}
	

			if(t.distance < attackrange)
			{
				foreach(TileData tile in t.neighborsList)
				{
					if(!tile.visited)
					{
						tile.parent = t;
						tile.visited = true;
						tile.distance = 1 + t.distance;
						Log.Info($"{tile.TileIndex} Tile Distance {tile.distance}");
						process.Enqueue(tile);
					}
				}
			}

		}

		return attackTiles;
	}
}
