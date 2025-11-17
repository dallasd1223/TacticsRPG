using Sandbox;
using System;

namespace TacticsRPG;

public static class FormulasDictionary
{
	private static Dictionary<string, Func<CombatObject, CombatObject>> _formulaDictionary = new()
	{
		{"001:heal1", (co) => { var formula = new HealFormula(); formula.ApplyFormula(co); return co; } },
	};

	public static CombatObject UseFormula(string formulaID, CombatObject co)
	{
		if (_formulaDictionary.TryGetValue(formulaID, out var formulaFunc))
		{
			return formulaFunc(co);
		}
		else
		{
			throw new ArgumentException($"Formula type '{formulaID}' is not recognized.");
		}
	}
}

public interface Formula
{
	void ApplyFormula(CombatObject co);
}

public class HealFormula : Formula
{
	public void ApplyFormula(CombatObject co)
	{
		//Heal Logic Here
	}
}
