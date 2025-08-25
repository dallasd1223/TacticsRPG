using Sandbox;

namespace TacticsRPG;
[Category("Unit")]
public sealed class Unit : Component
{
	[Property] public int StartTileIndex {get; set;}
	[Property] Vector2 CurrentTileXY {get; set;} = new Vector2(0,0);
	[Property] public bool isAIControlled = false;
	[Property] int MoveRange {get; set;} = 6;
	[Property] int AttackRange {get; set;} = 1;
	[Property] int JumpRange {get; set;} = 3;
	[Property] public FaceDirectionType FaceDirection = FaceDirectionType.North;
	[Property] public UnitTurn Turn {get; set;}
	[Property] public TileInteract Interact {get; set;}
	[Property] public UnitMove Move {get; set;}
	[Property] public UnitAttack Attack {get; set;}
	[Property] public UnitAbility Ability {get; set;}
	[Property] public UnitData Data {get; set;}
	[Property] public UnitStats Stats {get; set;}
	[Property] public UnitAbilities UAbility {get; set;}
	[Property] public UnitSpells USpell {get; set;}
	[Property] public UnitSkills USkill {get; set;}
	[Property] public UnitAnimator Animator {get; set;}
	[Property] public UnitBattle Battle {get; set;}
	[Property] public UnitAI AI {get; set;}
	[Property] public TeamType Team {get; set;}
	[Property] public bool IsTurn {get; set;} = false;
	[Property] Vector3 UnitPosition {get; set;}

	protected override void OnAwake()
	{
		Stats = GetComponent<UnitStats>();
		Interact = GetComponent<TileInteract>();
		Battle = GetComponent<UnitBattle>();
		Move = GetComponent<UnitMove>();
		Ability = GetComponent<UnitAbility>();
		Animator = GetComponent<UnitAnimator>();
		Turn = GetComponent<UnitTurn>();
		UAbility = GetComponent<UnitAbilities>();
		USpell = GetComponent<UnitSpells>();
		USkill = GetComponent<UnitSkills>();
		UAbility.OnAbilityAdded += AbilityAdded;
		USkill.OnSkillAdded += SkillAdded;
		USpell.OnSpellAdded += SpellAdded;
	}

	protected override void OnStart()
	{
		TileData tile = TileMapManager.Instance.FindTileFromIndex(StartTileIndex);
		GameObject.WorldPosition = tile.GameObject.WorldPosition + new Vector3(0,0,3.5f);
		CurrentTileXY = new Vector2(tile.XIndex, tile.YIndex);
		UnitPosition = WorldPosition;
	}

	public void AbilityAdded(Ability a)
	{
		Log.Info($"{Data.Name} Learned Ability {a.Data.Name}");
	}

	public void SkillAdded(Skill s)
	{
		Log.Info($"{Data.Name} Learned Skill {s.Data.Name}");
	}

	public void SpellAdded(Spell s)
	{
		Log.Info($"{Data.Name} Learned Spell {s.Data.Name}");
	}
}

public enum FaceDirectionType
{
	North,
	South,
	East,
	West,
}
