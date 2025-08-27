using Sandbox;
using TacticsRPG;

public class CameraManager: SingletonComponent<CameraManager>
{

	
	//Camera Component
	[Property] public CameraComponent Camera {get; set;}

	//PostProcessing Components
	[Property] public Bloom PostBloom {get; set;}
	[Property] public Tonemapping Tone {get; set;}

	//Manager Properties 
	[Property] public CameraState CurrentState {get; set;} = CameraState.PlayerControlled;
	[Property] public CameraDirection Direction {get; set;}
	[Property] public GameObject FocusPoint {get; set;}
	[Property] public List<GameObject> FocusList {get; set;} = new List<GameObject>();
	[Property] public int SelectedFocusIndex {get; set;} = 0;
	[Property] public List<GameObject> NodeList {get; set;}
	[Property] public Vector3 NodePosition {get; set;}
	[Property] public int SelectedNode {get; set;} = 0;
	[Property] public SoundEvent NodeSwitch {get; set;}
	[Property] public bool EffectOverride {get; set;} = false;
	[Property] public float ZoomValue {get; set;}
	[Property] public bool IsFreeLook {get; set;} = false;
	private Vector3 LastPosition {get; set;}

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayerEvents.FocusModeChange += HandleFocusMode;
	}

	protected override void OnStart()
	{
		var nodelist = Scene.GetAll<CameraFocusNode>();
		foreach(CameraFocusNode node in nodelist)
		{
			FocusList.Add(node.GameObject);
		}

		LastPosition = this.GameObject.LocalPosition;
	}

	public void HandleFocusMode(FocusMode? f, Unit u)
	{
		switch(f)
		{
			case FocusMode.FreeLook:
				IsFreeLook = true;
				return;
			default:
				IsFreeLook = false;
				return;
		}
	}

	protected override void OnUpdate()
	{

		if(!EffectOverride)
		{
			this.GameObject.LocalPosition = LocalPosition.LerpTo(NodeList[SelectedNode].WorldPosition + NodeList[SelectedNode].WorldRotation.Backward * ZoomValue, 0.05f);
			this.GameObject.LocalRotation = Rotation.Lerp(this.GameObject.LocalRotation, Rotation.LookAt(FocusPoint.WorldPosition-WorldPosition), Time.Delta* 10f);
		}


		if(IsFreeLook)
		{
			HandleCamInput();

		}

	}
	public void CursorObjectFocus(GameObject target)
	{
		FocusPoint = target;
	}
	public void HandleCamInput()
	{
		if(Input.Pressed("Use"))
		{
			if(SelectedNode == 0)
			{
				SelectedNode = 3;
				PlaySound();
				Log.Info($"SelectedNode {SelectedNode}");
				return;
			}
			SelectedNode--;
			Log.Info($"SelectedNode {SelectedNode}");
			PlaySound();
		}
		if(Input.Pressed("Menu"))
		{
			if(SelectedNode == NodeList.Count() - 1)
			{
				SelectedNode = 0;
				Log.Info($"SelectedNode {SelectedNode}");
				PlaySound();
				return;
			}
			SelectedNode++;
			Log.Info($"SelectedNode {SelectedNode}");
			PlaySound();
		}
	}
	/*
	public void HandleFocusSelect()
	{
		if(Input.Pressed("Forward"))
		{
			if(SelectedFocusIndex == FocusList.Count() -1)
			{
				SelectedFocusIndex = 0;
				FocusPoint = FocusList[SelectedFocusIndex];
				PlaySound();
				return;
			}
			SelectedFocusIndex++;
			FocusPoint = FocusList[SelectedFocusIndex];
			Log.Info($"SelectedFocusIndex {FocusList[SelectedFocusIndex].Name}");
			PlaySound();
		}
		if(Input.Pressed("Backward"))
		{
			if(SelectedFocusIndex == 0)
			{
				SelectedFocusIndex = FocusList.Count() - 1;
				FocusPoint = FocusList[SelectedFocusIndex];
				PlaySound();
				return;
			}
			SelectedFocusIndex--;
			FocusPoint = FocusList[SelectedFocusIndex];
			Log.Info($"SelectedFocusIndex {FocusList[SelectedFocusIndex].Name}");
			PlaySound();
		}
	}
	*/
	public bool HasFocusNode(GameObject obj)
	{
		CameraFocusNode node = obj.GetComponent<CameraFocusNode>();
		if(node.IsValid())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public void SetFocus(GameObject obj)
	{
		SelectedFocusIndex = FindInFocusList(obj);
		FocusPoint = FocusList[SelectedFocusIndex];
	}

	public int FindInFocusList(GameObject obj)
	{
		return FocusList.FindIndex(0, node => node == obj);
	}

	public void DirectCameraFocus(GameObject obj)
	{
		if(HasFocusNode(obj))
		{
			SetFocus(obj);
			return;
		}
		else
		{
			Log.Info("GameObject Must Have CameraFocusNode");
			return;
		}
	}

	public void DirectCameraFocus(Unit unit)
	{
		if(HasFocusNode(unit.GameObject))
		{
			SetFocus(unit.GameObject);
			return;
		}
		else
		{
			Log.Info("GameObject Must Have CameraFocusNode");
			return;
		}
	}

	public void DirectCameraFocus(TileData tile)
	{
		if(HasFocusNode(tile.GameObject))
		{
			SetFocus(tile.GameObject);
			return;
		}
		else
		{
			Log.Info("GameObject Must Have CameraFocusNode");
			return;
		}
	}

	public void ResetToActive()
	{
		GameObject obj = FocusList.Find(u => u == BattleMachine.Instance.Turn.ActiveUnit.GameObject);
		SelectedFocusIndex = FocusList.FindIndex(u => u == obj);
		FocusPoint = FocusList[SelectedFocusIndex];

	}

	public void PlaySound()
	{
		Sound.Play(NodeSwitch);
	}
}

public enum CameraState
{
	PlayerControlled,
	BattleControlled,
	Cinematic,
}

public enum CameraDirection
{
		North,
		South,
}
