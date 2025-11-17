using Sandbox;
using System;

namespace TacticsRPG;

public class StatusEffect
{
	public StatusEffectData Data;

	public void Apply(BattleUnit u)
	{
		foreach(Modifier mod in Data.Modifiers)
		{
			//DoStuff
			Log.Info($"Do Stuff To {u} Stats"); 
		}
	}

	public StatusEffect(StatusEffectData data)
	{
		Data = data;
	}
}

[GameResource("Status", "status", "Defines Data For Status Effects")]
public class StatusEffectData : GameResource
{
	public int ID {get; set;}
	public string Name {get; set;}
	public string Description {get; set;}

	public StatusEffectType Type {get; set;}

	public int Duration {get; set;} // -1 For Infinite

	public List<Modifier> Modifiers {get; set;} = new();
}

public enum StatusEffectType
{
	Frozen,
	Eletrocuted,
	Burning,
	Poisoned,
	Charmed,
	Blinded,
}
