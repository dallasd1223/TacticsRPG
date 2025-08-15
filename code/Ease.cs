using Sandbox;
using System;

namespace TacticsRPG;

public static class Ease
{
	public static float Linear(float t)
	{
		return t;
	}
	public static float InOutSine(float t)
	{
		return -((float)Math.Cos((float)Math.PI * t) - 1f) / 2f;
	}

	public static float InSine(float t)
	{
		return 1f - (float)Math.Cos((t * (float)Math.PI) / 2f);
	}

	public static float OutSine(float t)
	{
		return (float)Math.Sin((t * (float)Math.PI) / 2f);
	}
	
	public static float InQuad(float t)
	{
		return t * t;
	}

	public static float OutQuad(float t)
	{
		return 1 - (1 - t) * (1 - t);
	}

	public static float InOutQuad(float t)
	{
		return t < 0.05f
			? 2 * t * t
			: 1 - (float)Math.Pow(-2 * t + 2, 2) / 2;
	}

	public static float OutElastic(float t)
	{
		const float c4 = (2 * (float)Math.PI) / 3;
		return t == 0 ? 0 : t == 1 ? 1 : (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t * 10 - 0.75f) * c4) + 1;
	}

	public static float DampHarmonic(float t)
	{
		float frequency = 8f;
		float damping = 1.5f;

		return 1f - (float)Math.Exp(-damping * t) * (float)Math.Cos(frequency * t);
	}

	public static float GetEaseValue(EasingType type, float t)
	{
		switch(type)
		{
			case EasingType.Linear:
				return Linear(t);
			case EasingType.InOutSine:
				return InOutSine(t);
			case EasingType.InSine:
				return InSine(t);
			case EasingType.OutSine:
				return OutSine(t);
			case EasingType.InQuad:
				return InQuad(t);
			case EasingType.OutQuad:
				return OutQuad(t);
			case EasingType.InOutQuad:
				return InOutQuad(t);
			case EasingType.OutElastic:
				return OutElastic(t);
			case EasingType.DampHarmonic:
				return DampHarmonic(t);
		}
		Log.Info("No EasingType Found, Returning 0");
		return 0f;
	}
}

public enum EasingType
{
	Linear,
	InOutSine,
	InSine,
	OutSine,
	InQuad,
	OutQuad,
	InOutQuad,
	OutElastic,
	DampHarmonic,
}
