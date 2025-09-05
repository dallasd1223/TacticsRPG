using Sandbox;
using System;
using SpriteTools;

namespace TacticsRPG;

public partial class SelectorManager : StateMachine
{

	public SelectCursor Cursor;

	private bool IsActive = false;
	//Guard Past Input Capture
	private bool JustTransitioned = false;
	[Property] public bool IsConfirming = false;

	[Property] public Unit CurrentUnit {get; set;}

	[Property] public Vector2 HoveredVector {get; set;} = new Vector2(0,0);
	[Property] public TileData HoveredTile {get; set;}
	[Property] public Unit HoveredUnit {get; set;}
	[Property] public GameObject HoveredObject {get; set;}

	public event Action HoverChange;
	public event Action<Vector2> VectorChange;
	public event Action<TileData> TileChange;
	public event Action<Unit> UnitChange;

	public event Action<SelectorState> ValidSelect;
	public event Action Deactivate;

	//So We Can Turn Off Changes
	public Vector2 LastVector;
	public TileData LastTile;
	public Unit LastUnit;

	[Property] public SelectorState State {get; set;}

	[Property] public GameObject CursorPrefab {get; set;}
	[Property] public GameObject ActiveCursorObject {get; set;}
	[Property] public SelectCursor ActiveCursor {get; set;}

	public void TrySetAll(Vector2 vec, bool fresh)
	{
		TrySetVector(vec, fresh);
		TrySetTile(HoveredVector);
		TrySetUnit(HoveredTile);
		TrySetObject(HoveredTile);
		//Send Event To Current SelectorState
		HoverChange?.Invoke();
	}

	public void TrySetVector(Vector2 vec, bool fresh)
	{	
		Vector2 NewVec;
		if(fresh)
		{
			HoveredVector = vec;
			VectorChange?.Invoke(HoveredVector);
			return;
		}
		NewVec = HoveredVector + vec;
		if(NewVec.x < 0 || NewVec.x > TileMapManager.Instance.MaxX) return;
		if(NewVec.y < 0 || NewVec.y > TileMapManager.Instance.MaxY) return;
		Log.Info("Why");
		LastVector = HoveredVector;
		HoveredVector = NewVec;
		VectorChange?.Invoke(HoveredVector);	
	}

	public void TrySetTile(Vector2 vec)
	{
		TileData tile = TileMapManager.Instance.GetTileFromVector2(vec);
		if(tile is not null)
		{	
			if(HoveredTile is not null)
			{
				LastTile = HoveredTile;
			}
			HoveredTile = tile;
			TileChange?.Invoke(HoveredTile);
			return;
		}
		else
		{
			HoveredTile = null;	
			return;
		}
	}

	public void TrySetObject(TileData t)
	{
		if(!t.IsValid()) return;
		HoveredObject = t.GameObject;
	}

	public void TrySelect()
	{
		if(State is not null)
		{
			State.OnSelect();
		}
	}
	public void OnValidSelect()
	{
		ValidSelect?.Invoke(State);
		PlayerEvents.OnValidSelection(State);
		IsConfirming = true;
	}

	public void CancelSelection()
	{
		if(!IsConfirming) return;
		if(State is not null)
		{
			State.OnCancel();
			IsConfirming = false;
		}
	}

	public void TrySetUnit(TileData data)
	{
		if(data is null) return;
		Unit unit = UnitManager.Instance.GetUnitFromTile(data);
		if(unit is not null)
		{
			if(HoveredUnit is not null)
			{
				LastUnit = HoveredUnit;
			}
			HoveredUnit = unit;
			UnitChange?.Invoke(HoveredUnit);
			return;
		}
		else 
		{
			HoveredUnit = null;
			return;
		}
	}
}

public class SelectorState : State
{

	public SelectorManager Selector;

	[Property] public bool IsSelectable {get; set;} = false;

	protected override void OnAwake()
	{
		Selector = GetComponent<SelectorManager>();
	}

	protected override void AddListeners()
	{
		Selector.HoverChange += HandleHoverChange;
	}

	protected override void RemoveListeners()
	{
		Selector.HoverChange -= HandleHoverChange;
	}

	public override void Enter()
	{
		base.Enter();
		Selector.BuildCursor();
	}

	public override void Exit()
	{
		Selector.DestroyCursor();
		base.Exit();
	}

	public void HandleHoverChange()
	{
		OffHover();
		OnHover();
		CheckIfSelectable();
	}

	public virtual void OnHover() {}

	public virtual void OffHover() {}

	protected virtual bool CheckIfSelectable()
	{
		IsSelectable = true;
		return true;
	}

	public virtual void OnSelect()
	{
		if(!IsSelectable) return;
		Selector.OnValidSelect();
	}

	public virtual void ConfirmSelection()
	{
		Selector.DeactivateSelector();
	}

	public virtual void OnCancel()
	{

	}

	public virtual void OnDeactivate()
	{

	}

	public SelectorState() {}
}

public class FreeLookSelectState : SelectorState
{

	protected override bool CheckIfSelectable()
	{

		IsSelectable = false;
		return false;
	}

}
public class MoveSelectState : SelectorState
{
	[Property] TileData SelectedTile {get; set;}

	protected override bool CheckIfSelectable()
	{
		var t = Selector.HoveredTile;
		if(t.selectable == true && t.current == false && t.IsOccupied == false)
		{
			IsSelectable = true;
			Log.Info($"Tile {t.TileIndex} Is Selectable");
			return true;
		}
		else
		{
			IsSelectable = false;
			Log.Info($"Tile {t.TileIndex} Is Not Selectable");
			return false;
		}
	}

	public override void OnSelect()
	{
		//Might be redundant
		if(IsSelectable)
		{
			SelectedTile = Selector.HoveredTile;
		}

		base.OnSelect();
	}

	public override void OnCancel()
	{
		SelectedTile = null;
	}

	public override void OnDeactivate()
	{
		SelectedTile = null;
		IsSelectable = false;
	}
	public override void ConfirmSelection()
	{
		Command command = new MoveCommand(Selector.CurrentUnit, SelectedTile);
		PlayerEvents.OnConfirmAction(command);
		OnDeactivate();
		base.ConfirmSelection();
	}

}

public class AttackSelectState : SelectorState
{
	[Property] TileData SelectedTile {get; set;}
	[Property] Unit SelectedUnit {get; set;}

	protected override bool CheckIfSelectable()
	{
		var t = Selector.HoveredTile;
		var u = UnitManager.Instance.GetUnitFromTile(t);

		if(!t.current && t.IsOccupied && t.selectable && u is not null)
		{
			IsSelectable = true;
			Log.Info($"Unit {u.Data.Name} On Tile {t.TileIndex} Is Selectable");
			return true;
		}
		Log.Info("Unit or Tile Not Selectalbe");
		return false;
	}

	public override void OnSelect()
	{
		//Might be redundant
		if(IsSelectable)
		{
			SelectedTile = Selector.HoveredTile;
			SelectedUnit = UnitManager.Instance.GetUnitFromTile(SelectedTile);
		}

		base.OnSelect();
	}

	public override void ConfirmSelection()
	{
		Selector.CurrentUnit.Attack.EndAttackSelection();
		
		Command command = new AttackCommand(Selector.CurrentUnit, SelectedUnit);
		PlayerEvents.OnConfirmAction(command);

		OnDeactivate();
		base.ConfirmSelection();
	}
}

public class AbilitySelectState : SelectorState
{
	[Property] public IAbilityItem CurrentAbilityItem {get; set;}
	[Property] public TileData SelectedTile {get; set;}
	[Property] public List<TileData> SelectedTiles {get; set;} = new();
	[Property] public Unit SelectedUnit {get; set;}

	public void SetAbilityItem(IAbilityItem AItem)
	{
		CurrentAbilityItem = AItem;
		CheckIfSelectable();
	} 

	public void RemoveAbilityItem()
	{
		CurrentAbilityItem = null;
	}

	protected override void AddListeners()
	{
		base.AddListeners();
		PlayerEvents.AbilityItemSelected += SetAbilityItem;
	}

	protected override void RemoveListeners()
	{
		base.RemoveListeners();
		PlayerEvents.AbilityItemSelected -= SetAbilityItem;		
	}

	public override void Exit()
	{
		RemoveAbilityItem();
		base.Exit();
	}
	public override void OnSelect()
	{
		if(IsSelectable)
		{
			Selector.CurrentUnit.Ability.MoveTempToFinalTiles();

			SelectedTile = Selector.HoveredTile;
			SelectedUnit = UnitManager.Instance.GetUnitFromTile(SelectedTile);
			SelectedTiles = Selector.CurrentUnit.Ability.FinalTiles;

		
			base.OnSelect();
		}
	}
	public override void OnCancel()
	{
		SelectedTile = null;
	}

	public override void OnDeactivate()
	{
		SelectedTile = null;
		SelectedUnit = null;
		SelectedTiles = null;
		IsSelectable = false;
	}
	protected override bool CheckIfSelectable()
	{
		var t = Selector.HoveredTile;
		var u = UnitManager.Instance.GetUnitFromTile(t);
		var a = CurrentAbilityItem;

		var item = CurrentAbilityItem;

		if(a is not null)
		{
			bool selfOk = a.Data.CanUseOnSelf;
			Selector.CurrentUnit.Ability.ForceLastTileHighlight();
			if(t.selectable && u is not null)
			{
				Selector.CurrentUnit.Ability.SetTempFinalTiles(t, new AOEData(item.Data.Shape, item.Data.ActionRange, item.Data.CanUseOnSelf));
				IsSelectable = true;
				return true;
			}
			else if(!t.current && t.selectable && u is not null)
			{
				IsSelectable = true;
				return true;			
			}	
			else
			{
				IsSelectable = false;
				return false;
			}
		}
	
		return false;
	}

	public override void ConfirmSelection()
	{
		Selector.CurrentUnit.Ability.EndAbilitySelect();	
		Command command = new AbilityCommand(CurrentAbilityItem, Selector.CurrentUnit, SelectedUnit, SelectedTiles);
		PlayerEvents.OnConfirmAction(command);
		Selector.CurrentUnit.Ability.ResetRemoveFinalTiles();
		OnDeactivate();
		base.ConfirmSelection();
	}
}

public class WaitSelectState : SelectorState
{
	private bool WaitFirstFrameBeforeSelect = false;
	public override void Enter()
	{
		base.Enter();
		IsSelectable = true;
	}

	protected override void OnUpdate()
	{
		if(WaitFirstFrameBeforeSelect) return;
		OnSelect();
		WaitFirstFrameBeforeSelect = true;
	}

	public override void ConfirmSelection()
	{
		Command command = new WaitCommand(Selector.CurrentUnit);
		PlayerEvents.OnConfirmAction(command);
		base.ConfirmSelection();		
	}
}

public class SelectCursor : Component
{
	[Property] public SelectorManager Selector {get; set;}

	private bool OnFirstRender = true;

	[Property] GameObject CursorObject {get; set;}
	[Property] ModelRenderer CursorModel {get; set;}
	[Property] GameObject BorderObject {get; set;}
	[Property] SpriteComponent BorderSprite {get; set;}

	private Vector3 LastPosition;
	[Property] public Vector3 TargetPosition {get; set;}
	[Property] public Angles CAngles {get; set;} = new Angles(0,0,180);
	[Property] public float RotateSpeed {get; set;} = 2f;

	[Property] SoundEvent NewPositionSound {get; set;}
	
	public void Activate()
	{
		SetListeners();
		HandleVector(Selector.HoveredVector);
	}
	public void SetListeners()
	{
		Selector.VectorChange += HandleVector;
		Selector.Deactivate += HandleDeactivate;
	}
	protected override void OnUpdate()
	{

		CursorObject.WorldPosition = TargetPosition + new Vector3(0,0,(float)Math.Sin(Time.Now * 3) * 5f);
		CAngles = CAngles + new Angles(0,RotateSpeed,0);
		CursorObject.WorldRotation = CAngles;	


		//Prevents Weird Visual PopIn 
		if(OnFirstRender)
		{
			EnableVisual();
		}	
	}

	public void HandleDeactivate()
	{
		//this.GameObject.Destroy();
	}
	private void HandleVector(Vector2 vec)
	{
		SetCursorPosition(vec);
		SetBorderPosition(vec);
		SetCameraFocus(vec);
		TryPlaySound();
	}

	private void EnableVisual()
	{
		CursorModel.Enabled = true;
		BorderSprite.Enabled = true;

		OnFirstRender = false;
	}

	private void TryPlaySound()
	{
		if(LastPosition != TargetPosition )
		{	
			Sound.Play(NewPositionSound);
		}
	}

	private void SetCameraFocus(Vector2 vec)
	{
		TileData t = TileMapManager.Instance.GetTileFromVector2(vec);
		if(t is null) return;
		GameObject obj = t.GameObject;
		CameraManager.Instance.CursorObjectFocus(obj);
	}

	private void SetCursorPosition(Vector2 vec)
	{
		LastPosition = TargetPosition;
		TargetPosition = GetCursorPositionFromVector(vec);
	}
	private void SetBorderPosition(Vector2 vec)
	{
		TileData tile = TileMapManager.Instance.GetTileData(vec);
		if(tile.IsValid())
		{
			BorderObject.WorldPosition = tile.GameObject.WorldPosition + new Vector3(0,0,1);
		}

	}
	public Vector3 GetCursorPositionFromVector(Vector2 vector)
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
}
