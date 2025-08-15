using Sandbox;
using System;

namespace TacticsRPG;

public class UnitTurn : Component
{
	[Property] public bool HasMoved {get; set;} = false;
	[Property] public bool HasActed {get; set;}= false;
	[Property] public List<string> InitialCommands = new List<string>{"MOVE", "ATTACK", "ABILITY", "WAIT"};
	[Property] public List<CommandItem> CommandItems {get; set;} = new List<CommandItem>();
	[Property] public List<CommandItem> AbilityCommands {get; set;} = new List<CommandItem>();
	[Property] public List<CommandItem> ItemCommands {get; set;} = new List<CommandItem>(); 

	public event Action TurnStarted;
	public event Action TurnEnded;

	protected override void OnAwake()
	{
		TurnStarted += OnTurnStart;
		TurnEnded += OnTurnEnd;

	}

	protected override void OnStart()
	{
		foreach(string s in InitialCommands)
		{
			CommandItems.Add(new CommandItem(s));
		}
		var unit = GetComponent<Unit>();
		if(unit.UAbility.Abilities.Any())
		{
			foreach(Ability a in unit.UAbility.Abilities)
			{
				AbilityCommands.Add(new CommandItem(a.Data.Name));
				Log.Info(a.Data.Name);
			}
		}
		if(PlayerMaster.Instance.Inventory.Items.Any())
		{
			foreach(InventorySlot slot in PlayerMaster.Instance.Inventory.Items)
			{
				ItemCommands.Add(new CommandItem(slot.SlotItem.Data.Name, slot.Quantity));
				Log.Info(slot.SlotItem.Data.Name);
			}
		}

	}
	public void SetCommand(string s, bool b)
	{
		CommandItem com = CommandItems.Find(c => c.Text == s);
		com.Active = b;
		Log.Info($"CommandItem {s} Set To {b}");
	}

	public bool CommandIsActive(string s)
	{
		CommandItem com = CommandItems.Find(c => c.Text == s);
		return com.Active;	
	}

	public void StartTurn()
	{
		TurnStarted?.Invoke();
	}

	public void EndTurn()
	{
		var unit = GetComponent<Unit>();
		unit.Move.UnitTile.current = false;
		TurnEnded?.Invoke();
	}

	private void OnTurnStart()
	{
		HasActed = false;
		HasMoved = false;
		foreach(CommandItem com in CommandItems)
		{
			com.Active = true;
		}
		Log.Info("Turn Started");
	}

	private void OnTurnEnd()
	{

		Log.Info("Turn Ended");
	}
}

public class CommandItem
{
	public virtual string Text {get; set;} = "";
	public bool Active = true;
	public int Amount = 0;
	public void SetActive(bool b)
	{
		Active = b;
	}

	public CommandItem(string text, int amount = 0)
	{
		Text = text;
	}
}




