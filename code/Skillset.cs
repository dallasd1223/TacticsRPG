using Sandbox;

public class SkillSet
{
	public SkillsetData Data {get; set;}
}

[GameResource("Skillset", "set", "Defines Skillset Data")]
public class SkillsetData: GameResource
{

	public string ID {get; set;}
	public string Name {get; set;}
	public string Description {get; set;}
	public string JobID {get; set;}
	public List<string> AbilityIDs {get; set;} = new();	
}
