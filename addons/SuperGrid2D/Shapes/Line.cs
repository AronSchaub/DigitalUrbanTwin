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

public struct Line : IConvex2D
{
	public readonly Vector2 v;
	public readonly Vector2 w;

	public Line(Vector2 v, Vector2 w)
	{
		this.v = v;
		this.w = w;
	}

	public Line(float vx, float vy, float wx, float wy)
	{
		v = new Vector2(vx, vy);
		w = new Vector2(wx, wy);
	}

	/// <summary>
	///     Returns the shortest distance squared from a point to this line
	/// </summary>
	public float DistanceSquared(Vector2 position)
	{
		float lengthSquared = Utility.DistanceSquared(v, w);
		if (lengthSquared < float.Epsilon)
			return Utility.DistanceSquared(v, position); // when we're actually a dot

		float t = Math.Max(0, Math.Min(1, (position - v).Dot(w - v) / lengthSquared));
		var projection = v + (w - v) * t;

		return Utility.DistanceSquared(position, projection);
	}

	public bool NoContactCertainty(IConvex2D shape)
	{
		float minA = 0;
		float maxA = 0;
		float minB = 0;
		float maxB = 0;


		var normal = new Vector2(-(w.Y - v.Y), w.X - v.X).Normalized();

		shape.Project(normal, out minA, out maxA);
		Project(normal, out minB, out maxB);

		// Check if the polygon projections are currently NOT intersecting
		if (Utility.IntervalDistance(minA, maxA, minB, maxB) > 0)
			return true;

		return false;
	}

	/// <summary>
	///     Modified code from http://playtechs.blogspot.com/2007/03/raytracing-on-grid.html
	///     Traces the line segment and returns all cell indexes that are passed
	/// </summary>
	public IEnumerable<Vector2I> Supercover(IGridDimensions2D grid)
	{
		// All flooring is done with a cast to int so this will only work
		// for positive values. Which is fine for our grid since after we normalized our coords
		// to the grid there are no negative values (except when we're out of bounds, but that's not allowed)

		// Set to offset of grid and make each integer correspond to a cell
		var normalizedV = (v - grid.TopLeft) / grid.CellSize;
		var normalizedW = (w - grid.TopLeft) / grid.CellSize;

		float lineDeltaX = Math.Abs(normalizedW.X - normalizedV.X);
		float lineDeltaY = Math.Abs(normalizedW.Y - normalizedV.Y);

		// starting position in grid
		var x = (int) normalizedV.X;
		var y = (int) normalizedV.Y;

		var totalSteps = 1;
		int xDirection, yDirection;

		float error;

		if (lineDeltaX < float.Epsilon)
		{
			xDirection = 0;
			error = float.PositiveInfinity;
		}
		else if (w.X > v.X)
		{
			xDirection = 1;
			totalSteps += (int) normalizedW.X - x;
			error = (float) ((Math.Floor(normalizedV.X) + 1 - normalizedV.X) * lineDeltaY);
		}
		else
		{
			xDirection = -1;
			totalSteps += x - (int) normalizedW.X;
			error = (float) ((normalizedV.X - Math.Floor(normalizedV.X)) * lineDeltaY);
		}

		if (lineDeltaY < float.Epsilon)
		{
			yDirection = 0;
			error -= float.PositiveInfinity;
		}
		else if (w.Y > v.Y)
		{
			yDirection = 1;
			totalSteps += (int) normalizedW.Y - y;
			error -= (float) (Math.Floor(normalizedV.Y) + 1 - normalizedV.Y) * lineDeltaX;
		}
		else
		{
			yDirection = -1;
			totalSteps += y - (int) normalizedW.Y;
			error -= (float) (normalizedV.Y - Math.Floor(normalizedV.Y)) * lineDeltaX;
		}

		for (; totalSteps > 0; --totalSteps)
		{
			// Only return values that are inside the grid
			// we can't clamp the line before hand because that could alter the direction of the line
			if (x >= 0 && x < grid.Columns &&
			    y >= 0 && y < grid.Rows)
				yield return new Vector2I(x, y);

			if (error > 0)
			{
				y += yDirection;
				error -= lineDeltaX;
			}
			else
			{
				x += xDirection;
				error += lineDeltaY;
			}
		}
	}

	public void Project(Vector2 normal, out float min, out float max)
	{
		float vProj = normal.Dot(v);
		float wProj = normal.Dot(w);

		if (vProj > wProj)
		{
			max = vProj;
			min = wProj;
		}
		else
		{
			max = wProj;
			min = vProj;
		}
	}
}