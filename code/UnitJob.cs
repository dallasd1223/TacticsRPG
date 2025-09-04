using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitJob : Component
{

	[Property] public JobData CurrentJob {get; set;}

	[Property] [ReadOnly] public Dictionary<string, JobExp> JobExpDictionary = new();

	public event Action<JobData, int, JobExp> OnJobExpChange;

	public void AddJP(int amount)
	{
		JobExpDictionary[CurrentJob.Name].CurrentJP += amount;
		JobExpDictionary[CurrentJob.Name].TotalJP += amount;

		OnJobExpChange?.Invoke(CurrentJob, amount, JobExpDictionary[CurrentJob.Name]);
	}

	public void LogJobExp(JobData job)
	{
		var exp = JobExpDictionary[job.Name];
		Log.Info($"Job: {job.Name} Job Level: {exp.JobLevel} CurrentJP : {exp.CurrentJP} TotalJP: {exp.TotalJP}");
	}

	protected override void OnStart()
	{
		AddJP(105);
		LogJobExp(CurrentJob);
	}
}
