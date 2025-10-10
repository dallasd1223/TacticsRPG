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
						Log.Info("Focus");
						SwitchFocusMode();
						return;
				default:
					break;
			}
		}
		else if(Mode == FocusMode.CommandSelectLook)
		{
			switch(key)
			{
				case InputKey.BACKSPACE:
					CancelCommand();
					return;
			}
		}
		else if(Mode == FocusMode.ConfirmMenu)
		{
			switch(key)
			{
				default:
					break;
			}
		}
		else if(Mode == FocusMode.FreeUnitSelectMenu)
		{
			switch(key)
			{
				case InputKey.ENTER:
					FreeLookScreen.SelectCommand();
					return;
				case InputKey.BACKSPACE:
						Log.Info("Focus");
						SwitchFocusMode();
						return;
				default:
					break;
			}
		}
		else if(Mode == FocusMode.StatusMenu)
		{
			switch(key)
			{
				case InputKey.BACKSPACE:
						Log.Info("Focus");
						LastFocusMode();
					return;
				default:
					break;
			}
		}
		else
		{
			Log.Info("No Focus Mode Set");
			return;
		}
	}
}
