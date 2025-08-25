using Sandbox;
using System;

namespace TacticsRPG;

//Separate TileInteract From The ActionClass (Move, Attack, Ability)
[Category("Unit")]
public class UnitMove: Component
{
	[Property] TileInteract Interact {get; set;}
	Vector3 startPos;

	public bool Moving;

	public float timeToLerp = 0.35f;
	public float timeLerped = 0.0f;

	public float moveSpeed = 2f;

	public event Action MoveStarted;
	public event Action MoveComplete;

	protected override void OnAwake()
	{
		Interact = GetComponent<TileInteract>();
		MoveStarted += OnMoveStarted;
		MoveComplete += OnMoveComplete;
	}

	protected override void OnUpdate()
	{

		if (Moving && !Interact.IsMoveSelecting)
		{
			Move();
			timeLerped += Time.Delta;
		}

	}

	public void MoveToTile(TileData tile)
	{
		Interact.path.Clear();
		tile.target = true;
		Moving = true;
		startPos = WorldPosition;

		TileData next = tile;
		while (next != null)
		{
			Log.Info($"Tile {next.TileIndex} Pushed To Path");
			Interact.path.Push(next);
			next = next.parent;
		}
		MoveStarted?.Invoke();
		UnitEvents.OnUnitMoved(Interact.ThisUnit, tile);
	}

	public void Move()
	{
		Log.Info($"Path Count {Interact.path.Count}");
		if(Interact.path.Count > 0)
		{
			TileData t = Interact.path.Peek(); 
			Vector3 target = t.GameObject.WorldPosition + new Vector3(0,0,3.5f);

			if(Vector3.DistanceBetween(this.GameObject.WorldPosition, target) >= 0.05)
			{
				WorldPosition = Vector3.Lerp(startPos, target, timeLerped/timeToLerp);
			}
			else
			{
				this.WorldPosition = target;
				timeLerped = 0.0f;
				startPos = WorldPosition;
				Interact.path.Pop();
			}
		}
		else
		{
			MoveComplete?.Invoke();
		}
	}

	public void OnMoveStarted()
	{
		Log.Info("Move Started");
		Interact.RemoveTileHighlights(Interact.moveableTiles);
	}

	public void OnMoveComplete()
	{

		Log.Info("Move Complete");
		RemoveMoveableTiles();
		Interact.ResetRemoveOccupiedTiles();
		Interact.RemoveUnitTile();
		Interact.GetUnitTile();
		Moving = false;

	}

	public void RemoveMoveableTiles()
	{

		foreach(TileData tile in Interact.moveableTiles)
		{
			Log.Info($"Tile {tile.TileIndex} Reset");
			tile.ResetTile();
		}
		Interact.moveableTiles.Clear();
	}
}
