using Sandbox;
using System;

namespace TacticsRPG;

public interface IAbilityItem
{
	public AbilityItemData Data {get; set;}
}

public class Spell : IAbilityItem
{
	public AbilityItemData Data {get; set;}

	public Spell(SpellData data)
	{
		Data = data;
	}
}

public interface ILearnable
{
	public AbilityEnum ParentAbility {get; set;}
	public int JPCost {get; set;}
}
[GameResource("Spell", "spell", "Defines Data For Spells")]
public class SpellData : AbilityItemData, ILearnable
{
	public int ID {get; set;}
	public override string Name {get; set;}
	public override string Description {get; set;}
	public override string IconPath {get; set;}

	public override EffectData effectData {get; set;}

	public AbilityEnum ParentAbility {get; set;}
	public int JPCost {get; set;}

	public int ManaCost {get; set;}
	public bool IsChargeSpell {get; set;}
	public int TurnCost {get; set;}
	public int Duration {get; set;}

	public override bool CanUseOnSelf {get; set;}
	public override RangeShape Shape {get; set;}
	public override int BaseRange {get; set;}
	public override int ActionRange {get; set;}

	public override int Value {get; set;}

	public StatType Stat {get; set;}
	public ElementType Element {get; set;}
}

public enum ElementType
{
	Fire,
	Ice,
	Lightning,
	Poison,
	Dark,
	Heal,
	Other,
}


