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

using System.Collections.Generic;
using Godot;

namespace SuperGrid2D;

/// <summary>
///     Implementation that allows for updating unit positions, faster removal and copying.
///     Downside is that is a little bit slower to query.
/// </summary>
public class DynamicGrid2D<TKey, T> : GridBase2D<T, DynamicGrid2D<TKey, T>.DynamicCell2D>
{
	/// <summary>
	///     Dictionary for easy access to updating / removing units
	/// </summary>
	private readonly Dictionary<TKey, UnitWrapper> wrappers = new Dictionary<TKey, UnitWrapper>();

	public DynamicGrid2D(Vector2 topLeft, float width, float height, Vector2 cellSize) : base(topLeft, width, height, cellSize)
	{
	}

	public DynamicGrid2D(Vector2 topLeft, float width, float height, float cellSize) : base(topLeft, width, height, cellSize)
	{
	}

	public DynamicGrid2D(Vector2 center, float radius, float cellSize) : base(center, radius, cellSize)
	{
	}

	/// <summary>
	///     Retreives unit by key
	/// </summary>
	public T this[TKey key] => wrappers[key].Unit;

	protected override DynamicCell2D _createNewCell(Vector2I location)
	{
		return new DynamicCell2D();
	}

	/// <summary>
	///     Adds a unit.
	/// </summary>
	public void Add(TKey key, T unit, IConvex2D shape)
	{
		var wrapper = new UnitWrapper(unit, shape);

		foreach (DynamicCell2D? cell in _getOrCreateSupercover(shape))
			cell.Add(key, wrapper);

		wrappers.Add(key, wrapper);
		Count++;
	}

	/// <summary>
	///     Removes unit
	/// </summary>
	public bool Remove(TKey key)
	{
		UnitWrapper wrapper;

		if (wrappers.TryGetValue(key, out wrapper))
		{
			foreach (DynamicCell2D? cell in _getOrCreateSupercover(wrapper.Shape))
				cell.Remove(key);

			wrappers.Remove(key);
			Count--;

			return true;
		}

		return false;
	}

	/// <summary>
	///     Updates a unit's position / shape
	/// </summary>
	public void Update(TKey key, IConvex2D newShape)
	{
		UnitWrapper? wrapper = wrappers[key];

		foreach (DynamicCell2D? cell in _getOrCreateSupercover(wrapper.Shape))
			cell.Remove(key);

		var newWrapper = new UnitWrapper(wrapper.Unit, newShape);

		foreach (DynamicCell2D? cell in _getOrCreateSupercover(newShape))
			cell.Add(key, newWrapper);
		wrappers[key] = newWrapper;
	}

	/// <summary>
	///     Clears the entire grid
	/// </summary>
	public override void Clear()
	{
		base.Clear();
		wrappers.Clear();
	}

	/// <summary>
	///     Method to enable foreach-loops
	/// </summary>
	public IEnumerator<T> GetEnumerator()
	{
		foreach (UnitWrapper? wrapper in wrappers.Values)
			yield return wrapper.Unit;
	}

	#region Nested type: DynamicGrid2D<TKey,T>.DynamicCell2D

	/// <summary>
	///     Uses a dictionary
	/// </summary>
	public class DynamicCell2D : CellBase
	{
		private readonly Dictionary<TKey, UnitWrapper> _unitDictionary = new Dictionary<TKey, UnitWrapper>();
		protected override IEnumerable<UnitWrapper> _unitWrappers => _unitDictionary.Values;
		public override int Count => _unitDictionary.Count;

		public void Add(TKey key, UnitWrapper wrapper)
		{
			_unitDictionary.Add(key, wrapper);
		}

		public bool Remove(TKey key)
		{
			return _unitDictionary.Remove(key);
		}
	}

	#endregion
}