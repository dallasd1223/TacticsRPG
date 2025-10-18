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

	//Returns True If Unit LVLed Up
	public bool AddXP(int amount)
	{
		int CurXP = Stats.GetStat(StatType.EXP);
		int temp = CurXP + amount;
		Log.Info($"temp exp: {temp}");
		if(temp >= 100)
		{
			Stats.SetStat(StatType.EXP, temp - 100, true);
			Log.Info($"temp - 100: {temp - 100}");
			LevelUp();
			OnExpEarned?.Invoke(amount);
			return true;
		}
		else
		{
			Stats.SetStat(StatType.EXP, amount);
			OnExpEarned?.Invoke(amount);
			return false;	
		}

	
	}

	public void LevelUp()
	{
		Stats.SetStat(StatType.LVL, 1);
		HasLeveledUp = true;
		OnLevelUp?.Invoke(Stats.GetStat(StatType.LVL));
	}

	public void ConfirmLevelUp()
	{
		HasLeveledUp = false;
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
		AddXP(50);
		Log.Info("EXP ADDED");
	}

}
