//C# port of: https://github.com/KOBUGE-Games/CircularContainer/blob/master/addons/KOBUGE-games.CircularContainer/CircularContainer.gd

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[Tool]
public partial class CircularContainer : Container
{
	[ExportCategory("CircularContainer")]
	[Export]
	public bool ForceSquared
	{
		get => forceSquared;
		set
		{
			forceSquared = value;
			Resort();
		}
	}

	[Export]
	public bool ForceExpand
	{
		get => forceExpand;
		set
		{
			forceExpand = value;
			Resort();
		}
	}

	[Export(PropertyHint.Range, "0,359,0.01")]
	public float StartAngle
	{
		get => Mathf.RadToDeg(startAngle);
		set
		{
			startAngle = Mathf.DegToRad(value); 
			Resort();
		}
	}

	[Export(PropertyHint.Range, "0,1,0.01")]
	public float PercentVisible
	{
		get => percentVisible;
		set
		{
			percentVisible = value;
			Resort();
		}
	}

	[Export]
	public bool AppearAtOnce
	{
		get => appearAtOnce;
		set
		{
			appearAtOnce = value;
			Resort();
		}
	}

	[Export]
	public bool AllowNode2D
	{
		get => allowNode2D;
		set
		{
			allowNode2D = value;
			Resort();
		}
	}

	[Export]
	public bool StartEmpty
	{
		get => startEmpty;
		set
		{
			startEmpty = value;
			Resort();
		}
	}

	[Signal]
	public delegate void MinimumSizeChangedEventHandler();

	public Action<Node, Vector2, Vector2, float> CustomAnimatorFunc { get; set; } = DefaultAnimator;

	private string cachedMinSizeKey = "";
	private Vector2 cachedMinSize;
	private bool cachedMinSizeDirty;
	private bool forceSquared;
	private bool forceExpand;
	private float startAngle;
	private float percentVisible = 1;
	private bool appearAtOnce;
	private bool allowNode2D;
	private bool startEmpty;

	public override void _Ready()
	{
		SortChildren += Resort;
		ChildEnteredTree += OnChildTreeEvent;
		ChildExitingTree += OnChildTreeEvent; 
		Resort();
	}

	private void OnChildTreeEvent(Node node)
	{
		Resort();
	}

	public override void _ExitTree()
	{
		SortChildren -= Resort;
		ChildEnteredTree -= OnChildTreeEvent;
		ChildExitingTree -= OnChildTreeEvent;
	}

	public void Resort()
	{
		var rect = GetRect();
		var origin = rect.Size / 2f;

		List<Node> children = GetFilteredChildren<Node>().ToList();
		if (children.Count == 0)
			return;

		var minChildSize = new Vector2();
		foreach (var child in children)
		{
			var size = GetChildMinSize(child);
			minChildSize.X = Mathf.Max(minChildSize.X, size.X);
			minChildSize.Y = Mathf.Max(minChildSize.Y, size.Y);
		}

		float radius = Mathf.Min(rect.Size.X - minChildSize.X, rect.Size.Y - minChildSize.Y) / 2f;
		if (!cachedMinSizeDirty)
		{
			cachedMinSizeDirty = true;
			CallDeferred(nameof(UpdateCachedMinSize));
		}

		var angleRequired = 0f;
		var totalStretchRatio = 0f;
		var anglesForChild = new List<float>();
		foreach (var child in children)
		{
			float angle = GetMaxAngleForDiagonal(GetChildMinSize(child).Length(), radius);
			angleRequired += angle;
			anglesForChild.Add(angle);
			totalStretchRatio += GetChildStretchRatio(child);
		}

		if (totalStretchRatio > 0)
		{
			for (var i = 0; i < children.Count; i++)
			{
				var child = children[i];
				anglesForChild[i] += (2f * Mathf.Pi - angleRequired) * GetChildStretchRatio(child) / totalStretchRatio;
			}
		}

		float angleReached = startAngle;
		if (!StartEmpty)
			angleReached -= anglesForChild[0] / 2f;

		float appear = PercentVisible;
		if (!AppearAtOnce)
			appear *= children.Count;

		for (var i = 0; i < children.Count; i++)
		{
			var child = children[i];
			PutChildAtAngle(child, radius, origin, angleReached, anglesForChild[i], Mathf.Clamp(appear, 0, 1));
			angleReached += anglesForChild[i];
			if (!AppearAtOnce)
				appear -= 1;
		}
	}

	public void PutChildAtAngle(Node child, float radius, Vector2 origin, float angleStart, float angleSize, float appear)
	{
		var size = GetChildMinSize(child);
		var target = new Vector2(0, -radius).Rotated(-(angleStart + angleSize / 2f)) + origin;
		if (child is Control control) control.Size = size;
		CustomAnimatorFunc(child, origin, target, appear);
	}

	public void UpdateCachedMinSize()
	{
		if (!cachedMinSizeDirty)
			return;
		cachedMinSizeDirty = false;

		List<Node> children = GetFilteredChildren<Node>().ToList();
		var minRadius = 1f;
		var minChildSize = new Vector2();
		var maxRadius = 1f;
		var diagonals = new List<float>();
		foreach (var size in children.Select(GetChildMinSize))
		{
			minChildSize.X = Mathf.Max(minChildSize.X, size.X);
			minChildSize.Y = Mathf.Max(minChildSize.Y, size.Y);
			float diagonal = size.Length();
			minRadius = Mathf.Max(minRadius, diagonal / 2f);
			maxRadius += diagonal / 2f;
			diagonals.Add(diagonal);
		}

		string key = string.Join(',', diagonals);
		if (cachedMinSizeKey == key)
			return;

		while (maxRadius > minRadius + 0.5)
		{
			float newRadius = (maxRadius + minRadius) / 2f;
			float angleRequired = children.Sum(child => GetMaxAngleForDiagonal(GetChildMinSize(child).Length(), newRadius));
			if (angleRequired < 2 * Mathf.Pi)
				maxRadius = newRadius;
			else
				minRadius = newRadius;
		}

		cachedMinSize = new Vector2(maxRadius, maxRadius) * 2 - minChildSize;
		cachedMinSizeKey = key;

		EmitSignal(SignalName.MinimumSizeChanged);
	}

	public static void DefaultAnimator(Node node, Vector2 containerCenter, Vector2 targetPos, float time)
	{
		switch (node)
		{
			case Control control:
				control.SetPosition(containerCenter.Lerp(targetPos - control.Size / 2f * time, time));
				control.Scale = time == 0 ? new Vector2(0.01f, 0.01f) : new Vector2(time, time);
				break;
			case Node2D node2D:
				node2D.Position = containerCenter.Lerp(targetPos, time);
				node2D.Scale = time == 0 ? new Vector2(0.01f, 0.01f) : new Vector2(time, time);
				break;
		}
	}

	public IEnumerable<T> GetFilteredChildren<T>() where T : Node
	{
		for (int i = GetChildCount() - 1; i >= 0; i--)
		{
			var child = GetChild<T>(i);
			var keep = false;
			if (child is Control)
				keep = true;
			else if (AllowNode2D && child is Node2D)
				keep = true;
			if (child is CanvasItem canvasItem && canvasItem.Visible == false)
				keep = false;
			if (!keep)
				RemoveChild(child);
			yield return child;
		}
	}

	public Vector2 GetChildMinSize(Node child)
	{
		if (child is Control control)
		{
			var size = control.GetCombinedMinimumSize();
			if (ForceSquared)
			{
				float s = Mathf.Max(size.X, size.Y);
				return new Vector2(s, s);
			}

			return size;
		}
		else
		{
			return new Vector2(0, 0);
		}
	}

	public float GetChildStretchRatio(Node child)
	{
		if (child is Control control && control.SizeFlagsHorizontal == SizeFlags.Expand)
			return control.SizeFlagsStretchRatio;
		if (child is Node2D)
			return 1;
		if (ForceExpand)
			return 1;
		return 0;
	}

	public static float GetMaxAngleForDiagonal(float diagonal, float radius)
	{
		float fitLength = diagonal / 2f;
		if (fitLength > radius)
			return Mathf.Pi;
		else
			return Mathf.Asin(fitLength / radius) * 2f;
	}
}
