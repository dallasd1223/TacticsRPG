using Sandbox;

namespace TacticsRPG;

public class TileInteract: Component
{
	public List<TileData> moveableTiles = new List<TileData>();
	public List<TileData> attackableTiles = new List<TileData>();
	public List<TileData> occupiedTiles = new List<TileData>();
	GameObject[] tiles;
	[Property] public TileData UnitTile;

	public bool IsMoveSelecting = false;
	public bool IsAttackSelecting = false;

	public bool moving = false;
	public bool attacking = false;
	public int attackrange = 1;
	public int move = 5;
	public float moveSpeed = 2f;
	
	public float timeToLerp = 0.35f;
	public float timeLerped = 0.0f;

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

	public void SetTileCurrent(TileData t, bool b)
	{
		t.current = b;
		Log.Info($"Tile {t.TileIndex} Is Current {t.current}");
	}

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

	public void SetTileOccupied(TileData t)
	{
		t.IsOccupied = true;
		Log.Info($"Tile {t.TileIndex} Is Occupied");
	}
	public void RemoveMoveableTiles()
	{

		foreach(TileData tile in moveableTiles)
		{
			Log.Info($"Tile {tile.TileIndex} Reset");
			tile.ResetTile();
		}
		moveableTiles.Clear();
	}

	public void ResetRemoveOccupiedTiles()
	{
		if(!occupiedTiles.Any()) return;
		foreach(TileData t in occupiedTiles)
		{
			t.ResetTile();
			t.IsOccupied = true;
		}
	}

	protected void RemoveUnitTile()
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
			tile.highlightType = HighlightType.None;
			tile.ApplyHighlightMat();
		}
	}

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
				t.highlightType = HighlightType.Move;
				t.ApplyHighlightMat();
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
				t.highlightType = HighlightType.Attack;
				t.ApplyHighlightMat();
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
