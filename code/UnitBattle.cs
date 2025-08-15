using Sandbox;

namespace TacticsRPG;

public sealed class UnitBattle : Component
{
	[Property] Unit ThisUnit {get; set;}
	
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
		ThisUnit = GetComponent<Unit>();
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
		ThisUnit.Data.CurrentHP -= damage;
		DamageTaken?.Invoke(damage);
	}

	public void OnDamageTaken(int d)
	{
		Log.Info($"Unit {ThisUnit.Data.Name} Took {d} Damage");
		if(CheckIfDead())
		{
			UnitHasDied?.Invoke();
		}
	}

	public bool CheckIfDead()
	{
		if(ThisUnit.Data.CurrentHP <= 0 )
		{
			HasDied = true;
			Sound.Play(DeathSound);
			ThisUnit.Animator.PlayAnimation("downed");
			this.GameObject.WorldRotation = new Angles(0,0,0).ToRotation();
			Log.Info($"Unit {ThisUnit.Data.Name} Has Died");
			return true;
		}
		else
		{
			return false;
		}
	}
}
