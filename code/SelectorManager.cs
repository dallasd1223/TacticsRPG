using Sandbox;
using System;

namespace TacticsRPG;

public partial class SelectorManager : StateMachine
{

	public SelectCursor Cursor;

	private bool IsActive = false;

	public event Action HoverChange;
	[Property] public Vector2 HoveredVector {get; set;} = new Vector2(0,0);
	public event Action<Vector2> VectorChange;
	[Property] public TileData HoveredTile {get; set;}
	public event Action<TileData> TileChange;
	[Property] public Unit HoveredUnit {get; set;}
	public event Action<Unit> UnitChange;

	public event Action ValidSelect;

	//So We Can Turn Off Changes
	public Vector2 LastVector;
	public TileData LastTile;
	public Unit LastUnit;

	[Property] public SelectorState State {get; set;}

	[Property] public GameObject CursorPrefab {get; set;}

	public void TrySetAll(Vector2 vec)
	{
		TrySetVector(vec);
		TrySetTile(HoveredVector);
		TrySetUnit(HoveredTile);

		//Send Event To Current SelectorState
		HoverChange?.Invoke();
	}
	public void TrySetVector(Vector2 vec)
	{
		Vector2 NewVec = HoveredVector + vec;
		if(NewVec.x < 0 || NewVec.x > TileMapManager.Instance.MaxX) return;
		if(NewVec.y < 0 || NewVec.y > TileMapManager.Instance.MaxY) return;
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

	public void OnValidSelect()
	{
		ValidSelect?.Invoke();
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
}

public class AttackSelectState : SelectorState
{
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

}

public class AbilitySelectState : SelectorState
{
	[Property] public IAbilityItem CurrentAbilityItem {get; set;}

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

	protected override bool CheckIfSelectable()
	{
		var t = Selector.HoveredTile;
		var u = UnitManager.Instance.GetUnitFromTile(t);
		var a = CurrentAbilityItem;

		if(a is not null)
		{
			bool selfOk = a.Data.CanUseOnSelf;

			if(t.selectable && u is not null)
			{
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
}

public class WaitSelectState : SelectorState {}

public class SelectCursor : Component
{
	[Property] SelectorManager Selector {get; set;}

	[Property] GameObject CursorObject {get; set;}
	[Property] GameObject BorderObject {get; set;}
	
	[Property] public Vector3 TargetPosition {get; set;}
	[Property] public Angles CAngles {get; set;}
	[Property] public float RotateSpeed {get; set;}

	protected override void OnAwake()
	{
		Selector.VectorChange += HandleVector;
	}

	protected override void OnUpdate()
	{
		GameObject.WorldPosition = TargetPosition + new Vector3(0,0,(float)Math.Sin(Time.Now * 3) * 5f);
		CAngles = CAngles + new Angles(0,RotateSpeed,0);
		GameObject.WorldRotation = CAngles;		
	}

	private void HandleVector(Vector2 vec)
	{
		SetPosition(vec);
	}

	private void SetPosition(Vector2 vec)
	{
		TargetPosition = GetCursorPositionFromVector(vec);
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
