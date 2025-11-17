using Sandbox;
using System;

namespace TacticsRPG;

// Units Have Stats, Equipment, SkillSets, Abilities, Jobs
public class UnitProfile
{
	public string Name {get; set;}
	public Dictionary<StatType, int > RawStats {get; set;} = new();
	public Dictionary<EquipmentSlotType, Equipment> ActiveEquipment {get; set;} = new();
	public JobData CurrentJob {get; set;}
	public Dictionary<string, UnitJobData> UnitJobDictionary {get; set;} = new();
	public Dictionary<string, UnitAbilityData> UnitAbilityDictionary {get; set;} = new();
}

public struct UnitSaveData
{
	public string Name;
	public Dictionary<StatType, int> Stats;
	public string CurrentJobID;
	public Dictionary<string, JobSaveData> JobRecords;
}
