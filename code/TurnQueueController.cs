using Sandbox;
using System;

namespace TacticsRPG;

public sealed class TurnQueueController : Component
{

	private Queue<BattleUnit> TurnQueue = new();

	public bool BuildQueue()
	{
		foreach(BattleUnit u in UnitManager.Instance.UnitList)
		{
			TurnQueue.Enqueue(u);
	
		}
		Log.Info("Units Processed");
		Log.Info($"{TurnQueue.Count()} Units in Turn Queue");
		return true;
	}

	public void AddToQueue(BattleUnit u)
	{
		TurnQueue.Enqueue(u);
	}
	public BattleUnit NextInQueue()
	{
		return TurnQueue.Dequeue();
	}
}
