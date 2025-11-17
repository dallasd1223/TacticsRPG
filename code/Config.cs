using Sandbox;
using System;

[GameResource("Config", "config", "Defines Values Different System")]
public abstract class Config : GameResource
{

	
}

[GameResource("BattleConfig", "bconfig", "Defines Values For Battle System")]
public class BattleConfig : Config
{
	public float INTRO_SPIRAL_DURATION {get; set;}
}

[GameResource("GameConfig", "gconfig", "Defines Values For Game System")]
public class GameConfig : Config
{

}

[GameResource("SysConfig", "sconfig", "Defines Values For Core System")]
public class SystemConfig : Config
{
	
}
