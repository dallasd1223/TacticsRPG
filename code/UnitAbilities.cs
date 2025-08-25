using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitAbilities : Component
{

	private List<Ability> _abilities = new();
	[Property] public IReadOnlyList<Ability> Abilities => _abilities;

	public event Action<Ability> OnAbilityAdded;

	public void AddAbility(Ability ability)
	{
		var ab = _abilities.FirstOrDefault(a => a == ability);
		if(ab != null)
		{
			return;
		}
		else
		{
			_abilities.Add(ability);
			OnAbilityAdded?.Invoke(ability);
		}
	}

	protected override void OnStart()
	{
		var ab = ResourceLibrary.Get<AbilityData>("resources/Abilities/ItemAbility.Ability");
		var abi = new Ability(ab);
		var ma = ResourceLibrary.Get<AbilityData>("resources/Abilities/MagicAbility.Ability");
		var mag = new Ability(ma);
		var sk = ResourceLibrary.Get<AbilityData>("resources/Abilities/SkillAbility.Ability");
		var skl = new Ability(sk);
		AddAbility(abi);
		AddAbility(mag);
		AddAbility(skl);
	}
}
