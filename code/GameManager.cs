using Sandbox;
using System;

namespace TacticsRPG;

public sealed class GameManager : Component
{
	public static GameManager Instance;
	[Property] public GameState CurrentState {get; set;} = GameState.NA;
	[Property] public SaveManager Save {get; set;}
	[Property] public DebugUI DebugVisual {get; set;}
	 
	private float RealStartTime {get; set;}
	[Property] public float GameTime {get; set;} = 0;
	
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
		RealStartTime = RealTime.Now;
		CurrentState = GameState.Playing;
		JobDatabase.Initialize();
	}

	protected override void OnUpdate()
	{
		GameTime = RealTime.Now - RealStartTime;
	}

	public int GetTimeAsINT()
	{
		return (int)GameTime;
	}

	public void SetDebug()
	{
		if(DebugVisual.IsActive)
		{
			DebugVisual.Deactivate();
		}
		else if(!DebugVisual.IsActive)
		{
			DebugVisual.Activate();
		}
	}

}

public enum GameState
{
	Playing,
	NA,
}


