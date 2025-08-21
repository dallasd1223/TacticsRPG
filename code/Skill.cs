using Sandbox;
using System;

namespace TacticsRPG;

public class Skill : IAbilityItem
{
	public SkillData Data;

	public Skill(SkillData data)
	{
		Data = data;
	}
}

[GameResource("Skill", "skill", "Defines Data For Skill Abilties")]
public class SkillData : GameResource
{
	public string Name {get; set;}
	public string Description {get; set;}
	public string IconPath {get; set;}

	public EffectData effectData {get; set;}

	public int ManaCost {get; set;}
	public bool isChargeSkill {get; set;}
	public int TurnCost {get; set;}
	public int Duration {get; set;}

	public bool CanUseOnSelf {get; set;}
	public RangeShape Shape {get; set;}
	public int BaseRange {get; set;}
	public int ActionRange {get; set;}

	public int Value {get; set;}

	public StatType Stat {get; set;}
	public ModifierType Modifier {get; set;}
}
