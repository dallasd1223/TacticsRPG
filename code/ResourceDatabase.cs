using Sandbox;
using System;

namespace TacticsRPG;

public static class JobDatabase 
{
	private static Dictionary<string, JobData> JobsByID = new();

	public static void Initialize()
	{
		var alljobs = ResourceLibrary.GetAll<JobData>();
		foreach(JobData job in alljobs)
		{
			if(job == null) return;
			Log.Info($"{job.ID}");
			JobsByID[job.ID] = job;
			Log.Info($"{job.Name} ID: {job.ID} Added To JobDatabase");
		}
	}

	public static void ClearDatabase()
	{
		JobsByID.Clear();
	}

	public static JobData Get(string jobID)
	{
		return JobsByID.TryGetValue(jobID, out var job) ? job : null;
	}

	public static List<JobData> GetAll() => JobsByID.Values.ToList();
}

public static class SkillsetDatabase 
{
	private static Dictionary<string, SkillsetData> SkillsetsByID = new();

	public static void Initialize()
	{
		var allskillsets = ResourceLibrary.GetAll<SkillsetData>();
		foreach(SkillsetData skillset in allskillsets)
		{
			if(skillset == null) return;
			Log.Info($"{skillset.Name}");
			SkillsetsByID[skillset.ID] = skillset;
			Log.Info($"{skillset.Name} ID: {skillset.ID} Added To SkillsetDatabase");
			
		}
	}

	public static void ClearDatabase()
	{
		SkillsetsByID.Clear();
	}

	public static SkillsetData Get(string skillsetID)
	{
		return SkillsetsByID.TryGetValue(skillsetID, out var skillset) ? skillset : null;
	}

	public static List<SkillsetData> GetAll() => SkillsetsByID.Values.ToList();
}

public static class AbilityDatabase 
{
	private static Dictionary<string, AbilityData> AbilitiesByID = new();

	public static void Initialize()
	{
		var allabilities = ResourceLibrary.GetAll<AbilityData>();
		foreach(AbilityData ability in allabilities)
		{
			if(ability == null) return;
			if(ability.ID == null || ability.Name == null) return;
			Log.Info($"{ability.Name}");
			AbilitiesByID[ability.ID] = ability;
			Log.Info($"{ability.Name} ID: {ability.ID} Added To AbilityDatabase");
		}
	}
	
	public static void ClearDatabase()
	{
		AbilitiesByID.Clear();
	}
	public static AbilityData Get(string abilityID)
	{
		return AbilitiesByID.TryGetValue(abilityID, out var ability) ? ability : null;
	}

	public static List<AbilityData> GetAll() => AbilitiesByID.Values.ToList();
}
