using Sandbox;
using Sandbox.UI;

namespace Sandbox.UI;

public interface ICharAnimation
{
	void Reset(Panel panel1, Panel panel2 = null);
	void Update(Panel panel1,  float dt, Panel panel2 = null);
	bool IsFinished {get;}
	bool IsStarted {get; set;}
}

