using Sandbox;
using System;

namespace TacticsRPG;

public class UnitJobData
{
	string JobID {get; set;}
	bool IsUnlocked {get; set;} = false;
	JobExp JobExperience {get; set;} = new JobExp();
}

public class JobExp
{
	public int JobLevel {get; set;} = 1;
	private int _currentjp = 0;
	public int CurrentJP
	{
	
	get
		{
			return _currentjp;
		}
	
	set
		{
			if(HasLeveledUp(value))
			{
				JobLevel++;
				_currentjp = _currentjp + value - 100;
				Log.Info($"Job Has Leveled Up: LVL {JobLevel}");
			}
			else
			{
				_currentjp = _currentjp + value;
			}
		}
	}

	public int TotalJP {get; set;} = 0;

	public int UntilLevel()
	{
		return 100 - CurrentJP;
	}

	public bool HasLeveledUp(int value)
	{
		if(CurrentJP + value >= 100)
		{
			return true;
		}
		else return false;
	}
}

public class JobSaveData
{
	public int ID;
	public bool IsUnlocked;
	public int JobLevel;
	public int CurrentJP;
	public int TotalJP;
}


[GameResource("Job", "job", "Defines Data of A Job")]
public class JobData : GameResource
{
	public string ID {get; set;}
	public string Name {get; set;}
	public string Description {get; set;}

	public JobEnum Type {get; set;}
	public JobStatData Stats {get; set;}
	public string SkillsetID {get; set;}

	public Dictionary<string, int> UnlockRequirements {get; set;} = new();

	public List<AbilityData> JobAbilities {get; set;} = new();
	public List<WeaponType> WeaponRestrictions {get; set;} = new();
	public List<ArmorType> ArmorRestrictions {get; set;} = new();
	public List<HelmetType> HelmetRestrictions {get; set;} = new();
	public List<AccessoryType> AccessoryRestrictions {get; set;} = new();



}

[GameResource("JobStat", "jstat", "Defines Data For Job Stats")]
public class JobStatData : GameResource
{
	public string ID {get; set;}
	public JobEnum Type {get; set;}

	public Dictionary<StatType, int> StatMultiplier {get; set;} = new Dictionary<StatType, int>
	{
		{StatType.MAXHP, 0},
		{StatType.MAXMP, 0},
		{StatType.MOV, 0},
	};

	public Dictionary<StatType, int> StatGrowthConstant {get; set;} = new();
}

public enum JobEnum
{
	Squire,
	Knight,
	Archer,
	Wizard,
	Thief,
	Priest,
}
