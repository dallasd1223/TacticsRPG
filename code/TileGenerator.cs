using Sandbox;
using System;

namespace TacticsRPG;

public sealed class TileGenerator : Component
{
	[Property] public string MapName {get; set;} = "MAP 00";
	[Property] public int MaxX {get; set;}
	[Property] public int MaxY {get; set;}
	[Property] public int CellSize {get; set;}
	[Property] public Model TileModel {get; set;}
	public bool IsEditing {get; set;} = false;
	public List<GameObject> TileList = new List<GameObject>();

	protected override void OnAwake()
	{

	}

	[Button("Generate TileMap")]
	public void GenerateTiles()
	{
		var manager = Scene.GetAll<TileMapManager>().FirstOrDefault();
		if(manager.IsValid())
		{
			Log.Info("Remove Existing Mangager to Generate");
			return;
		}
		if(IsEditing)
		{
			Log.Info($"IsEditing: {IsEditing}");
			Log.Info("Already Editing A TileMap.");
			return;
		}
		IsEditing = true;
		int count = 0;
		for(int indexX = 0; indexX < MaxX; indexX++){
			for(int indexY = 0; indexY < MaxY; indexY++){
				count++;
				GameObject test = new GameObject();
				TileList.Add(test);
				test.Name = $"Tile {count}";
				test.WorldPosition = new Vector3(28 + (indexX * CellSize), 28 + (indexY * CellSize), 24.5f);
				test.LocalScale = new Vector3((float)0.56,(float)0.56, 0);
				var tileComp = test.AddComponent<TileDataBuilder>();
				tileComp.TileIndex = count;
				tileComp.XIndex = indexX;
				tileComp.YIndex = indexY;
				tileComp.TilePosition = tileComp.GameObject.WorldPosition;
				var model = test.GetOrAddComponent<ModelRenderer>();
				model.Model = TileModel;
			}	
		}
		Log.Info("TileMap Generated");	
	}

	[Button("Clear TileMap")]
	public void ClearTiles()
	{
		if(TileList.Count() == 0)
		{
			Log.Info("No Tiles Found In Edit List");
			return;
		}

		if(!IsEditing)
		{
			Log.Info("No Tiles Being Edited.");
		}

		foreach(GameObject tile in TileList)
		{
			tile.Destroy();
		}

		TileList.Clear();
		IsEditing = false;
		Log.Info("TileMap Removed");

	}
	
	[Button("Finish TileMap")]
	public void FinishTileMap()
	{
		var map = Scene.GetAll<TileMapManager>().FirstOrDefault();
		if(map.IsValid())
		{
			Log.Info("Scene Already Contains Finished TileMap!");
			return;
		}
		if(TileList.Count() == 0)
		{
			Log.Info("No Unfinished TileMap Found");
			return;
		}
		Log.Info("Finishing TileMap");
		GameObject tileManager = new GameObject();
		tileManager.Name = $"{MapName} Tiles";
		var manager = tileManager.AddComponent<TileMapManager>();
		manager.MapName = MapName;
		manager.MaxX = MaxX;
		manager.MaxY = MaxY;
		foreach(GameObject tile in TileList)
		{
			var builder = tile.GetComponent<TileDataBuilder>();
			var data = tile.AddComponent<TileData>();
			data.TileIndex = builder.TileIndex;
			data.XIndex = builder.XIndex;
			data.YIndex = builder.YIndex;
			data.HeightIndex = builder.HeightIndex;
			data.SurfaceType = builder.SurfaceType;
			data.TilePosition = builder.TilePosition;
			builder.Destroy();
			tile.Parent = tileManager;
			var node = tile.AddComponent<CameraFocusNode>();
			node.Type = NodeType.Tile;
			manager.TileList.Add(tile.GetComponent<TileData>());
		}
		TileList.Clear();
		IsEditing = false;
		Log.Info($"TileManager Created");
		Log.Info($"{manager.TileList.Count()} Tiles ");
	}
	
	[Button("Edit TileMap")]
	public void EditTilemap()
	{
		if(TileList.Count() > 0)
		{
			Log.Info("Already Editing Tiles");
			return;
		}
		var parent = Scene.GetAll<TileMapManager>().FirstOrDefault();
		if (!parent.IsValid())
		{
			Log.Info("No TileMap Found");
			return;
		}
		foreach(TileData data in parent.TileList)
		{
			data.GameObject.SetParent(null);
			var builder = data.GameObject.AddComponent<TileDataBuilder>();
			builder.TileIndex = data.TileIndex;
			builder.XIndex = data.XIndex;
			builder.YIndex = data.YIndex;
			builder.HeightIndex = data.HeightIndex;
			builder.SurfaceType = data.SurfaceType;
			builder.TilePosition = data.TilePosition;
			TileList.Add(data.GameObject);
			var cam = data.GameObject.GetComponent<CameraFocusNode>();
			if(cam.IsValid())
			{
				cam.Destroy();
			}
			data.Destroy();

		}
		parent.GameObject.Destroy();
		IsEditing = true;
		Log.Info($"{TileList.Count()} Tiles Added For Editing");
		Log.Info("TileMap Ready To Edit!");
	}

	[Button("Force Add Tiles")]
	public void ForceFindTiles()
	{
		var builders = Scene.GetAll<TileDataBuilder>();
		int AmountAdded = 0;
		foreach(TileDataBuilder tile in builders)
		{
			TileList.Add(tile.GameObject);
			AmountAdded++;
		}
		Log.Info($"{AmountAdded} Tiles Found");
		Log.Info($"{TileList.Count()} Total Editing Tiles");
	}

	[Button("Force Clear")]
	public void ForceClear()
	{
		if(TileList.Count() == 0)
		{
			Log.Info("No TileData To Clear");
			IsEditing = false;
			return;
		}
		Log.Info($"{TileList.Count()}");
		TileList.Clear();
		IsEditing = false;
		Log.Info($"{TileList.Count()} Tile List Cleared");
	}
	protected override void OnUpdate()
	{

	}

	public void LoadMapFromFile(string path)
	{

	}

	public void SaveMapToFile()
	{
		
	}
}


