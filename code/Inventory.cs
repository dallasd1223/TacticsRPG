using Sandbox;
using System;

namespace TacticsRPG;

public class InventoryManager : Component
{
	private List<InventorySlot> _items = new();
	[Property] public IReadOnlyList<InventorySlot> Items => _items;

	protected override void OnStart()
	{
		var potionData = ResourceLibrary.Get<ItemData>( "Items/Potion.Item" );
		var potion = new Item(potionData);
		AddItem(potion, 1);
	}

	public void AddItem(Item item, int amount)
	{
		var slot = _items.FirstOrDefault(s => s.SlotItem == item);
		if(slot != null)
		{
			slot.Add(amount);
		}
		else
		{
			_items.Add(new InventorySlot(item, amount));
			Log.Info($"{amount} {item.Data.Name} Added To Player Inventory");
		}
	}
	
	public bool RemoveItem(Item item)
	{
		var slot = _items.FirstOrDefault(s => s.SlotItem == item);
		if(slot != null)
		{
			_items.Remove(slot);
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool UseItem(Item item, int amount)
	{
		var slot = _items.FirstOrDefault(s => s.SlotItem == item);
		if (slot == null || !slot.Remove(amount))
			return false;
		if (slot.Quantity <= 0)
		{
			_items.Remove(slot);
		}
		return true;
	}
}

public class InventorySlot
{
	public Item SlotItem {get; private set;}
	public int Quantity {get; private set;}

	public InventorySlot(Item item, int quantity)
	{
		SlotItem = item;
		Quantity = quantity;
	}

	public void Add(int amount) => Quantity += amount;
	public bool Remove(int amount)
	{
		if (Quantity < amount) return false;
		Quantity -= amount;
		return true;
	}
}
