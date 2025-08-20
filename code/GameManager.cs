using Sandbox;

public sealed class GameManager : Component
{
	public static GameManager Instance;
	[Property] public GameState CurrentState {get; set;} = GameState.NA;
	[Property] public CameraManager CamManager {get; set;}
	[Property] public MapData Map {get; set;}
	[Property] public SoundEvent ModeSound {get; set;}
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


