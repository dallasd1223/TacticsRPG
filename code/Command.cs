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
		ThisUnit.Interact.IsMoveSelecting = false;
		ThisUnit.Move.MoveToTile(TargetTile);
		ThisUnit.Animator.AssignAnimation();
	}

	public override void Tick()
	{
		if(!ThisUnit.Move.Moving)
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
		CombatMachine.Instance.Add(obj, true);
		CombatMachine.Instance.ProcessFinished += OnFinished;
	}

	public void OnFinished()
	{
		ThisUnit.Turn.SetCommand("ATTACK", false);
		ThisUnit.Turn.SetCommand("ABILITY", false);
		IsFinished = true;
		CombatMachine.Instance.ProcessFinished -= OnFinished;
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

public class AbilityCommand: Command
{
	public Unit TargetUnit;
	public List<TileData> TargetTiles;
	public IAbilityItem AbilityItem;

	public override void Execute()
	{	
		AffectType Type;

		List<Unit> targetUnits = new List<Unit>();
		Log.Info($"{TargetTiles.Count()} in Command");
		foreach(TileData t in TargetTiles)
		{
			Log.Info($"TargetTiles {t} in Command");
			targetUnits.Add(UnitManager.Instance.GetUnitFromTile(t));
		}
		if(targetUnits.Count() > 1)
		{
			Type = AffectType.Multi;
		}
		else if(ThisUnit == TargetUnit)
		{
			Type = AffectType.Self;
		}
		else
		{
			Type = AffectType.Single;
		}
		CombatObject obj = new CombatObject(ThisUnit, TargetUnit, Type, false, targetUnits, AbilityItem);
		Log.Info("Logging Combat Object Info");
		Log.Info($"ComObj {obj.AbilityItem}, {obj.ActingUnit}, {obj.AffectedUnit}, {obj.Affect}, {targetUnits.Count()}");
		Log.Info($"{CombatMachine.Instance.IsValid()} Controller Valid");
		CombatMachine.Instance.Add(obj, true);
		CombatMachine.Instance.ProcessFinished += OnFinished;		
	}

	public void OnFinished()
	{
		Log.Info("Ability Command Finished");
		IsFinished = true;
		ThisUnit.Turn.SetCommand("ATTACK", false);
		ThisUnit.Turn.SetCommand("ABILITY", false);
		CombatMachine.Instance.ProcessFinished -= OnFinished;
	}
	public override void Tick(){}

	public AbilityCommand(IAbilityItem AItem, Unit unit, Unit Target, List<TileData> TarTiles)
	{
		Log.Info($"{TarTiles.Count()} in Ability Command Initialization");
		AbilityItem = AItem;
		ThisUnit = unit;
		TargetUnit = Target;
		TargetTiles = TarTiles;
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
