using Sandbox;
using System;

namespace TacticsRPG;

public class CombatMachine : StateMachine
{
	public Queue<CombatObject> CombatObjectList {get; set;} = new Queue<CombatObject>();
	public List<EffectSequence> ActiveSequences {get; set;} = new List<EffectSequence>();

	public CombatObject CurrentObject;
	public bool IsProcessing = false;
	

	public event Action ProcessFinished;
	public static CombatMachine Instance {get; set;}

	protected override void OnAwake()
	{
		if(Instance is null)
		{
			Instance = this;
		}
		else
		{
			Instance = null;
			Instance = this;
		}
	}

	protected override void OnStart()
	{
		Log.Info("Combat Manager Has Started");
	}

	public void StartMachine()
	{
		ChangeState<InitialCombatState>();
	}

	public void Add(CombatObject obj, bool b)
	{
		Log.Info($"New Combat Object Added {obj}");
		CombatObjectList.Enqueue(obj);
		Log.Info($"Queue Count: {CombatObjectList.Count}");
		if(b)
		{
			StartMachine();
		}
	}

	public void StartQueuedObject()
	{
		Log.Info("Starting Queued Combat Object");
		CurrentObject = CombatObjectList.Dequeue();
		IsProcessing = true;
		ProcessObject();

	}

	//REFACTOR THIS METHOD, ITS DOING WAY TO MUCH. DISTRIBUTE RESPONSIBILITY (I THINK EFFECT SYSTEM SHOULD TAKE OVER `MASTER EFFECT SYSTEM`)
	//MAKE MORE DYNAMIC (ALL NECESSARY INFORMATION IN COMBAT OBJECT)
	public async void ProcessObject()
	{
		if(CurrentObject is not null && IsProcessing)
		{
			switch(CurrentObject.Affect)
			{
				case AffectType.Self:
					//InCombat Turns On Unit HealthBars
					CurrentObject.ActingUnit.Battle.InCombat = true;
					Log.Info("Inside CombatController Self");
					await Task.DelayRealtimeSeconds(0.5f);
					if(CurrentObject.AbilityItem is Item)
					{
						var item = CurrentObject.AbilityItem;
						EffectEvent COeffect = new PotionEffect(CurrentObject, item.Data.effectData);
						EffectManager.Instance.PlayEffect(COeffect);
						await Task.DelayRealtimeSeconds(item.Data.effectData.TotalDuration);
					}
					CurrentObject.ActingUnit.Turn.HasActed = true;
					CurrentObject.ActingUnit.Battle.InCombat = false;
					ProcessFinished?.Invoke();
					CurrentObjectFinished();
					break;
				case AffectType.Single:
					CurrentObject.ActingUnit.Battle.InCombat = true;
					CurrentObject.AffectedUnit.Battle.InCombat = true;
					await Task.DelayRealtimeSeconds(0.5f);
					CurrentObject.ActingUnit.Battle.StartAttack();
					CurrentObject.ActingUnit.Animator.PlayAnimation("attack", (string n) => Sound.Play(CurrentObject.AffectedUnit.Battle.DamageSound));
					CurrentObject.AffectedUnit.Animator.PlayAnimation("hit");
					CurrentObject.AffectedUnit.Animator.jitter = true;
					Log.Info($"{CurrentObject.ActingUnit.Data.Name} Attacks {CurrentObject.AffectedUnit.Data.Name}");
					var result = CombatResolver.ResolveAttack(CurrentObject.ActingUnit, CurrentObject.AffectedUnit);
					Log.Info($"{result.DamageAmount} Damage Of Type {result.Type}");
					UnitEvents.UnitAttacked(CurrentObject.ActingUnit, CurrentObject.AffectedUnit);
					CurrentObject.AffectedUnit.Battle.TakeDamage(result.DamageAmount);
					CurrentObject.AffectedUnit.FEM.CreateIntValueText(result.DamageAmount, new Color(1.00f, 1.00f, 1.00f, 1.00f));
					SpriteEffect.Instance.DamageNum.Clone(CurrentObject.AffectedUnit.GameObject.WorldPosition + new Vector3(0,0,10));
					await Task.DelayRealtimeSeconds(1.5f);
					bool dead = CurrentObject.AffectedUnit.Battle.CheckIfDead();
					if(dead) await Task.DelayRealtimeSeconds(1.5f);
					CurrentObject.ActingUnit.Turn.HasActed = true;

					CurrentObject.ActingUnit.Battle.EndAttack();
					CurrentObject.AffectedUnit.Animator.AssignAnimation();
					CurrentObject.AffectedUnit.Animator.EndJitter();
					CurrentObject.ActingUnit.Battle.InCombat = false;
					CurrentObject.AffectedUnit.Battle.InCombat = false;
	
					ChangeState<ReactionCombatState>();
					break;
				case AffectType.Multi:
					break;
			}
		}
	}

	protected override void OnUpdate()
	{
		if(ActiveState.IsValid())
		{
			ActiveState.Update();
		}
	}

	public void CurrentObjectFinished()
	{
		IsProcessing = false;
		CurrentObject = null;
		ProcessFinished?.Invoke();
	}
	public void ClearObjectList()
	{
		CombatObjectList.Clear();
	}

}

public class CombatObject
{
	public Unit ActingUnit;
	public Unit AffectedUnit;
	public AffectType Affect = AffectType.Single;
	public List<Unit> AffectedUnits;
	public ActionType Type = ActionType.BasicAttack;
	public bool BasicAttack = false;
	public IAbilityItem AbilityItem;

	public CombatObject(Unit au1, Unit au2, AffectType at, bool b, List<Unit> ul = null, IAbilityItem ai = null)
	{
		ActingUnit = au1;
		AffectedUnit = au2;
		Affect = at;
		AffectedUnits = ul;
		BasicAttack = b;
		AbilityItem = ai;
	}
}


public enum ActionType
{
	BasicAttack,
	SpecialAttack,
	Ability,
}

public enum AffectType
{
	Self,
	Single,
	Multi,
}


public class CombatState : State
{
	protected CombatMachine Machine {get; set;}

	protected override void OnAwake()
	{
		Machine = GetComponent<CombatMachine>();
	}


}

public class InitialCombatState : CombatState
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("InitialCombatState Entered");
		Machine.StartQueuedObject();
	}

	public override void Update()
	{

	}

	public override void Exit()
	{
		base.Exit();
	}	
}
public class ReactionCombatState : CombatState
{
	public override void Enter()
	{
		base.Enter();
		Log.Info("ReactionCombatState Entered");
	}

	public override void Update()
	{
		Machine.ChangeState<ExperienceCombatState>();
	}

	public override void Exit()
	{
		base.Exit();
	}
}

public class ExperienceCombatState : CombatState
{
	public override void Enter()
	{
		Machine.CurrentObject.ActingUnit.FEM.ClearedActiveOnceElements += FEMCallback;
		int xpgain = 60;
		int jpgain = 21;
		var b = Machine.CurrentObject.ActingUnit.Experience.AddXP(xpgain);
		Machine.CurrentObject.ActingUnit.Job.AddJP(jpgain);
		if(b)
		{
			Machine.CurrentObject.ActingUnit.FEM.CreateLVLUPText();
		}
		Machine.CurrentObject.ActingUnit.FEM.CreateEXPText(xpgain);
		Machine.CurrentObject.ActingUnit.FEM.CreateJPText(jpgain);
	}

	void FEMCallback()
	{
		Machine.NullState();
	}

	public override void Update()
	{
		
	}

	public override void Exit()
	{
		Machine.CurrentObject.ActingUnit.FEM.ClearedActiveOnceElements -= FEMCallback;
		Machine.CurrentObjectFinished();
		base.Exit();
	}
}
