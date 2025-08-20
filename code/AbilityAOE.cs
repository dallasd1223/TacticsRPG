using Sandbox;
using System;

public class AbilityAOE
{

}

public class AOEData
{
	public RangeShape Shape;
	public int Distance; 
	public int Radius;
}

public enum RangeShape
{
	Line,
	Cross,
	Donut,
	XDiagonal,
}
