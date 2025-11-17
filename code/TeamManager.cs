using Sandbox;
using TacticsRPG;

namespace TacticsRPG;

public sealed class TeamManager : Component
{
	[Property] public TeamType Team {get; set;} = TeamType.Alpha;
	[Property] public List<BattleUnit> TeamUnits {get; set;} = new List<BattleUnit>();
	[Property] public bool AllUnitsDead {get; set;} = false;

	protected override void OnStart()
	{
		TeamUnits = GetAllTeamUnits();
	}
	public List<BattleUnit> GetAllTeamUnits()
	{
		return Scene.GetAll<BattleUnit>().Where(u => u.Team == Team).ToList();
	}

	public bool CheckAllDead()
	{
		return TeamUnits.All(p => p.Combat.HasDied);
	}
}

public enum TeamType
{
	Alpha,
	Omega,
}
