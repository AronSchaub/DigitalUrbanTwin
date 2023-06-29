using Godot;
using System.Runtime.CompilerServices;

namespace SuperGrid2D;

public static class Utility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float DistanceSquared(Vector2 a, Vector2 b)
	{
		var d = a - b;
		return (d.X * d.X) + (d.Y * d.Y);
	}

	/// <summary>
	/// A helper function that does a dot product where v is op Vector2 type but the other Vector is supplied with 2 float values
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Dot(Vector2 v, float x2, float y2)
	{
		return v.X * x2 + v.Y * y2;
	}

	// Calculate the distance between [minA, maxA] and [minB, maxB]
	// The distance will be negative if the intervals overlap
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float IntervalDistance(float minA, float maxA, float minB, float maxB)
	{
		return minA < minB ? minB - maxA : minA - maxB;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Lerp(float a, float b, float t)
	{
		return a + (b - a) * Clamp01(t);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Clamp01(float value)
	{
		if (value < 0F)
			return 0F;
		else if (value > 1F)
			return 1F;
		else
			return value;
	}

	public static readonly Vector2 Vector2Up = new Vector2(0, 1);
	public static readonly Vector2 Vector2Right = new Vector2(1, 0);

}