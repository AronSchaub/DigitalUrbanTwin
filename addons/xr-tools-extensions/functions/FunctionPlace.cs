using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Leipzig3DGodot.Scripts;
using Leipzig3DGodot.Scripts.MVVM.ViewModel;
using Array = System.Array;

[Tool]
public partial class FunctionPlace : CharacterBody3D
{
    private const float TOLERANCE = 0.001f;
    private const string MIX_COLOR = "mix_color";
    private const string COLLIDER = "collider";
    private bool isOnFloor = true;
    private bool isPlacing = false;
    private bool canPlace = true;
    private float objectRotation = 0f;
    private Vector3 floorNormal = Vector3.Up;
    private Transform3D lastTargetTransform = new();
    private Shape3D? collisionShape;
    private float stepSize = 0.5f;

    private float ws;
    private XROrigin3D? originNode;
    private XRCamera3D? cameraNode;
    private XRController3D? controller;
    private MeshInstance3D? arcRay;
    private MeshInstance3D? target;
    private Node3D? figure;
    private bool enabled = true;
    private readonly List<ISelectable> oldBodies = new();

    public Node3D? ObjectToPlace { get; set; }

    [Export]
    public bool Enabled
    {
        get => enabled;
        set
        {
            if (value)
                SetPhysicsProcess(true);
            enabled = value;
        }
    }

    [Export]
    public MeshInstance3D? PlaceholderForNoObject { get; set; }

    [Export]
    public Color ColorCanBePlaced { get; set; }

    [Export]
    public Color ColorCantBePlaced { get; set; }

    [Export]
    public Color ColorNoCollision { get; set; }

    [Export]
    public float Strength { get; set; } = 5;

    [Export]
    public float MaxSlope { get; set; } = 20;

    [Export(PropertyHint.Layers3DPhysics)]
    public uint ValidArcRayMask { get; set; } = uint.MaxValue;

    [Export]
    public string PlaceButtonAction { get; set; } = "trigger_click";

    [Export]
    public string SelectionButtonAction { get; set; } = "ax_button";

    [Export]
    public string RotationAction { get; set; } = "primary";

    public MeshInstance3D ArcRay
    {
        get => arcRay ??= GetNode<MeshInstance3D>("ArcRay");
        set => arcRay = value;
    }

    public MeshInstance3D Target
    {
        get => target ??= GetNode<MeshInstance3D>("Target");
        set => target = value;
    }

    public Node3D Figure
    {
        get => figure ??= GetNode<Node3D>("Target/ObjectFigure");
        set => figure = value;
    }

    public Shape3D CollisionShape
    {
        get
        {
            if (collisionShape == null)
            {
                var collisionShape3D = this.GetChild<CollisionShape3D>();
                collisionShape = collisionShape3D.Shape;
                collisionShape3D.Shape = null;
                RemoveChild(collisionShape3D);
            }

            return collisionShape;
        }
        // set => collisionShape = value;
    }

    public XRController3D Controller
    {
        get => controller ??= GetParent<XRController3D>();
        set => controller = value;
    }

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            return;
        PlaceholderForNoObject ??= GetNode<MeshInstance3D>("Target/ObjectFigure/ObjectHolder");
        ws = (float)XRServer.WorldScale;
        originNode = XRHelpers.XROrigin;
        cameraNode = XRHelpers.XRCamera;

        ArcRay.Visible = false;
        Target.Visible = false;

        if (ArcRay.Mesh is PlaneMesh arcRayMesh)
            arcRayMesh.Size = new Vector2(0.05f * ws, 1.0f);
        if (Target.Mesh is PlaneMesh targetMesh)
            targetMesh.Size = new Vector2(ws, ws);
        Figure.Scale = new Vector3(ws, ws, ws);

        // BuildViewModel.Instance.UISelection = "res://Prefabs/Lanterns/schinkelleuchte_mast.tscn"; //TODO: remove. for testing only
        // UpdatePlayerHeight();
        // UpdatePlayerRadius();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
            return;

        if (originNode == null || cameraNode == null)
            return;

        if (!Enabled)
        {
            isPlacing = false;
            ArcRay.Visible = false;
            Target.Visible = false;
            HandleDeSelection();
            SetPhysicsProcess(false);
            return;
        }

        if (ObjectToPlace == null && IsPlaceMode())
        {
            var nextScene = (PackedScene)GD.Load(BuildViewModel.Instance.UISelection);
            var lantern = nextScene.Instantiate<Lantern>();
            lantern.Collision = false;
            Figure.AddChild(lantern);
            lantern.Owner = Figure;
            ObjectToPlace = lantern;
            if (PlaceholderForNoObject != null) PlaceholderForNoObject.Visible = false;
        }

        if (!Controller.GetIsActive())
            return;

        double newWs = XRServer.WorldScale;
        if (Math.Abs(ws - newWs) > TOLERANCE)
        {
            ws = (float)newWs;
            if (ArcRay.Mesh is PlaneMesh arcRayMesh)
                arcRayMesh.Size = new Vector2(0.05f * ws, 1.0f);
            if (Target.Mesh is PlaneMesh targetMesh)
                targetMesh.Size = new Vector2(ws, ws);
            Figure.Scale = new Vector3(ws, ws, ws);
        }

        if (Controller.IsButtonPressed(SelectionButtonAction))
        {
            HandleDeSelection();
        }

        if (Controller.IsButtonPressed(PlaceButtonAction))
        {
            HandleButtonPressed(delta);
        }
        else if (isPlacing)
        {
            HandlePlacingObject();
        }
    }

    private void HandlePlacingObject()
    {
        if (canPlace) //todo spawn new object instead
        {
            PlaceObject();
            // var newTransform = lastTargetTransform;
            // newTransform.Basis.Y = new Vector3(0, 1f, 0);
            // newTransform.Basis.X = newTransform.Basis.Y.Cross(newTransform.Basis.Z).Normalized();
            // newTransform.Basis.Z = newTransform.Basis.X.Cross(newTransform.Basis.Y).Normalized();
            //
            // var camTransform = cameraNode.Transform;
            // var userFeetTransform = new Transform3D
            // {
            //     origin = camTransform.Origin
            // };
            // userFeetTransform.Origin.Y = 0;
            //
            // userFeetTransform.Basis.Y = new Vector3(0, 1f, 0);
            // userFeetTransform.Basis.X = userFeetTransform.Basis.Y.Cross(camTransform.Basis.Z).Normalized();
            // userFeetTransform.Basis.Z = userFeetTransform.Basis.X.Cross(userFeetTransform.Basis.Y).Normalized();
            //
            // originNode.GlobalTransform = newTransform * userFeetTransform.Inverse();
        }

        isPlacing = false;
        ArcRay.Visible = false;
        Target.Visible = false;
    }

    private void HandleButtonPressed(double delta)
    {
        if (!isPlacing)
        {
            isPlacing = true;
            ArcRay.Visible = true;
            Target.Visible = true;
            objectRotation = 0.0f;
        }

        var space = PhysicsServer3D.BodyGetSpace(GetRid());
        var state = PhysicsServer3D.SpaceGetDirectState(space);
        var query = new PhysicsShapeQueryParameters3D();

        query.CollisionMask = CollisionMask;
        query.Margin = SafeMargin;
        query.ShapeRid = CollisionShape.GetRid();

        var shapeTransform = new Transform3D(Basis, new Vector3(0, GetObjectHeight() / 2.0f, 0.0f));
        var arcRayGlobalTransform = ArcRay.GlobalTransform;
        var targetGlobalOrigin = arcRayGlobalTransform.Origin;
        var down = Vector3.Down / ws;

        var castLength = 0f;
        var fineTune = 1f;
        var hitSomething = false;
        float maxSlopeCos = Mathf.Cos(Mathf.DegToRad(MaxSlope));

        for (var i = 1; i < 26; i++)
        {
            float newCastLength = castLength + (stepSize / fineTune);
            var globalTarget = new Vector3(0, 0, -newCastLength);
            float t = globalTarget.Z / Strength;
            float t2 = t * t;
            globalTarget = arcRayGlobalTransform * globalTarget;
            globalTarget += down * t2;
            query.Transform = new Transform3D(Basis, globalTarget) * shapeTransform;
            Array<Dictionary> intersectResult = state.IntersectShape(query, 10);
            if (intersectResult.Count == 0) // no collision
            {
                castLength = newCastLength;
                targetGlobalOrigin = globalTarget;
            }
            else if (fineTune <= 16)
            {
                fineTune *= 2.0f;
            }
            else // collision
            {
                HandleCollision(globalTarget, intersectResult, state, maxSlopeCos, ref targetGlobalOrigin, ref castLength);
                hitSomething = true;
                break;
            }
        }

        var arcRayMaterial = ArcRay.GetSurfaceOverrideMaterial(0) as ShaderMaterial;
        arcRayMaterial?.SetShaderParameter("scale_t", 1.0 / Strength);
        arcRayMaterial?.SetShaderParameter("ws", ws);
        arcRayMaterial?.SetShaderParameter("length", castLength);
        if (hitSomething)
        {
            var color = ColorCanBePlaced;
            var normal = Vector3.Up;
            if (isOnFloor)
            {
                normal = floorNormal;
                canPlace = true;
            }
            else
            {
                if (IsPlaceMode()) // if there is no object to place then we are in selectionMode
                    color = ColorCantBePlaced; // we are in place mode
                canPlace = false;
            }

            objectRotation += (float)(delta * Controller.GetVector2(RotationAction).X * -4.9f);
            var targetBasis = Basis;
            targetBasis.Z = new Vector3(arcRayGlobalTransform.Basis.Z.X, 0.0f, arcRayGlobalTransform.Basis.Z.Z).Normalized();
            targetBasis.Y = normal;
            targetBasis.X = targetBasis.Y.Cross(targetBasis.Z);
            targetBasis.Z = targetBasis.X.Cross(targetBasis.Y);

            targetBasis = targetBasis.Rotated(normal, objectRotation);
            lastTargetTransform.Basis = targetBasis;
            lastTargetTransform.Origin = targetGlobalOrigin + new Vector3(0.0f, 0.001f, 0.0f);
            Target.GlobalTransform = lastTargetTransform;

            arcRayMaterial?.SetShaderParameter(MIX_COLOR, color);
            if (Target.GetSurfaceOverrideMaterial(0) is BaseMaterial3D targetMaterial)
                targetMaterial.AlbedoColor = color;
            Target.Visible = canPlace;
        }
        else
        {
            HandleUnHover();

            oldBodies.Clear();
            canPlace = false;
            Target.Visible = false;
            arcRayMaterial?.SetShaderParameter(MIX_COLOR, ColorNoCollision);
        }
    }

    private static bool IsPlaceMode()
    {
        return !string.IsNullOrEmpty(BuildViewModel.Instance.UISelection);
    }

    private void HandleCollision(Vector3 globalTarget, IEnumerable<Dictionary> intersectResult, PhysicsDirectSpaceState3D state, float maxSlopeCos, ref Vector3 targetGlobalOrigin, ref float castLength)
    {
        var collidedAt = targetGlobalOrigin;
        List<ISelectable> collidingBodies = intersectResult.Select(dictionary => dictionary[COLLIDER]).Select(b => b.As<Node>().GetParent()).OfType<ISelectable>().ToList();
        // if (globalTarget.Y > targetGlobalOrigin.Y)
        if (collidingBodies.Count > 0)
        {
            if (!IsPlaceMode() && ObjectToPlace == null)
            {
                foreach (var selectable in oldBodies.Except(collidingBodies))
                {
                    selectable.UnHover();
                }

                foreach (var selectable in collidingBodies)
                {
                    selectable.Hover();
                }

                oldBodies.Clear();
                oldBodies.AddRange(collidingBodies);

                if (Controller.IsButtonPressed(SelectionButtonAction))
                {
                    HandleSelection();
                }
            }

            isOnFloor = false;
        }
        else
        {
            //collision with ground
            var rayQuery = new PhysicsRayQueryParameters3D();
            rayQuery.From = targetGlobalOrigin + Vector3.Up * 0.5f * GetObjectHeight();
            rayQuery.To = targetGlobalOrigin - Vector3.Up * 1.1f * GetObjectHeight();
            rayQuery.CollisionMask = CollisionMask;

            var intersects = state.IntersectRay(rayQuery);
            if (intersects.Count == 0)
            {
                isOnFloor = false;
            }
            else
            {
                floorNormal = (Vector3)intersects["normal"];
                float dot = floorNormal.Dot(Vector3.Up);
                isOnFloor = dot > maxSlopeCos;
                var intersectPosition = (Vector3)intersects["position"];
                var diff = collidedAt - intersectPosition;
                if (diff.Length() > 0.1)
                    collidedAt = intersectPosition;
                uint colliderMask = 0;
                if (intersects[COLLIDER].As<Node3D>() is CollisionObject3D collisionObject3D)
                    colliderMask = collisionObject3D.CollisionLayer;
                else if (intersects[COLLIDER].As<Node3D>() is CsgShape3D mesh3D)
                    colliderMask = mesh3D.CollisionLayer;

                if ((ValidArcRayMask & colliderMask) == 0) //if not valid_teleport_mask & collider_mask:
                {
                    isOnFloor = false;
                }
            }
        }

        castLength += (collidedAt - targetGlobalOrigin).Length();
        targetGlobalOrigin = collidedAt;
    }

    private void HandleUnHover()
    {
        foreach (var oldBody in oldBodies)
        {
            oldBody.UnHover();
        }
    }

    private void HandleSelection()
    {
        if (oldBodies.FirstOrDefault() is { } selectable)
        {
            selectable.Select();
        }
        else
        {
            HandleDeSelection();
        }
    }

    private void HandleDeSelection()
    {
        foreach (var selectable in BuildViewModel.Instance.CurrentSelected)
        {
            selectable.DeSelect();
        }
    }

    private void PlaceObject()
    {
        if (IsPlaceMode() && ObjectToPlace != null)
        {
            Figure.RemoveChild(ObjectToPlace);
            GetTree().Root.AddChild(ObjectToPlace);
            ObjectToPlace.Owner = GetTree().Root;
            ObjectToPlace.Transform = lastTargetTransform;
            if (ObjectToPlace is Lantern lantern)
                lantern.Collision = true;
            if (PlaceholderForNoObject != null)
                PlaceholderForNoObject.Visible = true;
            ObjectToPlace = null;
            BuildViewModel.Instance.UISelection = "";
        }
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (GetParentOrNull<XRController3D>() == null)
            warnings.Add("Parent of this node needs to be a " + nameof(XRController3D));
        if (GetNodeOrNull<MeshInstance3D>("ArcRay") == null)
            warnings.Add("this Node needs a child named 'ArcRay' of type: " + nameof(MeshInstance3D));
        if (GetNodeOrNull<MeshInstance3D>("Target") == null)
            warnings.Add("this Node needs a child named 'Target' of type: " + nameof(MeshInstance3D));
        if (GetNodeOrNull<Node3D>("Target/ObjectFigure") == null)
            warnings.Add("this Node needs a child named 'Target/ObjectFigure' of type: " + nameof(Node3D));
        if (GetNodeOrNull<MeshInstance3D>("Target/ObjectFigure/ObjectHolder") == null)
            warnings.Add("this Node needs a child named 'Target/ObjectFigure/ObjectHolder' of type: " + nameof(MeshInstance3D));
        return warnings.ToArray();
    }

    private float GetObjectHeight()
    {
        return .5f;
    }

    public bool IsXRClass(string name)
    {
        return Name == "XRToolsFunctionPlace";
    }

    public override void _Process(double delta)
    {
    }
}