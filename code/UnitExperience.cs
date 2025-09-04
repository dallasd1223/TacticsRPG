using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitExperience : Component
{
	[Property] public UnitStats Stats {get; set;}

	[Property] public bool HasLeveledUp {get; set;}

	public void AddXP(int amount)
	{

	}

	public void LevelUp()
	{

	}

	public int UntilLevelUp()
	{
		return 1;
	}


	protected override void OnAwake()
	{
		Stats = GetComponent<UnitStats>();
	}


}
