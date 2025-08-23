using Sandbox;
using System;

namespace TacticsRPG;

public sealed class TurnQueueController : Component
{

	private Queue<Unit> TurnQueue;

	public bool BuildQueue()
	{
		foreach(Unit u in UnitManager.Instance.UnitList)
		{
			TurnQueue.Enqueue(u);
	
		}
		Log.Info("Units Processed");
		Log.Info($"{TurnQueue.Count()} Units in Turn Queue");
		return true;
	}

	public void AddToQueue(Unit u)
	{
		TurnQueue.Enqueue(u);
	}
	public Unit NextInQueue()
	{
		return TurnQueue.Dequeue();
	}
}
