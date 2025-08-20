using Sandbox;
using System;

namespace TacticsRPG;

public sealed class UnitSpells : Component
{
	private List<Spell> _spells = new();
	[Property] public IReadOnlyList<Spell> Spells => _spells;

	public event Action<Spell> OnSpellAdded;

	public void AddSkill(Spell spell)
	{
		var sp = _spells.FirstOrDefault(s => s == spell);
		if(sp != null)
		{
			return;
		}
		else
		{
			_spells.Add(spell);
			OnSpellAdded?.Invoke(spell);
		}
	}

	protected override void OnStart()
	{
		var sp = ResourceLibrary.Get<SpellData>("resources/Spells/Fire1.Spell");
		var spe = new Spell(sp);
		AddSkill(spe);	
	}
}
