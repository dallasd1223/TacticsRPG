using Sandbox;
using System;
using SpriteTools;

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
	public string ID {get; set;}
	public string Name {get; set;}
	public string Description {get; set;}
	public string IconPath {get; set;}
	public SpriteResource Sprite {get; set;}

	public string SkillsetID {get; set;}

	public string FormulaID {get; set;}

	public EffectData effectData {get; set;}
	public string EffectID {get; set;}

	public AbilityType Type {get; set;}
	
	public int JPCost {get; set;}

	public int ManaCost {get; set;}
	public bool IsChargeSpell {get; set;}
	public int TurnCost {get; set;}
	public int Duration {get; set;}

	public bool CanUseOnSelf {get; set;}
	public RangeShape Shape {get; set;}
	public int BaseRange {get; set;}
	public int ActionRange {get; set;}

	public int Value {get; set;}

	public StatType Stat {get; set;}
	public ElementType Element {get; set;}

}

public interface IActivate
{
	public void Activate(){}
}

