using Sandbox;
using System;

namespace TacticsRPG;
[GameResource("Test", "test", "For Testing Resource Functionality")]
public class TestResourceData : GameResource
{
	[Property] public List<IAbilityExecute> execute {get; set;} = new();
	[Property] public List<FocusMode> enumlist {get; set;} = new();
	public void ExecuteAll(Unit source)
	{
		foreach(IAbilityExecute ex in execute)
		{
			ex.Execute(source);
		}
	}

	[Button("Add DamageClass")]
	public void AddDamageTest()
	{
		var type = TypeLibrary.GetType<DamageExecute>();
		var damage = type.Create<DamageExecute>();
		Log.Info($"{type.ClassName} Type Libary");
		execute.Add(damage);
	}

	protected override void PostLoad()
	{
		base.PostLoad();


	}
}



public class IAbilityExecute
{
	public virtual int Amount {get; set;}

	public virtual void Execute(Unit source)
	{
		Log.Info($"Unit {source.Data.Name} Base Execute, but its something");
	}

	
}

public class DamageExecute : IAbilityExecute
{
	public override int Amount {get; set;}= 0;

	public override void Execute(Unit source)
	{
		base.Execute(source);
		var rb =source.GameObject.AddComponent<Prop>();
		Log.Info($"Prop Added {rb}");
		Log.Info($"Amount:{Amount} Execution from DamageExecute resource success");
	}

	public DamageExecute(){}
}

public class HealExecute : IAbilityExecute
{

	public int Amount {get; set;}= 0;

	public override void Execute(Unit source)
	{
		base.Execute(source);
		Log.Info("Execution from HealExecute resource success");
	}
}
