using Sandbox;
using System;

namespace TacticsRPG;

public interface IAbilityItem
{

}

public class Spell : IAbilityItem
{
	public SpellData Data;

	public Spell(SpellData data)
	{
		Data = data;
	}
}

[GameResource("Spell", "spell", "Defines Data For Spells")]
public class SpellData : GameResource
{
	public string Name {get; set;}
	public string Description {get; set;}
	public string IconPath {get; set;}

	public EffectData effectData {get; set;}

	public int ManaCost {get; set;}
	public bool IsChargeSpell {get; set;}
	public int TurnCost {get; set;}
	public int Duration {get; set;}

	public bool CanUseOnSelf {get; set;}
	public RangeShape Shape {get; set;}
	public int Range {get; set;}

	public int Value {get; set;}

	public StatType Stat {get; set;}
	public ElementType Element {get; set;}
	public ModifierType Modifier {get; set;}



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

public enum ModifierType
{
	Increase,
	Decrease,
	Buff,
	Debuff,
}
