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

/// <summary>
///     Axis aligned rectangle
/// </summary>
public struct Aabb : IConvex2D
{
	public readonly Vector2 topLeft;
	public readonly Vector2 bottomRight;

	public Aabb(Vector2 topLeft, float width, float height)
	{
		this.topLeft = topLeft;
		bottomRight = new Vector2(topLeft.X + width, topLeft.Y + height);
	}

	public Aabb(Vector2 topLeft, Vector2 bottomRight)
	{
		this.topLeft = topLeft;
		this.bottomRight = bottomRight;
	}

	public float DistanceSquared(Vector2 position)
	{
		float dX = position.X - Math.Max(Math.Min(position.X, bottomRight.X), topLeft.X);
		float dY = position.Y - Math.Max(Math.Min(position.Y, bottomRight.Y), topLeft.Y);

		return Math.Max(0, dX * dX + dY * dY);
	}

	public IEnumerable<Vector2I> Supercover(IGridDimensions2D grid)
	{
		var minX = (int) Math.Max(0, (topLeft.X - grid.TopLeft.X) / grid.CellSize.X);
		var minY = (int) Math.Max(0, (topLeft.Y - grid.TopLeft.Y) / grid.CellSize.Y);

		var maxX = (int) Math.Min(grid.Columns - 1, (bottomRight.X - grid.TopLeft.X) / grid.CellSize.X);
		var maxY = (int) Math.Min(grid.Rows - 1, (bottomRight.Y - grid.TopLeft.Y) / grid.CellSize.Y);

		for (int x = minX; x <= maxX; x++)
		for (int y = minY; y <= maxY; y++)
			yield return new Vector2I(x, y);
	}

	// Calculate the projection of this shape on an axis
	// and returns it as a [min, max] interval
	public void Project(Vector2 normal, out float min, out float max)
	{
		// First corner
		float proj = normal.Dot( topLeft);
		min = max = proj;

		// Other corners
		proj = Utility.Dot(normal, bottomRight.X, topLeft.Y);
		if (proj < min)
			min = proj;
		else if (proj > max)
			max = proj;

		proj = normal.Dot( bottomRight);
		if (proj < min)
			min = proj;
		else if (proj > max)
			max = proj;

		//proj = Utility.Dot(normal, topLeft.X, bottomRight.Y);
		//if (proj < min)
		//    min = proj;
		//else if (proj > max)
		//max = proj;
	}

	public bool NoContactCertainty(IConvex2D shape)
	{
		float minA = 0;
		float maxA = 0;
		float minB = 0;
		float maxB = 0;

		// Only check up & right normals for an Aabb since down & left are the same axes

		shape.Project(Utility.Vector2Up, out minA, out maxA);
		Project(Utility.Vector2Up, out minB, out maxB);

		// Check if the polygon projections are currently NOT intersecting
		if (Utility.IntervalDistance(minA, maxA, minB, maxB) > 0)
			return true;

		shape.Project(Utility.Vector2Right, out minA, out maxA);
		Project(Utility.Vector2Right, out minB, out maxB);

		// Check if the polygon projections are currently NOT intersecting
		if (Utility.IntervalDistance(minA, maxA, minB, maxB) > 0)
			return true;

		return false;
	}
}