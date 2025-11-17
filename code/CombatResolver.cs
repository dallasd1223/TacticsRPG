using Sandbox;

namespace TacticsRPG;

public static class CombatResolver
{
	public static CombatResult ResolveAttack(BattleUnit Attacker, BattleUnit Target)
	{
		bool didhit = RollHitChance(Attacker.Stats.GetStat(StatType.ACC), Target.Stats.GetStat(StatType.EV));
		bool crit = RollCritChance();
		int damage = Attacker.Stats.GetStat(StatType.ATK);
		return new CombatResult
		{
			DidHit = didhit,
			Type = AttackResultType.Attacked,
			DamageAmount = damage,
			Crit = crit,
		};
	}

	public static bool RollHitChance(int accuracy, int evasion)
	{
		int finalChance = accuracy - evasion;
		int roll = Game.Random.Int(0,100);
		return roll <= finalChance;
	}

	public static bool RollCritChance()
	{
		int roll = Game.Random.Int(0,100);
		return roll <= 95;
	}
}


public struct CombatResult
{
	public bool DidHit;
	public AttackResultType Type;
	public int DamageAmount;
	public bool Crit;

}

public enum AttackResultType
{
	Attacked,
	Missed,
	Blocked,
	Dodged,
}


