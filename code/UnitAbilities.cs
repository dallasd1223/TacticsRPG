using Sandbox;
using System;

namespace TacticsRPG;

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
		var ab = ResourceLibrary.Get<AbilityData>("Abilities/ItemAbility.Ability");
		var abi = new Ability(ab);
		AddAbility(abi);
	}
}
