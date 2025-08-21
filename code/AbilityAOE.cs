using Sandbox;
using System;

namespace TacticsRPG;

public class AbilityAOE
{

}

public class AOEData
{
	public RangeShape Shape;
	public int Range;
	public bool KeepCenter;

	public AOEData(RangeShape shape, int range, bool center)
	{
		Shape = shape;
		Range = range;
		KeepCenter = center;
	}
}

public enum RangeShape
{
	Line,
	Square,
	Cross,
	Donut,
	Diagonal,
	Diamond,
}
