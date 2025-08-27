using Sandbox;

namespace TacticsRPG; 

[Category("Manager")]
public class TileMapManager : Component
{
	public static TileMapManager Instance;
	[Property] public int MaxX {get; set;}
	[Property] public int MaxY {get; set;}
	[Property] public string MapName {get; set;}
	public List<TileData> TileList = new List<TileData>();

	protected override void OnAwake()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		foreach(GameObject child in this.GameObject.Children)
		{
			var data = child.GetComponent<TileData>();
			if(data.IsValid())
			{
				TileList.Add(data);
			}

		}
		ComputeAllNeighbors();
		Log.Info($"{TileList.Count()} Tiles");
	}

	public void ComputeAllNeighbors()
	{
		foreach(TileData tile in TileList)
		{
			tile.FindNeighbors();
		}
	}

	public TileData GetTileData(int index)
	{
		return TileList[index];
	}

	public bool TileIsValid(Vector2 vec)
	{
		var tile = GetTileData(vec);
		if(tile is null) return false;
		else return true;
	}
	public TileData FindTileFromIndex(int index)
	{
		foreach(TileData tile in TileList)
		{
			if(tile.TileIndex == index)
			{
				Log.Info($"Tile Matching Index {index} Found");
				return tile;
			}
		}
		Log.Info($"No Tile Matching {index} Found");
		return null;
	}

	public TileData GetTileData(Vector2 index)
	{
		TileData FoundTile = null;
		foreach(TileData tile in TileList)
		{
			if(tile.XIndex == index.x && tile.YIndex == index.y)
			{
				FoundTile = tile;
				break;
			}
		}
		if(FoundTile is not null)
		{
			return FoundTile;
		}
		else
		{
			return null;
		}
	}

	public GameObject GetTileObject(int index)
	{
		return TileList[index].GameObject;
	}

	public GameObject GetTileObject(TileData data)
	{
		return TileList[data.TileIndex].GameObject;
	}

	public TileData GetTileFromVector2(Vector2 vec)
	{
		foreach(TileData tile in TileList)
		{
			if(tile.XIndex == vec.x && tile.YIndex == vec.y)
			{
				Log.Info($"{tile.TileIndex} Tile Found");
				return tile;
			}
		}
		Log.Info($"No Tile Found Matching {vec}");
		return null;
	}

	public Vector2 GetVector2FromTile(TileData data)
	{
		return new Vector2(data.XIndex, data.YIndex);
	}

	public Vector3 GetTilePositionFromVector(Vector2 vec)
	{
		TileData tile = GetTileData(vec);
		if(tile.IsValid())
		{
			return tile.TilePosition;
		}
		Log.Info("No TileFound");
		return this.GameObject.WorldPosition;		
	}
}
