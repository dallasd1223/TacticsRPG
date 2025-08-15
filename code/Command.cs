using Sandbox;

namespace TacticsRPG;

public abstract class Command
{
	public Unit ThisUnit;
	public bool IsFinished {get; set;} = false;
	
	public abstract void Execute();

	public abstract void Tick();

}

public class MoveCommand: Command
{
	public TileData TargetTile;
	public override void Execute()
	{
		Log.Info($"{ThisUnit.Data.Name} Moving to Tile {TargetTile.TileIndex} at {TargetTile.TilePosition}");
		ThisUnit.Move.IsMoveSelecting = false;
		ThisUnit.Move.MoveToTile(TargetTile);
		ThisUnit.Animator.AssignAnimation();
	}

	public override void Tick()
	{
		if(!ThisUnit.Move.moving)
		{
			Log.Info("Unit Finished Moving");
			ThisUnit.Turn.HasMoved = true;
			ThisUnit.Turn.SetCommand("MOVE", false);
			ThisUnit.Animator.AssignAnimation();
			IsFinished = true;

		}
	}

	public MoveCommand(Unit unit, TileData tile)
	{
		ThisUnit = unit;
		TargetTile = tile;
	}

}

public class AttackCommand: Command
{
	public Unit TargetUnit;
	
	public override void Execute()
	{
		CombatObject obj = new CombatObject(ThisUnit, TargetUnit, AffectType.Single, true);
		Log.Info("Logging Combat Object Info");
		Log.Info($"ComObj {obj.ActingUnit}, {obj.AffectedUnit}, {obj.Affect}, Basic Attack: {obj.BasicAttack}");
		CombatManager.Instance.Add(obj, true);
		CombatManager.Instance.ProcessFinished += OnFinished;
	}

	public void OnFinished()
	{
		IsFinished = true;
		CombatManager.Instance.ProcessFinished -= OnFinished;
	}
	public override void Tick()
	{

	}

	public AttackCommand(Unit unit, Unit target)
	{
		ThisUnit = unit;
		TargetUnit = target;
	}
}

public class WaitCommand: Command
{
	public override void Execute()
	{
		Log.Info($"{ThisUnit.Data.Name} Is Waiting");
		ThisUnit.Turn.HasMoved = true;
		ThisUnit.Turn.HasActed = true;
		ThisUnit.Turn.SetCommand("WAIT", false);
		IsFinished = true;
	}

	public override void Tick()
	{
		Log.Info("Wait Is Ticking (It Shouldnt Be)");
	}

	public WaitCommand(Unit unit)
	{
		ThisUnit = unit;
	}
}
