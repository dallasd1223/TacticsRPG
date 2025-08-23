using Sandbox;
using System;

namespace TacticsRPG;

public sealed class MapManager : Component
{
	[Property] public ModelRenderer Model {get; set;}
	[Property] public MapData Data {get; set;}
	[Property] public GameObject Tiles {get; set;}
	[Property] public GameObject Node {get; set;}

	[Property] public BackgroundSky Background {get; set;}

	protected override void OnStart()
	{
		Model = GetComponent<ModelRenderer>();
		Data = GetComponent<MapData>();
	}
}
