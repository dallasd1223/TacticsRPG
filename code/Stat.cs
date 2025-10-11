using Sandbox;
using System;

namespace TacticsRPG;

public enum StatType
{
	LVL, // LEVEL
	EXP, // EXPERIENCE POINTS
	MAXHP, // MAX HEALTH POINTS
	HP,	// HEALTH POINTS
	MAXMP, // MAX MANA POINTS
	MP, // MANA POINTS
	CTR, //CHARGE TIME
	MOV, // MOVE RANGE
	ATK, // ATTACK
	MAG, // MAGIC
	DEF, // DEFENSE
	SPD, // SPEED
	JMP, // JUMP
	ACC, // ACCURACY
	EV, // EVASION
}

[GameResource("Stat", "stat", "Defines Unit/Job Stats")]
public class StatData : GameResource
{
	public Dictionary<StatType, int> Stats {get; set;}
}
