using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitAbilities : Component
{
	public Dictionary<string, UnitAbilityData> UnitAbilityDictionary = new();
	[Property]	public List<Ability> LearnedAbilities = new();

	[Property] public Ability SupportAbility {get; set;}
	[Property] public Ability ReactionAbility {get; set;}
	[Property] public Ability MovementAbility {get; set;}

	public event Action<Ability> OnAbilityAdded;

	public void LearnAbility(string ID)
	{
		UnitAbilityDictionary.TryGetValue(ID, out var abilityData);
		if(abilityData != null && abilityData.IsLearned == false)
		{
			abilityData.IsLearned = true;
			var ability = new Ability(AbilityDatabase.Get(ID));
			LearnedAbilities.Add(ability);
			OnAbilityAdded?.Invoke(ability);
		}
	}

	void FillAbilityDictionary()
	{
		foreach(AbilityData data in AbilityDatabase.GetAll())
		{
			if(!UnitAbilityDictionary.ContainsKey(data.ID))
			{
				UnitAbilityDictionary[data.ID] = new UnitAbilityData
				{
					AbilityID = data.ID,
					IsLearned = false
				};

				Log.Info($"Ability {data.Name} Added To UnitAbilityDictionary");
			}
		}
	}
	void AddLearnedAbilities()
	{
		var skillsets = GetComponent<UnitSkillset>();
		var prim = skillsets.PrimarySkillset;
		var sec = skillsets.SecondarySkillset;
		if(!UnitAbilityDictionary.Any()) return;
		if(prim == null && sec == null) return;

		Log.Info("Starting Ability Loop");
		foreach(string abilityID in prim.AbilityIDs)
		{
			if(UnitAbilityDictionary.ContainsKey(abilityID))
			{
				UnitAbilityDictionary[abilityID].IsLearned = true;
				LearnedAbilities.Add(new Ability(AbilityDatabase.Get(abilityID)));
				Log.Info($"Primary Skillset Ability Learned: {abilityID} {AbilityDatabase.Get(abilityID).Name}");
			}
		}
		foreach(string abilityID in sec.AbilityIDs)
		{
			if(UnitAbilityDictionary.ContainsKey(abilityID))
			{
				UnitAbilityDictionary[abilityID].IsLearned = true;
				LearnedAbilities.Add(new Ability(AbilityDatabase.Get(abilityID)));
				Log.Info($"Secondary Skillset Ability Learned: {abilityID} {AbilityDatabase.Get(abilityID).Name}");
			}
		}

	}

	void LoadOtherAbilities()
	{
		SupportAbility = new Ability(AbilityDatabase.Get("006"));
		Log.Info($"Support Ability Loaded: {SupportAbility.Data.Name}");
		ReactionAbility = new Ability(AbilityDatabase.Get("005"));
		Log.Info($"Reaction Ability Loaded: {ReactionAbility.Data.Name}");
		MovementAbility = new Ability(AbilityDatabase.Get("004"));
		Log.Info($"Movement Ability Loaded: {MovementAbility.Data.Name}");
	}
	protected override void OnStart()
	{
		FillAbilityDictionary();
		AddLearnedAbilities();
		LoadOtherAbilities();
	}
}
