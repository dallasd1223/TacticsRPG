using Sandbox;
using System;

namespace TacticsRPG;

[Category("Unit")]
public class UnitCommand : Component
{

	[Property] public List<string> InitialCommands = new List<string>{"MOVE", "ACT", "WAIT", "STATUS"};
	[Property] public List<string> UnitCommandsList = new List<string>{"STATUS"};
	[Property] public List<CommandItem> UnitCommands {get; set;} = new List<CommandItem>();
	[Property] public List<CommandItem> CommandItems {get; set;} = new List<CommandItem>();
	[Property] public List<CommandItem> ActionCommands {get; set;} = new List<CommandItem>{new CommandItem("ATTACK")};
	[Property] public List<CommandItem> AbilityCommands {get; set;} = new List<CommandItem>();


	protected override void OnStart()
	{
		var unit = GetComponent<BattleUnit>();
		foreach(string s in InitialCommands)
		{
			CommandItems.Add(new CommandItem(s));
		}

		foreach(string s in UnitCommandsList)
		{
			UnitCommands.Add(new CommandItem(s));
		}
		if(unit.Skillset.PrimarySkillset != null)
		{
			ActionCommands.Add(new CommandItem(unit.Skillset.PrimarySkillset.Name, 0, 0, null, unit.Skillset.PrimarySkillset.ID));
			Log.Info($"Primary Skillset CommandItem: {unit.Skillset.PrimarySkillset.Name}");
		}
		if(unit.Skillset.SecondarySkillset != null)
		{
			ActionCommands.Add(new CommandItem(unit.Skillset.SecondarySkillset.Name, 0, 0, null, unit.Skillset.SecondarySkillset.ID));
			Log.Info($"Secondary Skillset CommandItem: {unit.Skillset.SecondarySkillset.Name}");
		}
		if(unit.UAbility.LearnedAbilities.Any())
		{
			foreach(Ability a in unit.UAbility.LearnedAbilities)
			{
				AbilityCommands.Add(new CommandItem(a.Data.Name, 0, 0, a, ""));
				Log.Info($"Ability CommandItem: {a.Data.Name}");
			}
		}
	}
	public void SetCommand(string s, bool b)
	{
		CommandItem com = CommandItems.Find(c => c.Text == s);
		com.Active = b;
		Log.Info($"CommandItem {s} Set To {b}");
	}

	public bool CommandIsActive(CommandItem item)
	{
		return item.Active;	
	}

	public List<CommandItem> GetAbilityCommandsBySkillset(string skillsetID)
	{
		var list = AbilityCommands
		.Where(x => x.AbilityIns.Data.SkillsetID == skillsetID);

		return list.ToList();
	}

}

public class CommandItem
{
	public virtual string Text {get; set;} = "";
	public bool Active = true;
	public Ability AbilityIns = null;
	public int Amount = 0;
	public int Cost = 0;
	public string SkillsetID = "";

	public void SetActive(bool b)
	{
		Active = b;
	}

	public CommandItem(string text, int amount = 0, int cost = 0, Ability ability = null, string skillsetID = "")
	{
		Text = text;
		Amount = amount;
		Cost = cost;
		AbilityIns = ability;
		SkillsetID = skillsetID;
	}
}




