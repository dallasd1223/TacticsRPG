using Sandbox;
using TacticsRPG;

public sealed class InputManager : Component
{
	[Property] public InputMode Mode {get; set;} = InputMode.Battle;
	[Property] public DeviceMode Device {get; set;} = DeviceMode.Keyboard;

	public event Action<InputKey> InputPressed;
	/*
	-URGENT-
	REFACTOR ALL INPUT HANDLING INTO APPROPRIATE STATE MACHINES
	THIS IS RETARDED & LAZY
	-URGENT-
	*/


	protected override void OnUpdate()
	{
		HandleMasterInput();
		HandleInputEvents();
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
				//HandleBattleInput();
				break;
		}
	}

	public void HandleSystemInput()
	{

	}

	public void HandleGameInput()
	{

	}
	public void HandleInputEvents()
	{
		if(Input.Pressed("Menu"))
		{
			InputPressed?.Invoke(InputKey.DEBUG);
		}
		if(Input.Pressed("Score"))
		{
			InputPressed?.Invoke(InputKey.TAB);
		}
		if(Input.Pressed("Back"))
		{
			InputPressed?.Invoke(InputKey.BACKSPACE);
		}
		if(Input.Pressed("Chat"))
		{
			InputPressed?.Invoke(InputKey.ENTER);
		}
		if(Input.Pressed("Left"))
		{
			InputPressed?.Invoke(InputKey.LEFT);
		}
		if(Input.Pressed("Right"))
		{
			InputPressed?.Invoke(InputKey.RIGHT);
		}
		if(Input.Pressed("Forward"))
		{
			InputPressed?.Invoke(InputKey.FORWARD);
		}
		if(Input.Pressed("Backward"))
		{
			InputPressed?.Invoke(InputKey.BACKWARD);			
		}
	}
/*	public void HandleBattleInput()
	{
		if(Input.Pressed("Score"))
		{
			GameManager.Instance.SetDebug();
			InputPressed?.Invoke(InputKey.TAB);
			Log.Info("Score Pressed");
		}
		switch(BattleManager.Instance.CurrentBattleState)
		{
			case BattleState.BattleStart:
				if(Input.Pressed("Back"))
				{
					var b = EffectManager.Instance.TrySkipSequence();
					BattleManager.Instance.IntroUI.Deactivate();
					InputPressed?.Invoke(InputKey.BACKSPACE);
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
				if(PlayerMaster.Instance.Mode == FocusMode.FreeLook && (PlayerMaster.Instance.cMode == CommandMode.MoveSelect || PlayerMaster.Instance.cMode == CommandMode.AttackSelect || PlayerMaster.Instance.cMode == CommandMode.AbilitySelect))
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
	}*/
}

public enum InputKey
{
	TAB,
	BACKSPACE,
	SPACE,
	ESC,
	ENTER,
	LEFT,
	RIGHT,
	FORWARD,
	BACKWARD,
	DEBUG,
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
