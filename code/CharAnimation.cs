using System;
using Sandbox;
using Sandbox.UI;
using TacticsRPG;

public class BounceInAnimation : ICharAnimation
{
	//internal
	private float time;
	//exposed
	private float duration;
	private float startY;
	private float currentY;
	private float endY;

	public bool IsFinished => time >= duration;
	public bool IsStarted {get; set;} = false;

	public BounceInAnimation(float startY = -80f, float endY = 0f, float duration = 0.6f)
	{
		this.startY = startY;
		this.endY = endY;
		this.duration = duration;
	}

	public void Reset(Panel panel1, Panel panel2 = null)
	{
		time = 0;
	}

	public void Update(Panel panel1, float dt, Panel panel2 = null)
	{
		if(!IsStarted) return;
		if(IsFinished) return;
		time += dt;
		float t = MathF.Min(time / duration, 1f);
		float easedt = Ease.EaseOutBounce(t);
		var fc = panel1.Parent as FloatingChar;
		endY = fc.Position.y;
		currentY = endY + startY;
		var lerpval = MathX.Lerp(currentY, endY, easedt);
		panel1.Style.Top = lerpval;
		
		if(panel2 is not null)
		{
		panel2.Style.Top = lerpval + 8;			
		}

	}
}
