using Sandbox;

public sealed class UnitData : Component
{
	[Property] public string Name {get; set;}

	[Property] public int Level {get; set;} = 1;
	[Property] public int MaxHP {get; set;} = 20;
	[Property] public int CurrentHP {get; set;} = 20;
	[Property] public int MaxMP {get; set;} = 10;
	[Property] public int CurrentMP {get; set;} = 10;
	[Property] public int Speed {get; set;} = 3;
	[Property] public int Accuracy {get; set;} = 90;
	[Property] public int Strength {get; set;} = 15;
	[Property] public int Evasion {get; set;} = 5;


}
