using Sandbox;
using System;

namespace TacticsRPG;

public class Skill : IAbilityItem
{
	public AbilityItemData Data {get; set;}

	public Skill(SkillData data)
	{
		Data = data;
	}
}

[GameResource("Skill", "skill", "Defines Data For Skill Abilties")]
public class SkillData : AbilityItemData
{
	public override string Name {get; set;}
	public override string Description {get; set;}
	public override string IconPath {get; set;}

	public override EffectData effectData {get; set;}

	public int ManaCost {get; set;}
	public bool isChargeSkill {get; set;}
	public int TurnCost {get; set;}
	public int Duration {get; set;}

	public override bool CanUseOnSelf {get; set;}
	public override RangeShape Shape {get; set;}
	public override int BaseRange {get; set;}
	public override int ActionRange {get; set;}

	public override int Value {get; set;}

	public StatType Stat {get; set;}
	public ModifierType Modifier {get; set;}
}
