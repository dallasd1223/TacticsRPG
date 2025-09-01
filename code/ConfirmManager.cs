using Sandbox;
using System;

namespace TacticsRPG;

public sealed class ConfirmManager : Component
{
	[Property] bool IsConfirming {get; set;} = false;
	[Property] ConfirmUI UI {get; set;}
	[Property] public CommandMode? Mode {get; set;}
	[Property] public SelectorState State {get; set;}

	public event Action ConfirmStart;
	public event Action ConfirmCancel;
	public event Action ConfirmEnd;

	protected override void OnAwake()
	{
		UI = GetComponent<ConfirmUI>();
		PlayerEvents.CommandModeChange += HandleCommandMode;
		PlayerEvents.ValidSelection += HandleOnSelection;

	} 

	protected void Deactivate()
	{
		State = null;
		UI.Deactivate();
		IsConfirming = false;
		InputEvents.ActionSelectInputPressed -= HandleInput;
	}

	private void HandleInput(InputKey key)
	{
		if(!IsConfirming) return;
		switch(key)
		{
			case InputKey.LEFT:
				UI.DecreaseIndex();
				break;
			case InputKey.RIGHT:
				UI.IncreaseIndex();
				break;
			case InputKey.ENTER:
				HandleConfirm(UI.SelectOption());
				break;
			case InputKey.BACKSPACE:
				CancelSelection();
				break;
		}
	}
	private void HandleCommandMode(CommandMode? mode)
	{
		Mode = mode;
	}
	
	private void HandleOnSelection(SelectorState state)
	{
		Log.Info("Setting Confirm State");
		State = state;
		UI.Activate();
		IsConfirming = true;
		ConfirmStart?.Invoke();
		InputEvents.ActionSelectInputPressed += HandleInput;
	}

	public void HandleConfirm(string s)
	{
		if(s == "YES") ConfirmSelection();
		else CancelSelection();
	}

	public void ConfirmSelection()
	{
		Log.Info("Confirming Selection");
		State.ConfirmSelection();
		Deactivate();
		ConfirmEnd?.Invoke();

	}

	public void CancelSelection()
	{
		Deactivate();
		PlayerEvents.OnCancelSelection();
		ConfirmCancel?.Invoke();
	}

	public string GetStringFromMode()
	{
		switch(Mode)
		{
			case CommandMode.MoveSelect:
				return "Move Here?";
			case CommandMode.AttackSelect:
				return "Attack On This Unit?";
			case CommandMode.AbilitySelect:
				return "Use Ability Here?";
			case CommandMode.WaitSelect:
				return "End Turn?";
			default:
				return "";
		}
	}
	protected override void OnDestroy()
	{
		PlayerEvents.CommandModeChange -= HandleCommandMode;	
		PlayerEvents.ValidSelection -= HandleOnSelection;
		InputEvents.ActionSelectInputPressed -= HandleInput;	
	}

}
