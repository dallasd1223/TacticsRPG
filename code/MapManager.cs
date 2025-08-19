using Sandbox;
using System;

namespace TacticsRPG;

public sealed class MapManager : Component
{
	[Property] public ModelRenderer Model {get; set;}
	[Property] public MapData Data {get; set;}
	[Property] public GameObject Tiles {get; set;}

	protected override void OnStart()
	{
		Model = GetComponent<ModelRenderer>();
		Data = GetComponent<MapData>();
	}
}
