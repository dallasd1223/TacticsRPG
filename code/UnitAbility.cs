using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitAbility : Component
{
	[Property] TileInteract Interact {get; set;}
	public List<TileData> TempFinalTiles = new();
	public List<TileData> FinalTiles = new();
	public event Action AbilitySelectionEnd;

	protected override void OnAwake()
	{
		Interact = GetComponent<TileInteract>();
		AbilitySelectionEnd += OnAbilitySelectionEnd;
	}

	public void EndAbilitySelect()
	{
		AbilitySelectionEnd?.Invoke();
	}
	public void OnAbilitySelectionEnd()
	{
		Interact.IsAbilitySelecting = false;
		Interact.RemoveTileHighlights(Interact.abilityTiles);
		ResetRemoveAbilityTiles();
	}

	public void MoveTempToFinalTiles()
	{
		FinalTiles = TempFinalTiles;
		Log.Info($"Final Tiles Count: {FinalTiles.Count()}");
	}

	public void SetTempFinalTiles(TileData target, AOEData aoe)
	{
		ForceLastTileHighlight();
		TempFinalTiles = Interact.GetTilesInRangeFromShape(target, aoe);
		Interact.HighlightTilesFromList(TempFinalTiles, HighlightType.Ability);
	}

	public void ForceLastTileHighlight()
	{
		if(TempFinalTiles.Any() == true)
		{
			Log.Info("Has Some");
			foreach(TileData t in TempFinalTiles)
			{
				t.ApplyHighlightMat(t.LastHighlightType);
			}
			TempFinalTiles.Clear();
		}
	}

	public void ResetRemoveTempFinalTiles()
	{
		Log.Info("Removing Temp Tiles");
		foreach(TileData tile in TempFinalTiles)
		{
			tile.ResetTile();
		}
		TempFinalTiles.Clear();
	}

	public void ResetRemoveFinalTiles()
	{
		Log.Info("Removing Temp Tiles");
		foreach(TileData tile in FinalTiles)
		{
			tile.ResetTile();
		}
		FinalTiles.Clear();
	}

	public void ResetRemoveAbilityTiles()
	{
		Log.Info("Removing Tiles");
		foreach(TileData tile in Interact.abilityTiles)
		{
			tile.ResetTile();
		}
		Interact.abilityTiles.Clear();
	}
}
