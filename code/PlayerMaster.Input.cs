using Sandbox;
using System;

namespace TacticsRPG;
partial class PlayerMaster 
{
	public void HandleInput(InputKey key)
	{
		if(Mode == FocusMode.Menu)
		{
			switch(key)
			{
				case InputKey.FORWARD:
					Menu.DecreaseIndex();
					break;
				case InputKey.BACKWARD:
					Menu.IncreaseIndex();
					break;
				case InputKey.BACKSPACE:
					if(Menu.MenuLevel > 0)
					{
						Menu.LastMenuState();
						return;
					}
					else
					{
						Log.Info("Focus");
						SwitchFocusMode();
						return;
					}

					break;
				case InputKey.ENTER:
					Menu.SelectItem();
					break;
			}
		}
		else if(Mode == FocusMode.FreeLook)
		{
			switch(key)
			{
				case InputKey.BACKSPACE:
					if(cMode == CommandMode.NA)
					{
						Log.Info("Focus");
						SwitchFocusMode();
						return;
					}
					else
					{
						CancelCommand();
						return;
					}
				default:
					break;
			}

		}
		else if(Mode == FocusMode.ConfirmMenu)
		{
			switch(key)
			{
				case InputKey.BACKSPACE:
					Cancel();
					break;
				case InputKey.ENTER:
					ConfirmFinish();
					break;
				default:
					break;
			}
		}
	}
}
