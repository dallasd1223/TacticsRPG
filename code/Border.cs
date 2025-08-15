using Sandbox;

public sealed class Border : Component
{
	public TimeSince AnimTick = 0f;
	public bool Shrunk = false;

	protected override void OnUpdate()
	{
		if(AnimTick >= 0.5f)
		{
			if(!Shrunk)
			{
				this.GameObject.LocalScale = new Vector3(0.52f, 0.52f, 1);
				AnimTick = 0;
				Shrunk = true;
			}
			else
			{
				this.GameObject.LocalScale = new Vector3(0.58f, 0.58f, 1);	
				AnimTick = 0;
				Shrunk = false;			
			}

		}
	}
}
