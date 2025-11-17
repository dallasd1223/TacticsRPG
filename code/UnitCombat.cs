using Sandbox;

namespace TacticsRPG;

[Category("Unit")]
public sealed class UnitCombat : Component
{
	[Property] BattleUnit ThisUnit {get; set;}
	
	public event Action AttackStart;
	public event Action AttackEnd;
	public event Action<int> DamageTaken;
	public event Action UnitHasDied;

	public bool InCombat = false;

	public bool HasDied {get; set;} = false;

	[Property] public SoundEvent DamageSound {get; set;}
	[Property] public SoundEvent DeathSound {get; set;}

	protected override void OnAwake()
	{
		ThisUnit = GetComponent<BattleUnit>();
		AttackStart += OnAttackStart;
		AttackEnd += OnAttackEnd;
		DamageTaken += OnDamageTaken;
	}

	public void StartAttack()
	{
		AttackStart?.Invoke();
	}

	public void OnAttackStart()
	{
		Log.Info("Attack Started");
	}

	public void EndAttack()
	{
		AttackEnd?.Invoke();
	}

	public void OnAttackEnd()
	{
		ThisUnit.Animator.AssignAnimation();
		Log.Info("Attack Ended");
	}

	public void TakeDamage(int damage)
	{
		ThisUnit.Stats.SetStat(StatType.HP, -damage);
		DamageTaken?.Invoke(damage);
	}

	public void OnDamageTaken(int d)
	{
		Log.Info($"Unit {ThisUnit.CoreData.Name} Took {d} Damage");
	}

	public bool CheckIfDead()
	{
		if(ThisUnit.Stats.GetStat(StatType.HP) <= 0 )
		{
			HasDied = true;
			Sound.Play(DeathSound);
			ThisUnit.Animator.PlayAnimation("downed");
			this.GameObject.WorldRotation = new Angles(0,0,0).ToRotation();
			UnitHasDied?.Invoke();
			Log.Info($"Unit {ThisUnit.CoreData.Name} Has Died");

			return true;
		}
		else
		{
			return false;
		}
	}
}
