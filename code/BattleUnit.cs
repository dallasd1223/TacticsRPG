using Sandbox;

namespace TacticsRPG;
[Category("Unit")]
public sealed class BattleUnit : Component
{
	[Property] public BattleUnitData BattleData;
	[Property] public int BattleID {get; set;}
	[Property] public int StartTileIndex {get; set;}
	[Property] Vector2 CurrentTileXY {get; set;} = new Vector2(0,0);

	[Property] public bool isAIControlled = false;

	[Property] int MoveRange {get; set;} = 6;
	[Property] int AttackRange {get; set;} = 1;
	[Property] int JumpRange {get; set;} = 3;

	[Property] public Direction FaceDirection = Direction.North;
	
	[Property] public UnitCommand Command {get; set;}
	[Property] public TileInteract Interact {get; set;}
	[Property] public UnitTurn Turn {get; set;}
	[Property] public UnitEquipment Equipment {get; set;}
	[Property] public UnitSkillset Skillset {get; set;}
	[Property] public UnitMove Move {get; set;}
	[Property] public UnitAttack Attack {get; set;}
	[Property] public UnitAbility Ability {get; set;}
	[Property] public UnitCoreData CoreData {get; set;}
	[Property] public UnitStats Stats {get; set;}
	[Property] public UnitExperience Experience {get; set;}
	[Property] public UnitJob Job {get; set;}
	[Property] public UnitAbilities UAbility {get; set;}
	[Property] public UnitAnimator Animator {get; set;}
	[Property] public UnitCombat Combat {get; set;}
	[Property] public UnitAI AI {get; set;}
	[Property] public FloatingElementManager FEM {get; set;}
	[Property] public TeamType Team {get; set;}
	[Property] public bool IsTurn {get; set;} = false;
	[Property] Vector3 UnitPosition {get; set;}

	protected override void OnAwake()
	{
		AI = GetComponent<UnitAI>();
		CoreData = GetComponent<UnitCoreData>();
		Stats = GetComponent<UnitStats>();
		Experience = GetComponent<UnitExperience>();
		Job = GetComponent<UnitJob>(); 
		Turn = GetComponent<UnitTurn>();
		Equipment = GetComponent<UnitEquipment>();
		Skillset = GetComponent<UnitSkillset>();
		Interact = GetComponent<TileInteract>();
		Combat = GetComponent<UnitCombat>();
		Move = GetComponent<UnitMove>();
		Attack = GetComponent<UnitAttack>();
		Ability = GetComponent<UnitAbility>();
		Animator = GetComponent<UnitAnimator>();
		Command = GetComponent<UnitCommand>();
		UAbility = GetComponent<UnitAbilities>();
		FEM = GetComponentInChildren<FloatingElementManager>();
		Experience.OnExpEarned += ExpEarned;
		Experience.OnLevelUp += LeveledUp;
		UAbility.OnAbilityAdded += AbilityAdded;
		Equipment.OnEquip += OnEquipped;
		Job.OnJobExpChange += JobEXPChanged;
	}

	protected override void OnStart()
	{
		TileData tile = TileMapManager.Instance.FindTileFromIndex(StartTileIndex);
		GameObject.WorldPosition = tile.GameObject.WorldPosition + new Vector3(0,0,3.5f);
		CurrentTileXY = new Vector2(tile.XIndex, tile.YIndex);
		UnitPosition = WorldPosition;
	}

	public void ExpEarned(int amt)
	{
		Log.Info($"{CoreData.Name} Has Earned {amt} EXP. {Experience.UntilLevelUp()} EXP Until Level Up");
	}
	public void LeveledUp(int i)
	{
		Log.Info($"{CoreData.Name} Has Leveled Up To LVL {i}");
	}
	public void JobEXPChanged(JobData job, int amount, JobExp exp)
	{
		Log.Info($"{CoreData.Name}'s Job {job.Name} CurrentJP has increased by {amount} JP, and now contains {exp.CurrentJP} CurrentJP");
		Log.Info($"{exp.UntilLevel()} JP Until Level Up");
	}
	public void AbilityAdded(Ability a)
	{
		Log.Info($"{CoreData.Name} Learned Ability {a.Data.Name}");
	}


	public void OnEquipped(EquipmentSlotType slot, Equipment equip)
	{
		Log.Info($"{CoreData.Name} Has Equipped {equip.Data.Name} in {equip.Data.Slot} Slot");
	}
}

public enum Direction
{
	North,
	South,
	East,
	West,
}
