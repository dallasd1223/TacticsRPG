using Sandbox;
using System;

namespace TacticsRPG;

public class UnitEquipment : Component
{
	public IReadOnlyDictionary<EquipmentSlotType, Equipment> ActiveEquipment => _activeEquipment;
	[Property] [ReadOnly] private Dictionary<EquipmentSlotType, Equipment> _activeEquipment = new Dictionary<EquipmentSlotType, Equipment>();

	public event Action<EquipmentSlotType, Equipment> OnEquip;
	public event Action<EquipmentSlotType, Equipment> OnUnEquip;

	public void Initialize()
	{
		foreach(EquipmentSlotType slot in Enum.GetValues(typeof(EquipmentSlotType)))
		{
			Log.Info(slot);
			_activeEquipment[slot] = null;
		}

		var ar = ResourceLibrary.Get<ArmorData>("resources/Equipment/Armors/ironarmor.armor");
		var arm = new Equipment(ar);
	
		var we = ResourceLibrary.Get<WeaponData>("resources/Equipment/Weapons/bronzesword.equip");
		var wep = new Equipment(we);

		var he = ResourceLibrary.Get<HelmetData>("resources/Equipment/Helmets/ironhelmet.helm");
		var hel = new Equipment(he);

		var ac = ResourceLibrary.Get<AccessoryData>("resources/Equipment/Accessories/ironboots.asc");
		var acc = new Equipment(ac);

		Equip(arm);
		Equip(wep);		
		Equip(hel);	
		Equip(acc);
	}

	public bool Equip(Equipment equipment)
	{
		var slot = equipment.Data.Slot;
		if(SlotIsEmpty(slot))
		{
			_activeEquipment[slot] = equipment;
			OnEquip?.Invoke(slot, equipment);
			return true;
		}
		return false;
	}

	public bool UnEquip(Equipment equipment)
	{
		if(HasEquippment(equipment))
		{
			_activeEquipment[equipment.Data.Slot] = null;
			Log.Info($"{equipment.Data.Name} Has Been UnEquipped From {equipment.Data.Slot}");
			OnUnEquip?.Invoke(equipment.Data.Slot, equipment);
			return true;
		}
		return true;
	}

	public bool SlotIsEmpty(EquipmentSlotType slot)
	{
		foreach(var kvp in _activeEquipment)
		{
			if(kvp.Key == slot)
			{
				bool b = kvp.Value is null;
				return b;
			}
		}
		Log.Info($"Error Finding {slot}");
		return false;
	}

	public bool HasEquippment(Equipment equipment)
	{
		foreach(var kvp in _activeEquipment)
		{
			if(kvp.Value == equipment)
			{
				Log.Info($"{kvp.Key} Has Equipment {equipment.Data.Name} {true}");
				return true;
			}
		}
		Log.Info($"{equipment.Data.Name} Was Not Found In ActiveEquipment");
		return false;
	}

	public T GetEquipmentData<T>(EquipmentSlotType slot) where T : EquipmentData
	{
		foreach(var kvp in _activeEquipment)
		{
			if(kvp.Key == slot)
			{
				if(kvp.Value is null) return null;
				return kvp.Value.Data as T;
			}
		}
		Log.Info($"Error Finding {slot}");
		return null;
	}

	public Equipment GetEquipment(EquipmentSlotType slot)
	{
		foreach(var kvp in _activeEquipment)
		{
			if(kvp.Key == slot)
			{
				return kvp.Value;
			}
		}
		Log.Info($"Error Finding {slot}");
		return null;		
	}

	protected override void OnStart()
	{
		Initialize();
	}
}

public enum EquipmentSlotType
{
	LeftHand,
	RightHand,
	Helmet,
	Armor,
	Accessory
}
