using Sandbox;
using System;
using System.IO;
using System.Text;

namespace TacticsRPG;

[Category("Unit")]
public class UnitStats : Component
{

	[Property] public StatData BaseData {get; set;}
	[Property] [ReadOnly] private Dictionary<StatType, int> Stats {get; set;} = new();

	private UnitEquipment equip;

	protected override void OnAwake()
	{
		equip = GetComponent<UnitEquipment>();
	}

	protected override void OnStart()
	{
		if(BaseData.IsValid())
		{
			LoadBaseStats();
		}

	}

	public int GetStat(StatType type)
	{
		int final = GetFinalValue(type);
		return final;
	}

	public bool SetStat(StatType type, int amount)
	{
		Stats[type] += amount;
		return true;
	}

	public int GetBaseStat(StatType type)
	{
		return Stats[type];		
	}

	public int GetFinalValue(StatType type)
	{
		int basev = Stats[type];

		switch(type)
		{
			case StatType.MAXHP:
				break;
			case StatType.MAXMP:
				break;
			case StatType.MOV:
				break;
			case StatType.ATK:
				break;
			case StatType.DEF:
				break;
			case StatType.MAG:
				break;
			case StatType.SPD:
				break;
			case StatType.ACC:
				break;
			case StatType.EV:
				break;
		}
		return basev;
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
}	
