using Sandbox;
using TacticsRPG;

public sealed class InputManager : Component
{
	[Property] public InputMode Mode {get; set;} = InputMode.Battle;
	[Property] public DeviceMode Device {get; set;} = DeviceMode.Keyboard;
	
	protected override void OnUpdate()
	{
		HandleMasterInput();
	}


	public void HandleMasterInput()
	{
		switch(Mode)
		{
			case InputMode.System:
				HandleSystemInput();
				break;
			case InputMode.Game:
				HandleGameInput();
				break;
			case InputMode.Battle:
				HandleBattleInput();
				break;
		}
	}

	public void HandleSystemInput()
	{

	}

	public void HandleGameInput()
	{

	}

	public void HandleBattleInput()
	{
		if(Input.Pressed("Scoreboard"))
		{
			GameManager.Instance.SetDebugActive();
		}
		switch(BattleManager.Instance.CurrentBattleState)
		{
			case BattleState.BattleStart:
				if(Input.Pressed("Back"))
				{
					var b = EffectManager.Instance.TrySkipSequence();
					Log.Info($"Trying To Skip: {b}");
				}
				break;
			case BattleState.TurnStart:
				break;
			case BattleState.WaitForCommands:
				if((PlayerMaster.Instance.Mode == FocusMode.Menu && PlayerMaster.Instance.cMode == CommandMode.NA) || (PlayerMaster.Instance.Mode == FocusMode.FreeLook && PlayerMaster.Instance.cMode == CommandMode.NA))
				{
					if(Input.Pressed("Back"))
					{
						if(PlayerMaster.Instance.Menu.State == MenuState.Action)
						{
							PlayerMaster.Instance.SwitchFocusMode();
						}
						else
						{
							PlayerMaster.Instance.Menu.LastMenuState();
						}
						Log.Info("Back Pressed");
					}
				}
				if(PlayerMaster.Instance.Mode == FocusMode.ConfirmMenu)
				{
					if(Input.Pressed("Chat"))
					{
						PlayerMaster.Instance.ConfirmMenu.SelectOption();
						return;

					}
					if(Input.Pressed("Back"))
					{
						PlayerMaster.Instance.Cancel();
						return;
					}
					if(Input.Pressed("Left"))
					{
						PlayerMaster.Instance.ConfirmMenu.DecreaseIndex();
					}	
					if(Input.Pressed("Right"))
					{
						PlayerMaster.Instance.ConfirmMenu.IncreaseIndex();
					}				
				}
				if(PlayerMaster.Instance.Mode == FocusMode.Menu)
				{
					if(Input.Pressed("Forward"))
					{
						PlayerMaster.Instance.Menu.DecreaseIndex();
					}
					if(Input.Pressed("Backward"))
					{
						PlayerMaster.Instance.Menu.IncreaseIndex();
					}
					if(Input.Pressed("Chat"))
					{
						PlayerMaster.Instance.Menu.SelectItem();
						return;
					}
				}
				if(PlayerMaster.Instance.Mode == FocusMode.FreeLook && (PlayerMaster.Instance.cMode == CommandMode.MoveSelect || PlayerMaster.Instance.cMode == CommandMode.AttackSelect))
				{
					if(Input.Pressed("Chat"))
					{
						GameCursor.Instance.PreCheck();
						return;
					}
					if(Input.Pressed("Back"))
					{
						Log.Info("Back Pressed");
						PlayerMaster.Instance.CancelCommand();
						return;
					}			
				}

				break;
			case BattleState.ProcessCommands:
				break;
			case BattleState.TurnEnd:
				break;
			case BattleState.BattleEnd:
				break;
		}
	}
}

public enum InputMode
{
	System,
	Game,
	Battle,
}

public enum DeviceMode
{
	Mouse,
	Keyboard,
	Gamepad,
}
