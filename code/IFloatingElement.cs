using Sandbox;

public interface IFloatingElement
{
	void Start();
	void Hide();
	ElementPlayType Type {get; set;}
	bool ReadyToDelete {get; set;}
}

public enum ElementPlayType
{
	Once,
	Loop,
}
