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

	public PotionEffect(EffectData e)
	{
		Data = e;

		List<EffectSequence> list = new List<EffectSequence>{
			new EffectSequenceBuilder().SetID(0).AddStep(new PlaySoundStep{
				sound = Data.stepData[0].Sound,
				StartTime = Data.stepData[0].StartTime,
				Duration = Data.stepData[0].Duration,
			}).Finish(),
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
		List<EffectSequence> list = new List<EffectSequence>{
			new EffectSequenceBuilder().SetID(0).AddStep(new PlaySoundStep{
				sound = Data.stepData[0].Sound,
				StartTime = Data.stepData[0].StartTime,
				Duration = Data.stepData[0].Duration,
			}).Finish(),
		};

		Sequences = list;
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
	public List<StepData> stepData {get; set;}= new();
	public List<GameObject> Particles {get; set;}
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
	public SoundEvent Sound {get; set;}
	public string AnimationName {get; set;}
}
