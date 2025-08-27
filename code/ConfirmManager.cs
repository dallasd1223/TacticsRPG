using Sandbox;
using System;

namespace TacticsRPG;

public sealed class ConfirmManager : Component
{
	[Property] ConfirmUI UI {get; set;}
	[Property] public CommandMode? Mode {get; set;}

	protected override void OnAwake()
	{
		UI = GetComponent<ConfirmUI>();
		PlayerEvents.CommandModeChange += HandleCommandMode;
	} 

	private void HandleCommandMode(CommandMode? mode)
	{
		Mode = mode;
	}

	protected override void OnDestroy()
	{
		PlayerEvents.CommandModeChange += HandleCommandMode;		
	}

}
