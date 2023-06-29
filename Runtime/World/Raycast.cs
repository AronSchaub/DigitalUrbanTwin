using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;

namespace Leipzig3DGodot.Scripts;

public partial class Raycast : Node3D
{
    private CsgMesh3D? cursor;
    private RigidBody3DAccess? body;
    private List<ISelectable> oldBodies = new();


    public override void _Ready()
    {
        body = this.GetChild<RigidBody3DAccess>();
        cursor = this.GetChild<CsgMesh3D>(true);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("rotate_right"))
        {
            RotateY((float)(delta));
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            RotateY(-(float)(delta));
        }


        Array<Node3D> collidingBodies = body.GetCollidingBodies();
        if (collidingBodies.Count > 0)
        {
            if (cursor != null && body.State.GetContactCount() > 0)
            {
                cursor.Position = cursor.Position.Lerp(body.Position + body.State.GetContactColliderPosition(0), (float)delta * 2f);
                // cursor.Position = cursor.Position.Lerp(Position + body.State.GetContactLocalPosition(0), (float)delta * 2f);
                cursor.LookAt(new Vector3(cursor.GlobalPosition.X + 1, cursor.GlobalPosition.Y, cursor.GlobalPosition.Z + 1), Vector3.Up);
            }

            List<ISelectable> newBodies = collidingBodies.Select(b => b.GetParent()).OfType<ISelectable>().ToList();
            foreach (var selectable in oldBodies.Except(newBodies))
            {
                selectable.UnHover();
                GD.Print("UnHover: " + string.Join(',', collidingBodies.Select(b => b.Name)));
            }

            foreach (var selectable in newBodies.Except(oldBodies))
            {
                selectable.Hover();
                GD.Print("Hover: " + string.Join(',', collidingBodies.Select(b => b.Name)));
            }

            oldBodies.Clear();
            oldBodies.AddRange(newBodies);
        }
        else
        {
            foreach (var selectable in oldBodies)
            {
                selectable.UnHover();
                GD.Print("UnHover: " + string.Join(',', collidingBodies.Select(b => b.Name)));
            }

            oldBodies.Clear();
        }

        if (Input.IsActionPressed("select") || Input.IsActionPressed("ui_select"))
        {
            if (oldBodies.FirstOrDefault() is { } selectable)
            {
                selectable.Select();
            }
            else
            {
                // BuildViewModel.Instance.CurrentSelected?.DeSelect();
                // BuildViewModel.Instance.CurrentSelected = null;
            }
        }
    }
}