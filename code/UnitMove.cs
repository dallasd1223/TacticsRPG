using Sandbox;
using System;

namespace TacticsRPG;

public class UnitMove: TileInteract
{
	Vector3 startPos;

	public event Action MoveStarted;
	public event Action MoveComplete;

	protected override void OnAwake()
	{
		MoveStarted += OnMoveStarted;
		MoveComplete += OnMoveComplete;
	}

	protected override void OnUpdate()
	{

		if (moving && !IsMoveSelecting)
		{
			Move();
			timeLerped += Time.Delta;
		}

	}

	public void MoveToTile(TileData tile)
	{
		path.Clear();
		tile.target = true;
		moving = true;
		startPos = WorldPosition;

		TileData next = tile;
		while (next != null)
		{
			Log.Info($"Tile {next.TileIndex} Pushed To Path");
			path.Push(next);
			next = next.parent;
		}
		MoveStarted?.Invoke();
	}

	public void Move()
	{
		Log.Info($"Path Count {path.Count}");
		if(path.Count > 0)
		{
			TileData t = path.Peek(); 
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
				path.Pop();
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
		RemoveTileHighlights(moveableTiles);
	}

	public void OnMoveComplete()
	{
		Log.Info("Move Complete");
		RemoveMoveableTiles();
		ResetRemoveOccupiedTiles();
		RemoveUnitTile();
		GetUnitTile();
		moving = false;

	}


}
