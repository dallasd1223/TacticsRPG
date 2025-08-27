using Sandbox;
using System;
using TacticsRPG;

public sealed class TestComponent : Component
{

	[Property] public TestResourceData Data {get; set;}
	[Property] public Unit unit {get; set;}

	protected override void OnStart()
	{
	
	/*	var types = TypeLibrary.GetTypes<IAbilityExecute>();
		foreach(var type in types)
		{
			var t = TypeLibrary.Create<TypeLibrary.GetType(type.ClassName)>();
			t.Execute(unit);

		} */


	
		Data.ExecuteAll(unit);
	}
}
