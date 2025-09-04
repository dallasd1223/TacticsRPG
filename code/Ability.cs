using Sandbox;
using System;

namespace TacticsRPG;

public class Ability: IActivate
{
	public AbilityData Data;

	public Ability(AbilityData data)
	{
		Data = data;
	}
}

[GameResource("Ability", "ability", "Defines Ability Data")]
public class AbilityData : GameResource
{
	public int ID {get; set;}
	public string Name {get; set;}
	public string Description {get; set;}
	public JobEnum Job {get; set;}
	public int EffectID {get; set;}
	public Effect EffectType {get; set;}
	public bool IsPassive {get; set;}
	public int MPCost {get; set;}
	public bool IsCharge {get; set;}
	public int TurnCost {get; set;}
	public int Value {get; set;}
}

public interface IActivate
{
	public void Activate(){}
}

public enum AbilityEnum
{
	Magic,
	Skill,
	Inventory,
}
