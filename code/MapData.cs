using Sandbox;

public sealed class MapData : Component
{
	[Property] public string Name {get; set;}
	[Property] public string MonthDay {get; set;}
	[Property] public int TimeOfDay {get; set;}
	[Property] public WeatherType Weather {get; set;} = WeatherType.Sunny;
}

public enum WeatherType 
{
	Sunny,
	Cloudy,
	Rainy,
	Special,
}
