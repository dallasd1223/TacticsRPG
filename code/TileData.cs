using Sandbox;
using System; 

namespace TacticsRPG;

public class TileData : Component
{
	[Property] public int TileIndex {get; set;}
	[Property] public int XIndex {get; set;}
	[Property] public int YIndex {get; set;}
	[Property] public int HeightIndex {get; set;} = 1;
	[Property] public Vector3 TilePosition {get; set;}
	[Property] public TileSurfaceType SurfaceType {get; set;} = TileSurfaceType.NA;
	[Property] public bool IsWalkable {get; set;} = false;
	[Property] public List<TileData> neighborsList = new List<TileData>();
	[Property] public ModelRenderer Model {get; set;}
	[Property] public Material MoveHighlightMat {get; set;}
	[Property] public Material AttackHighlightMat {get; set;}
	[Property] public HighlightType highlightType {get; set;} = HighlightType.Move;
	[Property] public bool current = false;
	[Property] public bool IsOccupied = false;
	public bool target = false;
	[Property] public bool selectable = false;

	[Property] public bool visited = false;
	public TileData parent = null;
	[Property] public int distance = 0;

	protected override void OnAwake()
	{
		MoveHighlightMat = Material.Load("materials/bluetilemat.vmat");
		AttackHighlightMat = Material.Load("materials/redtilemat.vmat");
		highlightType = HighlightType.Move;
		Model = GetComponent<ModelRenderer>();
		ApplyHighlightMat();
		Model.Tint = new Color(1,1,1,0);
	}

	protected override void OnUpdate()
	{
		if(selectable)
		{
			Model.Tint = new Color(Model.Tint.r, Model.Tint.g, Model.Tint.b, ((float)Math.Sin(Time.Now * 3)+1f) * 0.5f);
		}
		else if(target)
		{
			Model.Tint = new Color(1,0,0,1);
		}
		else if(current)
		{
			Model.Tint = new Color(0,1,0,0);
		}
		
	}
	public void FindNeighbors()
	{
		CheckTile(new Vector2(XIndex, YIndex+1));	
		CheckTile(new Vector2(XIndex, YIndex-1));
		CheckTile(new Vector2(XIndex+1, YIndex));
		CheckTile(new Vector2(XIndex-1, YIndex));
	}

	public void CheckTile(Vector2 direction)
	{
		var tile = TileMapManager.Instance.GetTileData(direction);
		if(tile is not null)
		{
			neighborsList.Add(tile);
		}
		else
		{
			Log.Info($"Tile {TileIndex} No Neighbor found at {direction.x}, {direction.y}");
		}
	}
	public void ApplyHighlightMat()
	{
		switch(highlightType)
		{
			case HighlightType.None:
				Model.Tint = new Color(1,1,1,0);
				break;
			case HighlightType.Move:
				Model.SetMaterial(MoveHighlightMat);
				break;
			case HighlightType.Attack:
				Model.SetMaterial(AttackHighlightMat);
				break;
			case HighlightType.Special:
				break;
		}
	}
	public void SetHighlightType(HighlightType type)
	{
		highlightType = type; 
	}
	public void ResetTile()
	{
		current = false;
		target = false;
		selectable = false;
		IsOccupied = false;
		visited = false;
		parent = null;
		distance = 0;
		Model.Tint = new Color(1,1,1,0);
	}
}

public enum TileSurfaceType
{
	NA,
	Grass,
	River,
	Stone,
}

public enum HighlightType
{
	None,
	Move,
	Attack,
	Special,
}
