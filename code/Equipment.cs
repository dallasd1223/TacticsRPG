using Sandbox;
using System;

namespace TacticsRPG;

public class Equipment: IInventoryItem
{
	public EquipmentData Data {get; set;}

	public Equipment(EquipmentData data)
	{
		Data = data;
	}
}

public enum EquipmentType
{
	Weapon,
	Armor,
	Helmet,
	Accessory,
}

public class EquipmentData : GameResource
{
	public virtual string Name {get; set;}
	public virtual string Description {get; set;}
	public virtual string IconPath {get; set;}

	public virtual EquipmentSlotType Slot {get; set;}
	public virtual EquipmentType EquipType {get; set;}

	public virtual int Price {get; set;}
}

[GameResource("Weapon", "equip", "Defines Weapon Data")]
public class WeaponData : EquipmentData
{
	public override string Name {get; set;}
	public override string Description {get; set;}
	public override string IconPath {get; set;}

	public override EquipmentSlotType Slot {get; set;}
	public EquipmentSlotType SecondarySlot {get; set;}
	public override EquipmentType EquipType {get; set;}	

	public override int Price {get; set;}

	public WeaponType Type {get; set;}

	public int WP {get; set;} //Weapon Power

	public int Ev {get; set;} //Weapon Evasion

	public List<StatusEffectData> StatusEffects {get; set;} = new(); //Applied Status Effects


}

public enum WeaponType
{
	Sword,
	Shield,
	Dagger,
	Staff,
	Bow
}

[GameResource("Armor", "armor", "Defines Armor Data")]
public class ArmorData : EquipmentData
{
	public override string Name {get; set;}
	public override string Description {get; set;}
	public override string IconPath {get; set;}

	public override EquipmentSlotType Slot {get; set;}

	public override EquipmentType EquipType {get; set;}	
	
	public override int Price {get; set;}

	public ArmorType Type {get; set;}

	public int HP {get; set;} 

	public int MP {get; set;}
}

public enum ArmorType
{
	Armor,
	Clothes,
	Robe,
}

[GameResource("Helmet", "helm", "Defines Helmet Data")]
public class HelmetData : EquipmentData
{
	public override string Name {get; set;}
	public override string Description {get; set;}
	public override string IconPath {get; set;}

	public override EquipmentSlotType Slot {get; set;}
	public override EquipmentType EquipType {get; set;}	
	
	public override int Price {get; set;}

	public HelmetType Type {get; set;}

	public int HP {get; set;} 

	public int MP {get; set;}
}

public enum HelmetType
{
	Helmet,
	Hat,
}

[GameResource("Accessory", "asc", "Defines Accessory Data")]
public class AccessoryData : EquipmentData
{
	public override string Name {get; set;}
	public override string Description {get; set;}
	public override string IconPath {get; set;}

	public override EquipmentSlotType Slot {get; set;}
	public override EquipmentType EquipType {get; set;}	
	
	public override int Price {get; set;}

	public AccessoryType Type {get; set;}

	public List<StatusEffectData> StatusEffects {get; set;} = new();
}

public enum AccessoryType
{
	Shoes,
	Rings,
	Amulet,
}
