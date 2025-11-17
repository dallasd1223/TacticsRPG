using Sandbox;
using System;
using TacticsRPG;

public class SaveManager : Component
{

	public string SavePath;

	public void SaveGame()
	{

	}

	public void LoadGame()
	{

	}
}

public interface ISaveSystem<T>
{
	T SaveData();
	void LoadData(T data);
}

public class SaveMetadata
{
	public int ID;
	public DateTime Timestamp;
}
