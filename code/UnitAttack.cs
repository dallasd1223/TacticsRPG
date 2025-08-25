using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public sealed class UnitAttack : Component
{	
	[Property] TileInteract Interact {get; set;}
	public event Action AttackSelectionEnd;

	protected override void OnAwake()
	{
		Interact = GetComponent<TileInteract>();
		AttackSelectionEnd += OnAttackSelectionEnd;
	}


	public void EndAttackSelection()
	{
		AttackSelectionEnd?.Invoke();
	}

	public void OnAttackSelectionEnd()
	{
		Interact.IsAttackSelecting = false;
		Interact.SetTileCurrent(Interact.UnitTile, false);
		Interact.RemoveTileHighlights(Interact.attackableTiles);
	}
	public void RemoveAttackableTiles()
	{
		Log.Info("Removing Tiles");
		foreach(TileData tile in Interact.attackableTiles)
		{
			tile.ResetTile();
		}
		Interact.attackableTiles.Clear();
	}
}
