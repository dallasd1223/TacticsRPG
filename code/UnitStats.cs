using Sandbox;
using System;
using System.IO;
using System.Text;
public sealed class UnitStats : Component
{
	[Property] private int _level = 1;
	public int Level
	{
		get
		{
			return _level;
		}
		set
		{
			TryChangeStat(StatType.LVL, value);
		}
	 }
	[Property] public StatData BaseData {get; set;}
	[Property] [ReadOnly] private Dictionary<StatType, int> Stats {get; set;} = new();


	[Property] [ReadOnly] public int EXP {get; set;} = 0;
	[Property] public int MaxHP {get; set;} = 20;
	[Property] public int CurrentHP {get; set;} = 20;
	[Property] public int MaxMP {get; set;} = 10;
	[Property] public int CurrentMP {get; set;} = 10;
	[Property] public int ChargeTime {get; set;} = 5;
	[Property] public int Speed {get; set;} = 3;
	[Property] public int Accuracy {get; set;} = 90;
	[Property] public int Strength {get; set;} = 15;
	[Property] public int Evasion {get; set;} = 5;

	public event Func<StatObject, StatObject> StatHasChanged;

	protected override void OnStart()
	{
		if(BaseData.IsValid())
		{
			LoadBaseStats();
		}

	}
	public void LoadBaseStats()
	{
		if(!BaseData.IsValid()) return;

		foreach(KeyValuePair<StatType, int> entry in BaseData.Stats)
		{
			Stats.Add(entry.Key, entry.Value);
		}
	}

	[Button("Save Stats")]
	public void SaveStatsToFile()
	{
		if(!FileSystem.Data.DirectoryExists("Stats"))
		{
			FileSystem.Data.CreateDirectory("Stats");
		}
		string filepath = "Stats/unitstats.txt";
		
		List<string> lines = new List<string>();
		foreach(KeyValuePair<StatType, int> entry in Stats)
		{
			lines.Add($"{entry.Key}:{entry.Value}");
		}
		string content = string.Join("\n", lines);
		FileSystem.Data.WriteAllText(filepath, content);
		
		Log.Info("File Saved");
	}

	[Button("Save Binary")]
	public void StatStatsToBinary()
	{
		if(!FileSystem.Data.DirectoryExists("Stats"))
		{
			FileSystem.Data.CreateDirectory("Stats");
		}

		string filepath = "Stats/unitstats.sav";

		using (var stream = FileSystem.Data.OpenWrite(filepath))
		using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false))
		{

			writer.Write(Stats.Count);

			foreach(var entry in Stats)
			{
				writer.Write((int)entry.Key);
				writer.Write(entry.Value);
			}
		}

		Log.Info("Binary saved");
	}

	[Button("LoadBinary")]
	public void LoadBinarySAV()
	{
		string filepath = "Stats/unitstats.sav";

		if(!FileSystem.Data.FileExists(filepath)) return;
		
		using (var stream = FileSystem.Data.OpenRead(filepath))
		using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: false))
		{
			int count = reader.ReadInt32();

			for (int i = 0; i < count; i++)
			{
				StatType key = (StatType)reader.ReadInt32();
				int value = reader.ReadInt32();
				Log.Info($"{key}: {value}");
			}
		}
	}
	public bool TryChangeStat(StatType type, int value)
	{
		switch(type)
		{
			case StatType.LVL:
				_level = value;
				StatHasChanged?.Invoke(new StatObject(type, value));
				return true;
				break;
			case StatType.EXP:
				StatHasChanged?.Invoke(new StatObject(type, value));
				return true;
				break;
			case StatType.MAXHP:
				StatHasChanged?.Invoke(new StatObject(type, value));
				return true;
				break;
			case StatType.HP:
				StatHasChanged?.Invoke(new StatObject(type, value));
				return true;
				break;
			case StatType.MAXMP:
				StatHasChanged?.Invoke(new StatObject(type, value));
				return true;
				break;
			case StatType.MP:
				StatHasChanged?.Invoke(new StatObject(type, value));
				return true;
				break;
		}
		return false;
	}
}	

public class StatObject
{
	StatType Type;
	int Value;

	public StatObject(StatType t, int v)
	{
		Type = t;
		Value = v;
	}
}

public enum StatType
{
	LVL, // LEVEL
	EXP, // EXPERIENCE POINTS
	MAXHP, // MAX HEALTH POINTS
	HP,	// HEALTH POINTS
	MAXMP, // MAX MANA POINTS
	MP, // MANA POINTS
	CTR, //CHARGE TIME
	MOV, // MOVE RANGE
	ATK, // ATTACK
	MAG, // MAGIC
	DEF, // DEFENSE
	SPD, // SPEED
	ACC, // ACCURACY
	EV, // EVASION
}

[GameResource("Stat", "stat", "Defines Unit/Job Stats")]
public class StatData : GameResource
{
	public Dictionary<StatType, int> Stats {get; set;}
}
