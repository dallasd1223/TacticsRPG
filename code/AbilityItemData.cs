using Sandbox;
using System;

namespace TacticsRPG;

public class AbilityItemData : GameResource
{
	public virtual string Name {get; set;}
	public virtual string Description {get; set;}
	public virtual string IconPath {get; set;}

	public virtual EffectData effectData {get; set;}

	public virtual bool CanUseOnSelf {get; set;}
	public virtual RangeShape Shape {get; set;}
	public virtual int BaseRange {get; set;}
	public virtual int ActionRange {get; set;}

	public virtual int Value {get; set;}	
}
