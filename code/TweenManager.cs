using Sandbox;
using System;
using SpriteTools;

namespace TacticsRPG;
public static class TweenManager
{

	private interface ITweenInternal
	{
		void UpdateInternal(float deltaTime);
		bool IsDone();
	}

	private class TweenWrapper<T> : ITweenInternal
	{
		private readonly Tween<T> tween;
		public TweenWrapper(Tween<T> tween) => this.tween = tween;
		public void UpdateInternal(float dt) => tween.Update(dt);
		public bool IsDone() => tween.IsCompleted;
	}

	private static readonly List<ITweenInternal> Tweens = new();

	public static Tween<T> To<T>(T start, T end, float duration, EasingType ease, Func<T, T, float, T> lerp, Action<T> onUpdate)
	{
		var tween = new Tween<T>(start, end, duration, ease, lerp, onUpdate);
		Tweens.Add(new TweenWrapper<T>(tween));
		return tween;
	}

	public static void Update(float deltaTime)
	{
		for (int i = Tweens.Count() - 1; i >= 0; i--)
		{
			var tween = Tweens[i];
			tween.UpdateInternal(deltaTime);
			if(tween.IsDone())
			{
				Tweens.RemoveAt(i);
			}
		}
	}
}

public class Tween<T>
{
	public float Duration;
	public float Elapsed;
	public float Delay;
	public T Value;
	public EasingType Type;
	public Func<T, T, float, T> LerpFunc;
	public T StartValue;
	public T EndValue;
	public Action<T> OnUpdate;
	public Action OnComplete;

	public bool IsCompleted => Elapsed >= Duration;
	public Tween<T> SetDelay(float delay) { Delay = delay; return this;}
	public Tween<T> OnCompleteCall(Action callback) { OnComplete = callback; return this; }
	public Tween(T start, T end, float duration, EasingType ease, Func<T, T, float, T> lerp, Action<T> onUpdate)
	{
		StartValue = start;
		EndValue = end;
		Duration = duration;
		Value = start;
		Type = ease;
		LerpFunc = lerp;
		OnUpdate = onUpdate;
		Elapsed = 0;
	}

	public void Update(float deltaTime)
	{
		if(IsCompleted) return;

		if (Delay > 0f)
		{
			Delay -= deltaTime;
			return;
		}

		Elapsed += deltaTime;
		float t = Math.Min(Elapsed / Duration, 1f);
		float eased = Ease.GetEaseValue(Type, t);
		OnUpdate?.Invoke(Value = LerpFunc(StartValue, EndValue, eased));


		if(IsCompleted) OnComplete?.Invoke();
	}

	
}

public static class TweenExtensions
{
	public static Tween<Vector3> MoveTo(this GameObject t, Vector3 end, float duration, EasingType ease = EasingType.Linear)
	{
		Vector3 start = t.WorldPosition;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Vector3.Lerp(a, b, t), v => t.WorldPosition = v);
	}

	public static Tween<Rotation> RotateTo(this GameObject t, Rotation end, float duration, EasingType ease = EasingType.Linear)
	{
		Rotation start = t.WorldRotation;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Rotation.Lerp(a, b, t), v => t.WorldRotation = v);
	}

	public static Tween<Vector3> ScaleTo(this GameObject t, Vector3 end, float duration, EasingType ease = EasingType.Linear)
	{
		Vector3 start = t.WorldScale;
		return TweenManager.To(start, end, duration, ease, (a,b,t) => Vector3.Lerp(a, b, t), v => t.WorldScale = v);
	}

	public static Tween<Color> ColorTo(this Color t, Color end, float duration, EasingType ease = EasingType.Linear)
	{
		Color start = t;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Color.Lerp(a, b, t), v => t = v);
	}

	public static Tween<Color> ColorTo(this ModelRenderer m, Color end, float duration, EasingType ease = EasingType.Linear)
	{
		
		Color start = m.Tint;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Color.Lerp(a, b, t), v => m.Tint = v);
	}

	public static Tween<Color> ColorTo(this SpriteComponent m, Color end, float duration, EasingType ease = EasingType.Linear)
	{
		
		Color start = m.FlashTint;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Color.Lerp(a, b, t), v => m.FlashTint = v);
	}

	public static Tween<Color> ColorTo(this SkyBox2D s, Color end, float duration, EasingType ease = EasingType.Linear)
	{
		
		Color start = s.Tint;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Color.Lerp(a, b, t), v => s.Tint = v);
	}

	public static Tween<Color> ColorTo(this PointLight l, Color end, float duration, EasingType ease = EasingType.Linear)
	{
		
		Color start = l.LightColor;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => Color.Lerp(a, b, t), v => l.LightColor = v);
	}

	public static Tween<float> FadeTo(this Color t, float end, float duration, EasingType ease = EasingType.Linear)
	{
		float start = t.a;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => MathX.Lerp(a, b, t), v => t = new Color(v, t.b, t.g, t.r));

	}

	public static Tween<float> FadeTo(this SpriteComponent s, float end, float duration, EasingType ease = EasingType.Linear)
	{
		float start = s.Tint.a;
		return TweenManager.To(start, end, duration, ease, (a, b, t) => MathX.Lerp(a, b, t), v => s.Tint = new Color(v, s.Tint.b, s.Tint.g, s.Tint.r));

	}

}
