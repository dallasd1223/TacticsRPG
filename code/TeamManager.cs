using Sandbox;
using TacticsRPG;

namespace TacticsRPG;

public sealed class TeamManager : Component
{
	[Property] public TeamType Team {get; set;} = TeamType.Alpha;
	[Property] public List<Unit> TeamUnits {get; set;} = new List<Unit>();
	[Property] public bool AllUnitsDead {get; set;} = false;

	protected override void OnStart()
	{
		TeamUnits = GetAllTeamUnits();
	}
	public List<Unit> GetAllTeamUnits()
	{
		return Scene.GetAll<Unit>().Where(u => u.Team == Team).ToList();
	}

	public bool CheckAllDead()
	{
		return TeamUnits.All(p => p.Battle.HasDied);
	}
}

public enum TeamType
{
	Alpha,
	Omega,
}
