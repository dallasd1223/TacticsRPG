using Sandbox;
using System;

namespace TacticsRPG;

public class Modifier
{
	public ModifierType Type {get; set;}

	public StatType Stat {get; set;}

	public int Value {get; set;}

}

public enum ModifierType
{
	Flat,
	Percentage,
}
