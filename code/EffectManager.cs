using Sandbox;
namespace TacticsRPG;
public sealed class EffectManager : Component
{
	[Property] public List<EffectSequence> SequenceList {get; set;} = new();

	public static EffectManager Instance {get; set;}

	public bool SkipSequence = false;
	
	protected override void OnAwake()
	{
		Instance = this;
	}
	protected override void OnUpdate()
	{
		if(SequenceList.Any())
		{

			foreach(EffectSequence seq in SequenceList)
			{
				if(seq.IsFinished)
				{
					SequenceList.Remove(seq);
				}

				seq.Update(Time.Delta);
			}
			if(SkipSequence)
			{
				ForceEndCurrent();
				SkipSequence = false;	
			}
		}

		//Maybe Move This To Its Own Component 
		TweenManager.Update(Time.Delta);
	}

	public void AddSequence(EffectSequence seq)
	{
		SequenceList.Add(seq);
		seq.Start();
	}

		public void AddSequence(EffectSequence seq, Action act)
	{
		SequenceList.Add(seq);
		seq.Start(act);
	}

	public void ForceEndAll()
	{
		if(SequenceList.Any())
		{
			foreach(EffectSequence seq in SequenceList)
			{
				seq.ForceEnd();
			}
		}
	}

	public void ForceEndCurrent()
	{
		if(SequenceList.Any())
		{
			foreach(EffectSequence seq in SequenceList)
			{
				if(seq.IsPlaying)
				{
					seq.ForceEnd();
				}
			}
		}
	}

	public bool TrySkipSequence()
	{
		if(SequenceList.Any())
		{
			Log.Info("Skip Sequence Set To True");
			SkipSequence = true;
		}
		Log.Info("No Sequence To Skip");
		return false;
	}

}
