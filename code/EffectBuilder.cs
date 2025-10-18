using Sandbox;
using System;

namespace TacticsRPG;

public class EffectSequenceBuilder
{
	private EffectSequence _sequence = new EffectSequence();

	public EffectSequenceBuilder SetID(int id)
	{
		_sequence.SetID(id);
		return this;
	}

	public EffectSequenceBuilder AddStep(IEffectStep step)
	{
		_sequence.AddStep(step);
		return this;
	}

	public EffectSequenceBuilder SetAllComplete(Action act)
	{
		_sequence.SetAllComplete(act);
		return this;
	}

	public EffectSequence Finish()
	{
		return _sequence;
	}
}

public class EffectBuilder
{
	private EffectEvent _effect;

	public EffectBuilder(EffectEvent e)
	{
		_effect = e;
	} 

	public EffectBuilder WithCObject(CombatObject obj)
	{
		_effect.CObject = obj;
		return this;
	}

	public EffectBuilder WithData(EffectData data)
	{
		_effect.Data = data;
		return this;
	}

	public EffectBuilder SetCallBack(Action act)
	{
		_effect.OnCompleteAll = act;
		return this;
	}

	public EffectBuilder SetSequences()
	{
		_effect.SetSequences();
		Log.Info($"Right After Sequence Count: {_effect.Sequences.Count()}");
		return this;
	}

	public EffectEvent Build()
	{
		Log.Info($"Effect Built {_effect}");
		Log.Info($"Builder Sequence Count: {_effect.Sequences.Count()}");
		return _effect;
	}
}

public class EffectEvent
{
	public virtual CombatObject CObject {get; set;}
	public virtual EffectData Data {get; set;}
	public virtual Action OnCompleteAll {get; set;}
	public virtual List<EffectSequence> Sequences {get; set;}= new();

	public virtual void SetSequences() {}
}

public class PotionEffect: EffectEvent
{
	public override CombatObject CObject {get; set;}
	public override EffectData Data {get; set;}
	public override Action OnCompleteAll {get; set;}
	public override List<EffectSequence> Sequences {get; set;}

	public PotionEffect(CombatObject co, EffectData e)
	{
		Log.Info(co);
		Log.Info(e);
		CObject = co;
		Data = e;

		List<EffectSequence> list = new List<EffectSequence>{
			new EffectSequenceBuilder().SetID(0)
				.AddStep(
					new ColorMapStep{
					StartTime = Data.stepData[0].StartTime,
					Duration = Data.stepData[0].Duration,
					ToColor = Data.stepData[0].Color,
					Map = BattleMachine.Instance.Map.Model,
				})
				.AddStep(
					new ColorSkyBoxStep
					{
						StartTime = Data.stepData[1].StartTime,
						Duration = Data.stepData[1].Duration,
						ToColor = Data.stepData[1].Color,
						Skybox = BattleMachine.Instance.Map.Background.Skybox,
					}
				)
				.AddStep(
					new SpawnPrefabStep{
					StartTime = Data.stepData[2].StartTime,
					Duration = Data.stepData[2].Duration,
					StartPosition = CObject.ActingUnit.GameObject.WorldPosition + Data.stepData[2].StartPosition,
					SpawnObject = Data.stepData[2].Resource,
				})
				.AddStep(
					new SpawnParticlePrefabStep{
						StartTime = Data.stepData[3].StartTime,
						Duration = Data.stepData[3].Duration,
						SpawnPosition = CObject.ActingUnit.GameObject.WorldPosition + Data.stepData[3].StartPosition,
						ParticlePrefab = Data.stepData[3].Resource,
					}
				)
				.AddStep(
					new SpawnPointLightStep{
						StartTime = Data.stepData[4].StartTime,
						Duration = Data.stepData[4].Duration,
						ToColor = Data.stepData[4].Color,
						SpawnPosition = CObject.ActingUnit.GameObject.WorldPosition + Data.stepData[4].StartPosition,						
					}
				)
				.AddStep(
					new PlaySoundStep{
						StartTime = Data.stepData[5].StartTime,
						Duration = Data.stepData[5].Duration,	
						sound = Data.stepData[5].Sound,			
					}
				)
				.AddStep(
					new ColorSpriteStep{
						StartTime = Data.stepData[6].StartTime,
						Duration = Data.stepData[6].Duration,
						ToColor = Data.stepData[6].Color,
						Sprite = CObject.ActingUnit.Animator.UnitSprite,				
					}
				)
				.AddStep(
					new PlaySoundStep{
						StartTime = Data.stepData[7].StartTime,
						Duration = Data.stepData[7].Duration,	
						sound = Data.stepData[7].Sound,							
					}
				)
				.AddStep(
					new CreateIntValueTextStep{
						StartTime = Data.stepData[8].StartTime,
						Duration = Data.stepData[8].Duration,
						unit = CObject.AffectedUnit,
						Amount = CObject.AbilityItem.Data.Value,
						color = Data.stepData[8].Color,	
						size = 60,
					}
				)
				.Finish(),

				new EffectSequenceBuilder().SetID(1)
				.AddStep(
					new ColorMapStep{
						StartTime = Data.stepData[9].StartTime,
						Duration = Data.stepData[9].Duration,
						ToColor = Data.stepData[9].Color,
						Map = BattleMachine.Instance.Map.Model,						
					}
				)
				.AddStep(
					new ColorSkyBoxStep{
						StartTime = Data.stepData[10].StartTime,
						Duration = Data.stepData[10].Duration,
						ToColor = Data.stepData[10].Color,
						Skybox = BattleMachine.Instance.Map.Background.Skybox,
					}
				)
				.AddStep(
					new ColorSpriteStep{
						StartTime = Data.stepData[11].StartTime,
						Duration = Data.stepData[11].Duration,
						ToColor = Data.stepData[11].Color,
						Sprite = CObject.ActingUnit.Animator.UnitSprite,	
					}
				)
				.Finish(),

		};

		Sequences = list;
	}

	public override void SetSequences()
	{
		base.SetSequences();
		Log.Info("Setting Sequence");
		if(Data is null)
		{
			Log.Info("No Effect Data Found");
			return;
		}

		Log.Info($"Sequence Count: {Sequences.Count()} {Sequences[0].SequenceID}");
	}

}

public class EffectContext
{

}

[GameResource("Effect", "effect", "Defines Effect Data")]
public class EffectData: GameResource
{
	public string Name {get; set;}
	public int ID {get; set;}
	public int SequenceCount {get; set;}
	public int TotalDuration {get; set;}
	public List<StepData> stepData {get; set;}= new();

}

public class StepData
{
	public string StepName {get; set;}
	public int SequenceNum {get; set;}
	public float StartTime {get; set;}
	public float Duration {get; set;}
	public float Revolutions {get; set;}
	public float HeightStart {get; set;}
	public Curve CurveData {get; set;}
	public Vector3 StartPosition {get; set;}
	public Vector3 EndPosition {get; set;}
	public Vector3 FocusPosition {get; set;}
	public Color Color {get; set;}
	public GameObject Resource {get; set;}
	public SoundEvent Sound {get; set;}
	public string AnimationName {get; set;}
}
