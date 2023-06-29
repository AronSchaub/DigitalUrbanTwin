using System.Collections.Generic;
using Godot;
using NetTopologySuite.GeometriesGraph;

namespace Leipzig3DGodot.Scripts.Vegetation;

public record TurtleTransform(Vector3 position, Quaternion rotation);

public class Turtle
{
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; } = Quaternion.Identity;
    public ImmediateMesh Mesh { get; } = new();

    private readonly Stack<TurtleTransform> transformInfos = new();

    public Turtle(Vector3 position)
    {
        this.Position = position;
    }

    public void Translate(Vector3 delta)
    {
        delta = Rotation * delta;
        DrawLine(Position, Position + delta, Colors.Black, 100f);
        Position += delta;
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, float timeToLive)
    {
        Mesh.SurfaceAddVertex(start);
        Mesh.SurfaceAddVertex(end);
    }

    public void Rotate(Vector3 delta) => Rotation = Quaternion.FromEuler(Rotation.GetEuler() + delta);

    public void Push() => transformInfos.Push(new TurtleTransform(Position, Rotation));

    public void Pop()
    {
        var poppedTransform = transformInfos.Pop();
        Position = poppedTransform.position;
        Rotation = poppedTransform.rotation;
    }

    public void StartDraw()
    {
        var standardMaterial3D = new StandardMaterial3D();
        standardMaterial3D.AlbedoColor = Colors.Black;
        Mesh.SurfaceBegin(Godot.Mesh.PrimitiveType.Lines, standardMaterial3D);
    }

    public void EndDraw()
    {
        Mesh.SurfaceEnd();
    }
}