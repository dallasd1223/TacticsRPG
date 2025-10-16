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


public class FadeOutAnimation : ICharAnimation
{
	private float time;

	private float duration;

	public bool IsFinished => time >= duration;
	public bool IsStarted {get; set;} = false;

	public FadeOutAnimation(float duration)
	{
		this.duration = duration;
	}

	public void Reset(Panel panel1, Panel panel2 = null)
	{
		time = 0f;
	}

	public void Update(Panel panel1, float dt, Panel panel2)
	{
		if(!IsStarted) return;
		if(IsFinished) return;

		time += dt;
		float t = MathF.Min(time/duration, 1f);
		panel1.Style.Opacity = 1 - t;

		if(panel2.IsValid())
		{
			panel2.Style.Opacity = 1 - t;
		}
	}
}

public class RainbowColorAnimation : ICharAnimation
{	
	private float time;
	private float delay;

	public bool IsFinished {get;} = false;
	public bool IsStarted {get; set;} = false;

	public RainbowColorAnimation(float delay)
	{
		this.delay = delay;
	}

	public void Reset(Panel panel1, Panel panel2 = null)
	{
		time = 0;
	}

	public void Update(Panel panel1, float dt, Panel panel2 = null)
	{
		if(!IsStarted) return;
		panel1.Style.FontColor = GetRainbow();
	}

	public Color GetRainbow()
	{
		float hue = (((float)RealTime.Now - delay * 0.4f)* 0.2f) % 1f;
		return HSVToRGB(hue, 1f, 1f);
	}

	public Color HSVToRGB(float h, float s, float v)
    {
        int i = (int)Math.Floor(h * 6f);
        float f = h * 6f - i;
        float p = v * (1f - s);
        float q = v * (1f - f * s);
        float t = v * (1f - (1f - f) * s);

        float r = 0, g = 0, b = 0;
        switch (i % 6)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            case 5: r = v; g = p; b = q; break;
        }

        return new Color(r, g, b, 1f); // full alpha
	}
}

public class WaitAnimation : ICharAnimation
{
	public float time;
	public float duration;

	public bool IsFinished => time >= duration;
	public bool IsStarted {get; set;} = false;

	public WaitAnimation(float duration)
	{
		this.duration = duration;
	}

	public void Reset(Panel panel1, Panel panel2)
	{
		time = 0;
	}

	public void Update(Panel panel1, float dt, Panel panel2)
	{
		if(!IsStarted) return;
		if(IsFinished) return;
		time += dt;
	}
}

public class CharAnimationSequence : ICharAnimation
{
	private Queue<ICharAnimation> animations = new();
	private ICharAnimation current;

	public CharAnimationSequence(params ICharAnimation[] anims)
	{
		foreach(var anim in anims)
		{
			animations.Enqueue(anim);
		}
		Log.Info("Anims Queued");	
		current = animations.Dequeue();
	}

	public bool IsFinished => current == null;
	public bool IsStarted {get; set;} = false;

	public void Reset(Panel panel1, Panel panel2)
	{
		current?.Reset(panel1);
		current.IsStarted = true;
	}

	public void Update(Panel panel1, float dt, Panel panel2 = null)
	{
		if (current == null) return;
		
		current.Update(panel1, dt, panel2);
		if(current.IsFinished && animations.Count > 0)
		{
			current = animations.Dequeue();
			current.Reset(panel1, panel2);
			current.IsStarted = true;
		}
		else if(current.IsFinished)
		{
			current = null;
		}
	}
}
