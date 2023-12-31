/*
    Copyright(c) 2018 Bart van de Sande / Nonline, https://www.nonline.nl

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Godot;

namespace SuperGrid2D;

public struct Circle : IConvex2D
{
	public readonly Vector2 center;
	public readonly float radius;

	public Circle(Vector2 center, float radius)
	{
		this.center = center;
		this.radius = radius;
	}

	public Circle(float x, float y, float radius)
	{
		center = new Vector2(x, y);
		this.radius = radius;
	}

	public float DistanceSquared(Vector2 position)
	{
		// Can't get rid of the sqrt operation unfortunately because radius has to be taken into account
		float dEdge = position.DistanceTo(center)-radius;
		return dEdge > 0 ? dEdge * dEdge : 0;
	}

	/// <summary>
	///     No SAT here
	///     A shape doesn't touch a circle when it's closest distance to the circle's center is greater than the circle's radius
	/// </summary>
	public bool NoContactCertainty(IConvex2D shape)
	{
		return shape.DistanceSquared(center) > radius * radius;
	}

	/// <summary>
	///     The supercover of a circle is it's bounding box
	/// </summary>
	public IEnumerable<Vector2I> Supercover(IGridDimensions2D grid)
	{
		var offsetPosition = center - grid.TopLeft;

		var minX = (int) Math.Max(0, (offsetPosition.X - radius) / grid.CellSize.X);
		var minY = (int) Math.Max(0, (offsetPosition.Y - radius) / grid.CellSize.Y);

		var maxX = (int) Math.Min(grid.Columns - 1, (offsetPosition.X + radius) / grid.CellSize.X);
		var maxY = (int) Math.Min(grid.Rows - 1, (offsetPosition.Y + radius) / grid.CellSize.Y);

		for (int x = minX; x <= maxX; x++)
		for (int y = minY; y <= maxY; y++)
			yield return new Vector2I(x, y);
	}

	/// <summary>
	///     Projection of a circle the same on all axes
	/// </summary>
	public void Project(Vector2 normal, out float min, out float max)
	{
		float dot = normal.Dot(center);
		min = dot - radius;
		max = dot + radius;
	}
}