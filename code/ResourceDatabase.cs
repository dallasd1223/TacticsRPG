using Sandbox;
using System;

namespace TacticsRPG;

public static class JobDatabase
{
	private static Dictionary<string, JobData> JobsByName = new();

	public static void Initialize()
	{
		var alljobs = ResourceLibrary.GetAll<JobData>();
		foreach(JobData job in alljobs)
		{
			if(job == null) return;
			Log.Info($"{job.Name}");
			JobsByName[job.Name] = job;
			Log.Info($"{job.Name} Added To JobDatabase");
		}
	}

	public static JobData Get(string jobName)
	{
		return JobsByName.TryGetValue(jobName, out var job) ? job : null;
	}

	public static List<JobData> GetAll() => JobsByName.Values.ToList();
}
