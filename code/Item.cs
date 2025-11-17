using Sandbox;
using System;
using SpriteTools;

namespace TacticsRPG;

public class Item: IUsable, IInventoryItem
{
	public ItemData Data {get; set;}

	public Item(ItemData data)
	{
		Data = data;
	}

	/*public void Use(Unit unit)
	{
		switch(Data.ItemEffect)
		{
			case Effect.Heal:
				if(unit.Stats.CurrentHP + Data.Value >= unit.Stats.MaxHP)
				{
					unit.Stats.CurrentHP = unit.Stats.MaxHP;
				}
				else
				{
					unit.Stats.CurrentHP += Data.Value;
				}
				break;
			case Effect.Hurt:
				break;
			case Effect.Revive:
				break;
		}
	}*/
}

[GameResource("Item", "item", "Defines Item Data")]
public class ItemData : GameResource
{
	public string Name {get; set;}
	public string Description {get; set;}
	public SpriteResource Sprite {get; set;}

	public int Price {get; set;}

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
	public void Use(BattleUnit unit)
	{

	}
}

public interface IInventoryItem
{

}
