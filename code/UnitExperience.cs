using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitExperience : Component
{
	[Property] public UnitStats Stats {get; set;}

	[Property] public bool HasLeveledUp {get; set;}

	public event Action<int> OnExpEarned;
	public event Action<int> OnLevelUp;

	public void AddXP(int amount)
	{
		int CurXP = Stats.GetStat(StatType.EXP);
		int temp = CurXP + amount;
		if(temp >= 100)
		{
			LevelUp();
			Stats.SetStat(StatType.EXP, temp - 100);

		}
		else
		{
			Stats.SetStat(StatType.EXP, amount);		
		}

		OnExpEarned?.Invoke(Stats.GetStat(StatType.EXP));	
	}

	public void LevelUp()
	{
		Stats.SetStat(StatType.LVL, 1);
		HasLeveledUp = true;
		OnLevelUp?.Invoke(Stats.GetStat(StatType.LVL));
	}

	public int UntilLevelUp()
	{
		int CurXP = Stats.GetStat(StatType.EXP);
		int amount = 100 - CurXP;
		Log.Info($"{amount} EXP Until Next LevelUp");
		return amount;
	}


	protected override void OnAwake()
	{
		Stats = GetComponent<UnitStats>();
	}

	protected override void OnStart()
	{
		AddXP(150);
		UntilLevelUp();
	}

}
