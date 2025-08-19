using Sandbox;
using System;
using System.Threading.Tasks;
namespace TacticsRPG;

public sealed class UnitAI : Component
{

	public Unit Self;
	public bool HasTakenTurn = false;
	public List<TileData> ResetMoveableTiles {get; set;} = new List<TileData>();
	public List<TileData> ResetAttackableTiles {get; set;} = new List<TileData>();	
	protected override void OnStart()
	{
		Self = GetComponent<Unit>();
	}

	public async void TakeTurn()
	{
		Log.Info("AI Taking Turn");
		HasTakenTurn = true;
		AIAction best = null;
		List<AIAction> actionslist = null;
		actionslist = await GatherPossibleActions();
		best = SelectBestAndExecute(actionslist);
		if(best != null)
		{
			best.Execute();
			HasTakenTurn = false;
		}
		else
		{
			Log.Info("NO BEST TURN, RESTART GAME");
		}
		//Self.Move.ResetTilesFromList(ResetMoveableTiles);
	}

	private async Task<List<AIAction>> GatherPossibleActions()
	{
		var actions = new List<AIAction>();
		Log.Info("Getting Temp Move Tiles");
		List<TileData> moveTiles = null;
		moveTiles = Self.Move.ForceFindTempMoveableTiles();
		ResetMoveableTiles = moveTiles;
		if(moveTiles is null) return null;
		foreach(var tile in moveTiles)
		{
			var moveaction = new AIAction
			{
				MoveTile = tile,
				ThisUnit = Self,
				Target = null,
				Attack = false,
			};
			Log.Info("Move Action Added");
			actions.Add(moveaction);
			Log.Info("Getting Temp Attack Tiles");
			var targetTiles = Self.Attack.FindTempAttackableTilesFromTile(tile);
			ResetAttackableTiles = targetTiles;
			foreach(var targetTile in targetTiles)
			{
				if(!UnitManager.Instance.TileHasUnit(targetTile)) continue;

				var target = UnitManager.Instance.GetUnitFromTile(targetTile);

				var action = new AIAction
				{
					MoveTile = tile,
					ThisUnit = Self,
					Target = target,
					Attack = true,
				};
				Log.Info($"Action Added: Tile {action.MoveTile.TileIndex}, Unit {action.Target?.Data.Name}, Attack {action.Attack}");
				actions.Add(action);
			}
			Self.Move.ResetTilesFromList(ResetAttackableTiles);
		}

		return actions;
	}

	public void EndTurn()
	{
		Self.Move.ResetTilesFromList(ResetMoveableTiles);
		Self.Move.GetUnitTile();
	}
	
	private AIAction SelectBestAndExecute(List<AIAction> possibleActions)
	{
		if(!possibleActions.Any()) return null;

		return possibleActions[Game.Random.Int(0,possibleActions.Count()-1)];
	}
}


public class AIAction
{
	public TileData? MoveTile;
	public Unit ThisUnit;
	public Unit Target;
	public bool Attack;
	public float Score;

	public void Execute()
	{
		BattleManager.Instance.commandHandler.AddCommand(new MoveCommand(ThisUnit, MoveTile));
		BattleManager.Instance.commandHandler.AddCommand(new WaitCommand(ThisUnit));	
	}
}

