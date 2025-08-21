using Sandbox;
using System;
using SpriteTools;

namespace TacticsRPG;
public class EffectSequence
{
	public int SequenceID = 0;
	private List<IEffectStep> steps = new();
	private float elapsed = 0f;
	public bool IsPlaying = false;
	public bool IsFinished = false;
	public void SetID(int id) => SequenceID = id;
	public void AddStep(IEffectStep step) => steps.Add(step);
	public void RemoveStep(IEffectStep step) => steps.Remove(step);
	public void SetAllComplete(Action act) => AllComplete = act;

	public event Action AllComplete;

	public void Start(Action act = null)
	{
		Log.Info("Sequence Started");
		IsPlaying = true;
		elapsed = 0f;

		if (act is not null)
		{
			AllComplete += act;
		}
	}

	public void ForceEnd()
	{
		foreach(IEffectStep step in steps)
		{
			step.End();
		}
	}
	public void Update(float deltatime)
	{
		if(IsFinished) return;
		if(!IsPlaying) return;
		elapsed += deltatime;
		foreach(var step in steps)
		{
			if(elapsed >= step.StartTime && !step.Started)
			{
				Log.Info("Starting Step");
				step.Start();
			}
			if(elapsed >= step.StartTime && !step.IsComplete(elapsed))
			{
				step.Update(deltatime);
			}
			if(step.IsComplete(elapsed))
			{
				step.End();
			}
		}
		if(steps.All(p => p.Finished))
		{
			IsFinished = true;
			AllComplete?.Invoke();
			Log.Info("Sequence Finished");
		}
	}

}

public interface IEffectStep
{
	float StartTime {get;}
	float Duration {get; }
	bool Started {get;}
	bool Finished {get;}

	void Start();
	void Update(float localTime);
	void End();

	bool IsComplete(float globalTime);
}

public class ColorSpriteStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public SpriteComponent Sprite;
	public Color ToColor;

	public void Start()
	{
		Started = true;
		if(!Sprite.IsValid()) return;

		Sprite.ColorTo(ToColor, Duration, EasingType.Linear);
	}

	public void Update(float localTime) {}

	public void End()
	{
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;		
}

public class SpawnValueNumberStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public GameObject NumberPrefab;
	public Vector3 SpawnPosition;

	public void Start()
	{
		if(!NumberPrefab.IsValid()) return;
		Started = true;

		NumberPrefab.Clone(SpawnPosition);
	}

	public void Update(float localTime) {}

	public void End()
	{
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}

public class SpawnPointLightStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public Vector3 SpawnPosition;
	public Color ToColor;
	private GameObject light;

	public void Start()
	{
		Started = true;

		light = new GameObject( true, "light");
		light.WorldPosition = SpawnPosition;
		var comp = light.AddComponent<PointLight>();
		comp.Radius = 400f;
		comp.Attenuation = 1f;
		comp.LightColor = "#E9FAFF";
		comp.ColorTo(ToColor, Duration);

	}

	public void Update(float localTime) {}

	public void End()
	{
		light.Destroy();
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;	
}

public class ColorSkyBoxStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public Color ToColor;
	public SkyBox2D Skybox;

	public void Start()
	{
		if(!Skybox.IsValid()) return;
		Started = true;

		Skybox.ColorTo(ToColor, Duration, EasingType.Linear);

	}

	public void Update(float localTime) {}

	public void End()
	{
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}

public class ColorMapStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public Color ToColor;
	public ModelRenderer Map;

	public void Start()
	{
		if(!Map.IsValid()) return;
		Started = true;

		Map.ColorTo(ToColor, Duration, EasingType.Linear);

	}

	public void Update(float localTime) {}

	public void End()
	{
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}
public class PlaySoundStep : IEffectStep
{
	public SoundEvent sound;

	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public void Start()
	{
		Log.Info("Effect Step Started");
		Started = true;
		Sound.Play(sound);
	}

	public void Update(float localTime){Log.Info(localTime);}

	public void End(){

		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}
public class PlayAnimationStep : IEffectStep
{
	public Unit unit;
	public string AnimationName;

	public float StartTime {get; set;}
	public float Duration {get; set;}
	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;

	public void Start()
	{
		Log.Info("Effect Step Started");
		unit.Animator.PlayAnimation(AnimationName);
		Started = true;
	}

	public void Update(float localTime){Log.Info(localTime);}

	public void End(){
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}

public class CameraOrbitStep : IEffectStep
{
	public GameObject Target;
	public float Radius = 1250f;
	public float Height = 400f;
	public float ZoomDistance = 100f;
	public float Revolutions = 2f;
	public Curve eCurve {get; set;}
	public float StartTime {get; set;} = 0f;
	public float Duration {get; set;} = 10f;

	public bool Started {get; set;} = false;
	public bool Finished {get; set;} = false;
	public float elapsed = 0f;
	private Vector3 originalPosition;
	private Rotation originalRotation;
	private float _radius;

	private Vector3 center;
	private float startAngle;
	
	public void Start()
	{
		originalPosition = CameraManager.Instance.WorldPosition;
		originalRotation = CameraManager.Instance.WorldRotation;
		center = Target.WorldPosition + Vector3.Up;
		Vector3 toCamera = (originalPosition - center).WithZ(0f);
		_radius = toCamera.Length;
		startAngle = MathF.Atan2(toCamera.y, toCamera.x);
		CameraManager.Instance.EffectOverride = true;
		elapsed = 0;
		Started = true;
	}

	public void Update(float delta)
	{
		elapsed += Time.Delta;
		float t = MathX.Clamp(elapsed / Duration, 0, 1f);

		float angle = startAngle + (float)Math.PI * 2f * Revolutions;
		float curAngle = MathX.Lerp(startAngle, angle, t);
		float h = MathX.Lerp(0f, ZoomDistance, t);
		Vector3 offset = new Vector3((float)Math.Cos(curAngle) * _radius,(float)Math.Sin(curAngle) * _radius, originalPosition.z + h);
		Vector3 position = center + offset;
		CameraManager.Instance.WorldPosition = CameraManager.Instance.WorldPosition.LerpTo(position,1f);
		CameraManager.Instance.LocalRotation = Rotation.LookAt((center - position)+ new Vector3(0,0,h));
		_radius += h;
	}

	public void End()
	{
		CameraManager.Instance.EffectOverride = false;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;

}

public class CameraSpiralInStep : IEffectStep
{
	public GameObject Target;
	public Vector3 FinalOffset;
	public float HeightStart;
	
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;}
	public bool Finished {get; set;}

	public float elapsed = 0f;

	private Vector3 startPosition;
	private Rotation startRotation;
	private Vector3 finalPosition;
	private Rotation finalRotation;

	private Vector3 center;

	public void Start()
	{
		center = Target.WorldPosition;

		finalPosition = FinalOffset;

		startPosition = center + (Vector3.Up * HeightStart);

		CameraManager.Instance.EffectOverride = true;
		CameraManager.Instance.WorldPosition = startPosition;
		elapsed = 0f;
		Started = true;
	}

	public void Update(float delta)
	{
		elapsed += delta;
		float t = MathX.Clamp(elapsed/Duration, 0, 1f);
		float easedT = Ease.InOutSine(t);

		float radius = Vector3.DistanceBetween(startPosition, finalPosition) * easedT;
		float angle = (float)Math.Tau * easedT;
		Vector3 offset = new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0f) * radius;

		Vector3 spiralPos = center + offset + MathX.Lerp(HeightStart, FinalOffset.z, easedT);
		CameraManager.Instance.WorldPosition = spiralPos;
		CameraManager.Instance.LocalRotation = Rotation.LookAt(center - spiralPos);
	}

	public void End()
	{
		CameraManager.Instance.EffectOverride = false;
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}

public class SpawnPrefabStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;}
	public bool Finished {get; set;}

	public float elapsed = 0f;

	public Vector3 StartPosition {get; set;}
	public GameObject SpawnObject {get; set;}


	private GameObject SpawnedInstance;

	public void Start()
	{
		Log.Info("Before Object Spawn");
		if(!SpawnObject.IsValid()) return;
		Started = true;
		SpawnedInstance = SpawnObject.Clone(StartPosition);
		Log.Info($"Started Spawn PrefabStep {SpawnedInstance.IsValid()}");

	}

	public void Update(float localTime) {}

	public void End()
	{
		SpawnedInstance.Destroy();
		Finished = true;
	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}

public class SpawnParticlePrefabStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;}
	public bool Finished {get; set;}

	public float elapsed = 0f;

	public GameObject ParticlePrefab;
	public Vector3 SpawnPosition;

	public void Start()
	{
		if(!ParticlePrefab.IsValid) return;

		Started = true;

		ParticlePrefab.Clone(SpawnPosition);
	}

	public void Update(float localTime) {}

	public void End()
	{

	}

	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}

public class BetterCameraSpiralInStep : IEffectStep
{
	public float StartTime {get; set;}
	public float Duration {get; set;}

	public bool Started {get; set;}
	public bool Finished {get; set;}

	public float elapsed = 0f;

	public Vector3 startPosition;
	public Vector3 endPosition;
	public GameObject FocusTarget;

	public float HeightStart = 1000f;

	public float Revolutions = 2f;

	public Action action;

	private float initialDistance;
	private float finalDistance;

	private float initialAngle;
	private float finalAngle;

	public void Start()
	{
		elapsed = 0f;
		startPosition = new Vector3(FocusTarget.WorldPosition.x, FocusTarget.WorldPosition.y, HeightStart);
		Vector3 flatVecEnd = (endPosition - FocusTarget.WorldPosition).WithZ(0);
		initialDistance = Vector3.DistanceBetween(startPosition.WithZ(0), FocusTarget.WorldPosition.WithZ(0));
		finalDistance = Vector3.DistanceBetween(FocusTarget.WorldPosition.WithZ(0), endPosition.WithZ(0));
		finalAngle = MathF.Atan2(flatVecEnd.x, flatVecEnd.y);
		initialAngle = finalAngle + ((float)Math.PI * 2f * Revolutions);
		CameraManager.Instance.EffectOverride = true;
		CameraManager.Instance.WorldPosition = startPosition;
		Started = true;
	}

	public void Update(float delta)
	{
		elapsed += delta;
		float t = MathX.Clamp(elapsed / Duration, 0, 1f);
		float easedT = Ease.InOutSine(t);
		float currentDistance = MathX.Lerp(initialDistance, finalDistance, easedT);
		float currentAngle = MathX.Lerp(initialAngle, finalAngle, easedT);
		float x = (float)Math.Sin(currentAngle) * currentDistance;
		float y = (float)Math.Cos(currentAngle) * currentDistance;
		float z = MathX.Lerp(startPosition.z, endPosition.z, easedT);

		Vector3 newPos = new Vector3(x,y,z);

		CameraManager.Instance.WorldPosition = newPos;
		CameraManager.Instance.LocalRotation = Rotation.LookAt(FocusTarget.WorldPosition - newPos);
	}

	public void End()
	{
		if(action is not null) action?.Invoke();
		CameraManager.Instance.EffectOverride = false;
		Finished = true;
	}
	
	public bool IsComplete(float globalTime) => globalTime >= StartTime + Duration;
}
