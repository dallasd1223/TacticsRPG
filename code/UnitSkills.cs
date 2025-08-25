using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public sealed class UnitSkills : Component
{
	private List<Skill> _skills = new();
	[Property] public IReadOnlyList<Skill> Skills => _skills;

	public event Action<Skill> OnSkillAdded;

	public void AddSkill(Skill skill)
	{
		var sk = _skills.FirstOrDefault(s => s == skill);
		if(sk != null)
		{
			return;
		}
		else
		{
			_skills.Add(skill);
			OnSkillAdded?.Invoke(skill);
		}
	}

	protected override void OnStart()
	{
		var sk = ResourceLibrary.Get<SkillData>("resources/Skills/GutsSkill.Skill");
		var ski = new Skill(sk);
		AddSkill(ski);
	}
}
