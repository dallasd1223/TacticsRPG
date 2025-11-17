using Sandbox;

namespace TacticsRPG;

public sealed class UnitSkillset : Component
{
	[Property] public SkillsetData PrimarySkillset {get; set;}
	[Property] public SkillsetData SecondarySkillset {get; set;}

	protected override void OnStart()
	{
		LoadSkillsets(PrimarySkillset, SecondarySkillset);
	}
	void LoadSkillsets(SkillsetData primary, SkillsetData secondary)
	{
		Log.Info($"Unit Primary Skillset: {primary.Name} {primary.ID}");
		Log.Info($"Unit Secondary Skillset: {secondary.Name} {secondary.ID}");
	}
}
