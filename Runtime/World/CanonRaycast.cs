using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Leipzig3DGodot.Scripts;

public partial class CanonRaycast : Path3D
{
    private PathFollow3D? pathFollow;
    private RayCast3D? rayCast3D;
    private CsgMesh3D? cursor;
    private RigidBody3DAccess? body;
    private CollisionPolygon3D collisionPolygon;


    public override void _Ready()
    {
        // CSGPolygon3D rayCastMesh = GetNode<CSGPolygon3D>("RaycastMesh");
        body = this.GetChild<RigidBody3DAccess>();
        body.BodyEntered += OnBodyEntered;
        cursor = this.GetChild<CsgMesh3D>(true);
        collisionPolygon = this.GetChild<CollisionPolygon3D>(true);
        pathFollow = this.GetChild<PathFollow3D>(true);
        rayCast3D = pathFollow.GetChild<RayCast3D>(true);
    }

    private void OnBodyEntered(Node body)
    {
        GD.Print(body);
    }

    public override void _Process(double delta)
    {
        Array<Node3D> collidingBodies = body.GetCollidingBodies();
        if (collidingBodies.Count > 0)
        {
            if (cursor != null)
            {
                cursor.Position = cursor.Position.Lerp(-body.State.GetContactLocalPosition(0), (float)delta);
            }

            GD.Print(string.Join(',', collidingBodies.Select(b => b.Name)));
        }
        // if (rayCast3D.IsColliding())
        // {
        //     GD.Print(rayCast3D.GetCollider());
        //     if (cursor != null)
        //     {
        //         cursor.Position = rayCast3D.GetCollisionPoint();
        //     }
        // }
        // else
        // {
        //     if (pathFollow != null)
        //     {
        //         pathFollow.Progress += (float)delta * 100f;
        //         if (cursor != null)
        //         {
        //             cursor.Position = pathFollow.Position;
        //         }
        //     }
        // }
    }
}