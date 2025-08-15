using Sandbox;
using System;

namespace TacticsRPG;
public sealed class GameCursor : Component
{
	public static GameCursor Instance {get; set;}
	[Property] public bool IsActive {get; set;} = true;
	[Property] public Vector3 TargetPosition {get; set;}
	[Property] public Angles CAngles {get; set;} = new Angles(0,0,180);
	[Property] public float RotateSpeed {get; set;}
	[Property] public ModelRenderer Model {get; set;}
	[Property] public Vector2 SelectedVector {get; set;} = new Vector2(0,0);
	[Property] public GameObject SelectedObject {get; set;} = null;
	[Property] public TileData SelectedTile {get; set;}
	[Property] public Unit SelectedUnit {get; set;}
	[Property] public bool HasUnit {get; set;} = false;
	[Property] public GameObject Border {get; set;}
	[Property] public SoundEvent MoveSelect {get; set;}
	[Property] public SoundEvent Error {get; set;}

	protected override void OnAwake()
	{
		Instance = this;
		GetComponent<ModelRenderer>();
		TargetPosition = GameObject.WorldPosition;
		Log.Info($"{SelectedVector}");
	}

	protected override void OnStart()
	{

	}

	protected override void OnUpdate()
	{

		if(!IsActive) return;

		//Cursor Animation
		GameObject.WorldPosition = TargetPosition + new Vector3(0,0,(float)Math.Sin(Time.Now * 3) * 5f);
		CAngles = CAngles + new Angles(0,RotateSpeed,0);
		GameObject.WorldRotation = CAngles;

		if(PlayerMaster.Instance.Mode == FocusMode.ConfirmMenu) return;


		if(Input.Pressed("Forward"))
		{
			if(SelectedVector.y == 15)
			{
				Log.Info("Out Of Range");
				return;				
			}
			SelectedVector += new Vector2(0,1);
			SetTargetPosition();
			SetSelectedObject();
			CheckIfSelectable(SelectedTile);
			Sound.Play(MoveSelect);
		}
		if(Input.Pressed("Backward"))
		{
			if(SelectedVector.y == 0)
			{
				Log.Info("Out Of Range");
				return;
			}
			SelectedVector -= new Vector2(0,1);
			SetTargetPosition();
			SetSelectedObject();
			CheckIfSelectable(SelectedTile);
			Sound.Play(MoveSelect);
		}
		if(Input.Pressed("Left"))
		{
			if(SelectedVector.x == 0)
			{
				Log.Info("Out Of Range");
				return;
			}
			SelectedVector -= new Vector2(1,0);
			SetTargetPosition();
			SetSelectedObject();
			CheckIfSelectable(SelectedTile);
			Sound.Play(MoveSelect);
		}
		if(Input.Pressed("Right"))
		{
			if(SelectedVector.x == 8)
			{
				Log.Info("Out Of Range");
				return;				
			}
			SelectedVector += new Vector2(1,0);
			SetTargetPosition();
			SetSelectedObject();
			CheckIfSelectable(SelectedTile);
			Sound.Play(MoveSelect);
		}
	}

	public void SetTargetPosition()
	{
		TargetPosition = GetPositionFromVector(SelectedVector);
		
	}
	public void PreCheck()
	{
		switch(PlayerMaster.Instance.CurrentSelectedCommand)
		{
			case CommandType.Move:
				if(CheckIfSelectable(SelectedTile))
				{
					PlayerMaster.Instance.ActivateConfirmMenu();
					return;
				}
				else
				{
					Sound.Play(Error);
					Log.Info("Tile Not Selectable");
					return;
				}
			case CommandType.Attack:
				if(CheckIfSelectable(SelectedTile))
				{
					if(TileHasUnit(SelectedTile))
					{
						PlayerMaster.Instance.ActivateConfirmMenu();
						return;
					}
					else
					{
						Sound.Play(Error);
						Log.Info("No Attackable Unit On Tile");
						return;
					}
					return;
				}
				else
				{
					Sound.Play(Error);
					Log.Info("Tile Not Selectable");
					return;
				}
				break;
			case CommandType.Wait:
				PlayerMaster.Instance.ActivateConfirmMenu();
				break;
		}
	}

	public void SendConfirmData()
	{
		if(!CheckIfSelectable(SelectedTile))
		{
			Sound.Play(Error);
			return;
		}
		PlayerMaster.Instance.ConfirmCommand(BattleManager.Instance.ActiveUnit, SelectedTile);
	}

	public bool CheckIfSelectable(TileData data)
	{
		if(PlayerMaster.Instance.Mode == FocusMode.FreeLook && (PlayerMaster.Instance.cMode == CommandMode.MoveSelect || PlayerMaster.Instance.cMode == CommandMode.AttackSelect))
		{
			var tile = SelectedObject.GetComponent<TileData>();
			if(tile.current == false && tile.selectable == true)
			{
				Log.Info("Tile Selectable");
				return true;
			}
			else
			{
				Log.Info("Tile Unselectable");
				return false;
			}
		}
		return false;
	}
	
	public void SetSelectedObject()
	{
		TileData obj = TileMapManager.Instance.GetTileData(SelectedVector);
		if(obj is not null)
		{
			Log.Info($"{obj.GameObject.Name} Selected");
			SelectedTile = obj;
			SelectedObject = obj.GameObject;
			CameraManager.Instance.CursorObjectFocus(SelectedObject);
			Border.WorldPosition = SelectedObject.WorldPosition + new Vector3(0,0,1);
			if(TileHasUnit(obj))
			{
				SelectedUnit = GetUnitOnTile(obj);
			}
			else
			{
				SelectedUnit = null;
			}
			return;
		}
		Log.Info("No Object Found");
	}

	public bool TileHasUnit(TileData tile)
	{
		Log.Info("Checking Units");
		var units = Scene.GetAll<Unit>();
		foreach(Unit unit in units)
		{
			Log.Info("Checking Units");
			if(!unit.Move.IsValid()) break;
			if(unit.Move.UnitTile == tile)
			{
				return true;
			}
		}
		return false;
	}

	public Unit GetUnitOnTile(TileData tile)
	{
		var units = Scene.GetAll<Unit>();
		foreach(Unit unit in units)
		{
			if(unit.Move.UnitTile == tile)
			{
				return unit;
			}
		}
		return null;
	}

	public Vector3 GetPositionFromVector(Vector2 vector)
	{
		TileData tile = TileMapManager.Instance.GetTileData(vector);
		if(tile.IsValid())
		{
			int height = 0;
			if(tile.HeightIndex > 1)
			{
				height = (tile.HeightIndex - 1) * 28;
			}
			return tile.TilePosition + new Vector3(0,0,155 + height);
		}
		Log.Info("No TileFound");
		return this.GameObject.WorldPosition;
	}

	public void ActiveToggle()
	{
		if(IsActive)
		{
			DeactivateCursor();
		}
		else
		{
			ActivateCursor();
		}
	}

	public void ActivateCursor()
	{
		IsActive = true;
		Model.Enabled = true;
		Border.Enabled = true;
		CameraManager.Instance.ResetToActive();
		ResetCursor();
		Sound.Play(MoveSelect);
	}

	public void DeactivateCursor()
	{
		IsActive = false;
		Model.Enabled = false;
		Border.Enabled = false;
		CameraManager.Instance.ResetToActive();

	}

	public void ResetCursor()
	{
		SelectedVector = TileMapManager.Instance.GetVector2FromTile(BattleManager.Instance.ActiveUnit.Move.UnitTile);
		SelectedTile = TileMapManager.Instance.GetTileFromVector2(SelectedVector);
		SelectedUnit = UnitManager.Instance.GetUnitFromTile(SelectedTile);
		SetSelectedObject();
		SetTargetPosition();
	}
}
