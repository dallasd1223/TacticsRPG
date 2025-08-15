using Sandbox;
namespace TacticsRPG;
public static class ActionLog
{
	private static List<ActionLogEntry> _entries = new();

	public static IReadOnlyList<ActionLogEntry> Entries => _entries;

	public static void Clear(){}

	public static void Add(string type, Unit source, Unit? target, string desc, object? metadata = null)
	{
		_entries.Add(new ActionLogEntry
		{
			ActionType = type,
			Source = source,
			Target = target,
			Description = desc,
			Metadata = metadata,
			Timestamp = Time.Now,
		});
	}

	public static void PrintToConsole()
	{
		foreach(var entry in _entries)
		{
			Log.Info(entry.ToString());
		}
	}

}

public class ActionLogEntry
{
	public string ActionType;
	public Unit Source;
	public Unit? Target;
	public string Description;
	public float Timestamp;
	public object? Metadata;

	public override string ToString()
	{
		return $"[{Timestamp:0.00}] {Description}";
	}
}

