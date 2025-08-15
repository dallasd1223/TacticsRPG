using Sandbox;
using System;

namespace TacticsRPG;

public sealed class UnitAttack : TileInteract
{

	public event Action AttackSelectionEnd;

	protected override void OnAwake()
	{
		AttackSelectionEnd += OnAttackSelectionEnd;
	}


	public void EndAttackSelection()
	{
		AttackSelectionEnd?.Invoke();
	}

	public void OnAttackSelectionEnd()
	{
		IsAttackSelecting = false;
		SetTileCurrent(UnitTile, false);
		RemoveTileHighlights(attackableTiles);
	}
	public void RemoveAttackableTiles()
	{
		Log.Info("Removing Tiles");
		foreach(TileData tile in attackableTiles)
		{
			tile.ResetTile();
		}
		attackableTiles.Clear();
	}
}
