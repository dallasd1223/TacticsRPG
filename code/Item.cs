using Sandbox;
using System;

namespace TacticsRPG;

public class Item: IUsable
{
	public ItemData Data {get; private set;}

	public Item(ItemData data)
	{
		Data = data;
	}

	public void Use(Unit unit)
	{
		switch(Data.ItemEffect)
		{
			case Effect.Heal:
				if(unit.Data.CurrentHP + Data.Value >= unit.Data.MaxHP)
				{
					unit.Data.CurrentHP = unit.Data.MaxHP;
				}
				else
				{
					unit.Data.CurrentHP += Data.Value;
				}
				break;
			case Effect.Hurt:
				break;
			case Effect.Revive:
				break;
		}
	}
}

[GameResource("Item", "item", "Defines Item Data")]
public class ItemData : GameResource
{
	public string Name {get; set;}
	public string Description {get; set;}
	public Effect ItemEffect {get; set;}
	public int EffectID {get; set;}
	public int MaxStack {get; set;}
	public int Value {get; set;}

	// Access these statically with Clothing.All
	public static IReadOnlyList<ItemData> All => _all;
	internal static List<ItemData> _all = new();

	protected override void PostLoad()
	{
		base.PostLoad();

        // Since you are constructing the list yourself, you could add your own logic here
        // to create lists for All, AllHats, AllShirts, AllShoes, ect.
		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}

public interface IUsable
{
	public void Use(Unit unit)
	{

	}
}

public enum Effect
{
	Heal,
	Hurt,
	Revive,
	Item,
}


