using System;
using System.Linq;
using System.Text.Json;
using Godot;

namespace Leipzig3DGodot.Scripts.UX;

///<author email="aron.schaub@stud.htwk-leipzig.de">Aron Schaub</author>
[Tool]
public partial class Camera : Camera3D
{
	[Export]
	private bool cameraEnabled = true;

	[ExportGroup("Edge Scroll", "edgeScroll")]
	[Export]
	private bool edgeScrollEnabled = true;

	[Export]
	private float edgeScrollDetectionArea = 0.06f;

	[Export]
	private float edgeScrollTime = 5;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float edgeScrollSpeed = 1;

	[ExportGroup("Movement", "movement")]
	[Export]
	private bool movementEnabled = true;

	[Export]
	private float movementTime = 5;

	[Export]
	private float movementSpeed = 1;

	[Export]
	private float movementFastSpeedWaitTime = 0.5f;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float movementSpeedMultiplier = 1.5f;

	[ExportGroup("Rotation", "rotation")]
	[Export]
	private bool rotationEnabled = true;

	[Export]
	private float rotationTime = 5;

	[Export]
	private float rotationSpeed = 3;

	[Export]
	private float rotationFastSpeedWaitTime = 2;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float rotationSpeedMultiplier = 2;

	[ExportGroup("Pitch", "Pitch")]
	[Export]
	private bool pitchEnabled = true;

	[Export]
	private float pitchTime = 5;

	[Export]
	private float pitchSpeed = 3;

	[Export]
	private float pitchMinAngle = -90;

	[Export]
	private float pitchMaxAngle = -20;

	[Export]
	private float pitchFastSpeedWaitTime = 2;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float pitchSpeedMultiplier = 2;

	[ExportGroup("Zooming", "zooming")]
	[Export]
	private bool zoomingEnabled = true;

	[Export]
	private float zoomingTime = 5;

	[Export]
	private float zoomingSpeed = 1;

	[Export]
	private float zoomingMinDistance = 10;

	[Export]
	private float zoomingMaxDistance = 80;

	[Export]
	private float zoomingFastSpeedWaitTime = 2;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float zoomingSpeedMultiplier = 2;

	[ExportGroup("Looking", "looking")]
	[Export]
	private bool lookingEnabled = true;

	[Export]
	private float lookingSpeed = 8;

	[Export]
	private float lookingFastSpeedWaitTime = 2;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float lookingSpeedMultiplier = 2;

	[ExportGroup("Panning", "panning")]
	[Export]
	private bool panningEnabled = true;

	[Export]
	private float panningSpeed = 50;

	[Export]
	private float panningFastSpeedWaitTime = 2;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float panningSpeedMultiplier = 2;

	[ExportGroup("Scrolling", "scrolling")]
	[Export]
	private bool scrollingEnabled = true;

	[Export]
	private float scrollingSpeed = 50;

	[Export]
	private float scrollingFastSpeedWaitTime = 2;

	[Export(PropertyHint.Range, "0.01,10,or_greater")]
	private float scrollingSpeedMultiplier = 2;

// Disable limits in your own risk, this might break the camera.
	[ExportGroup("Limits", "limits")]
	[Export]
	private bool limitsEnabled = true;

	[Export]
	private Rect2 limitsRect = new Rect2(new Vector2(-2450, -2450), new Vector2(4900, 4900));

	[ExportGroup("Perspective Panning", "perspectivePanning")]
	[Export]
	private bool perspectivePanningEnabled;

	[Export]
	private float perspectivePanningSpeed = 10;

	[ExportGroup("Option", "option")]
	[Export]
	private bool optionTargetZoom = true;

	[Export]
	private bool optionZoomToCursor;

	[Export]
	private bool optionDisableEdgeScrollingWhenUsingMouse = true;

	[Export]
	private bool optionInvertZooming;

	[Export]
	private bool optionInvertPanning;

	[Export]
	private bool optionInvertRotationKeys;

	[Export]
	private bool optionInvertRotation;

	[Export]
	private bool optionInvertPitch;

	[Export]
	private float mouseSensitivity = 0.03f;


	private Vector3 newTranslation;
	private Vector3 newRotation;
	private Vector3 newPitchRotation;
	private Vector3 newZoom;

//float _translation_timer = Timer.new()
//float _rotation_timer = Timer.new()
//float _Pitch_timer = Timer.new()
//float _zooming_timer = Timer.new()
//float _panning_timer = Timer.new()
//float _looking_timer = Timer.new()
//float _scrolling_timer = Timer.new()

	private bool mouseIn = true;

	private bool isPanning = false;

	private float pitch = Mathf.DegToRad(-45);
	private Camera3D cameraZoom;
	private Node3D cameraTilt;

	public override void _Ready()
	{
		base._Ready();
		//TODO set focus
		cameraZoom = GetNode<Camera3D>("CameraTilt/CameraZoom");
		cameraTilt = GetNode<Node3D>("CameraTilt");
		if (!Engine.IsEditorHint())
		{
			cameraZoom.Current = true;
		}

//		_setup_timer(_translation_timer, movement_fast_speed_wait_time, "move")
//		_setup_timer(_rotation_timer, rotation_fast_speed_wait_time, "rotate")
//		_setup_timer(_Pitch_timer, Pitch_fast_speed_wait_time, "tilt")
//		_setup_timer(_zooming_timer, zooming_fast_speed_wait_time, "zoom")
//		_setup_timer(_looking_timer, looking_fast_speed_wait_time, "look")
//		_setup_timer(_panning_timer, panning_fast_speed_wait_time, "pan")
//		_setup_timer(_scrolling_timer, scrolling_fast_speed_wait_time, "scroll")

//		add_child(_translation_timer)
//		add_child(_rotation_timer)
//		add_child(_Pitch_timer)
//		add_child(_zooming_timer)
//		add_child(_looking_timer)
//		add_child(_panning_timer)
//		add_child(_scrolling_timer)

		newTranslation = GlobalTransform.Origin;
		newRotation = Rotation;
		newPitchRotation = cameraTilt.Rotation;
		newZoom = cameraZoom.Transform.Origin;
	}

	public override void _Input(InputEvent inputEvent)
	{
		base._Input(inputEvent);


		if (Engine.IsEditorHint() || !cameraEnabled)
			return;

		//(2 * int(!option_invert_zooming) - 1) //* invert_zooming
		// option_invert_zooming =                  false   true
		// !option_invert_zooming =                 true    false
		// int(!option_invert_zooming) =            1       0
		// 2 * int(!option_invert_zooming) =        2       0
		// (2 * int(!option_invert_zooming) - 1) =  1       -1
		// option_invert_zooming ? -1 : 1; 
		int invertedZooming = optionInvertZooming ? -1 : 1;

		// GD.Print(inputEvent.GetType());

		switch (inputEvent)
		{
			case InputEventMouseButton mouseEvent when !optionDisableEdgeScrollingWhenUsingMouse:
			{
				if (new[] { MouseButton.Left, MouseButton.Right }.Contains(mouseEvent.ButtonIndex) && inputEvent.IsPressed())
					edgeScrollEnabled = false;
				if (new[] { MouseButton.Left, MouseButton.Right }.Contains(mouseEvent.ButtonIndex) && !inputEvent.IsPressed())
					edgeScrollEnabled = true;
				break;
			}
			case InputEventMouseButton mouseEvent:
			{
				if (scrollingEnabled)
				{
					// GD.Print(mouseEvent.ButtonIndex);
					switch (mouseEvent.ButtonIndex)
					{
						case MouseButton.WheelDown:
							newZoom += Vector3.Back * scrollingSpeed * mouseSensitivity * invertedZooming;
							if (optionTargetZoom)
								newPitchRotation += Vector3.Right * -Mathf.DegToRad(pitchSpeed) * scrollingSpeed / 2 * mouseSensitivity * invertedZooming; //* invert_zooming
							break;
						case MouseButton.WheelUp:
							newZoom += Vector3.Back * -scrollingSpeed * mouseSensitivity * invertedZooming;
							break;
					}

					if (optionTargetZoom)
						newPitchRotation += Vector3.Right * Mathf.DegToRad(pitchSpeed) * scrollingSpeed / 2 * mouseSensitivity * invertedZooming;
				}

				break;
			}
			case InputEventMouseMotion mouseMotionEvent:
			{
				switch (mouseMotionEvent.ButtonMask)
				{
					case MouseButtonMask.Left when lookingEnabled:
						newRotation += Vector3.Up * -mouseMotionEvent.Relative.X * Mathf.DegToRad(lookingSpeed) * mouseSensitivity * invertedZooming;
						newPitchRotation += Vector3.Right * -mouseMotionEvent.Relative.Y * Mathf.DegToRad(lookingSpeed) * mouseSensitivity * invertedZooming;
						pitch += -mouseMotionEvent.Relative.Y * Mathf.DegToRad(lookingSpeed) * mouseSensitivity * invertedZooming;
						break;
					case MouseButtonMask.Right when perspectivePanningEnabled:
						newTranslation += -(GlobalTransform.Basis.Z * mouseMotionEvent.Relative.Y + GlobalTransform.Basis.X * mouseMotionEvent.Relative.X) * perspectivePanningSpeed * mouseSensitivity * invertedZooming;
						break;
					case MouseButtonMask.Right:
					{
						if (panningEnabled)
							newTranslation += -(GlobalTransform.Basis.Z * mouseMotionEvent.Relative.Normalized().Y + GlobalTransform.Basis.X * mouseMotionEvent.Relative.Normalized().X) * panningSpeed * mouseSensitivity * invertedZooming;
						break;
					}
				}

				break;
			}
		}
	}

	public void OnFloatingOriginSet(float x, float y, float z)
	{
		// GD.Print("_on_floating_origin_set");
		newTranslation -= new Vector3(x, y, z);
	}

	public void _process(float delta)
	{
		if (Engine.IsEditorHint() || !cameraEnabled)
			return;
		int invertRotationKeys = optionInvertRotationKeys ? -1 : 1;
		int invertPitch = optionInvertPitch ? -1 : 1;
		int invertZooming = optionInvertZooming ? -1 : 1;

		cameraZoom.Far = Far;
		var mousePosition = GetViewport().GetMousePosition();
		var visibleRect = GetViewport().GetVisibleRect();
		if (edgeScrollEnabled)
		{
			// GD.Print("edgeScrollEnabled");
			if (mouseIn && !isPanning)
			{
				if (mousePosition.X < (int)visibleRect.Size.X * edgeScrollDetectionArea)
					newTranslation -= GlobalTransform.Basis.X;
				if (mousePosition.X > visibleRect.Size.X - visibleRect.Size.X * edgeScrollDetectionArea)
					newTranslation += GlobalTransform.Basis.X;
				if (mousePosition.Y < (int)visibleRect.Size.Y * edgeScrollDetectionArea)
					newTranslation -= GlobalTransform.Basis.Z;
				if (mousePosition.Y > visibleRect.Size.Y - visibleRect.Size.Y * edgeScrollDetectionArea)
				{
					newTranslation += GlobalTransform.Basis.Z;
					var globalTransform = GlobalTransform;
					globalTransform.Origin = GlobalTransform.Origin.Lerp(newTranslation * edgeScrollSpeed, delta * edgeScrollTime);
					if (limitsEnabled)
					{
						globalTransform.Origin = ClampCamera(limitsRect, GlobalTransform.Origin);
					}

					GlobalTransform = globalTransform;
				}
			}
		}

		if (movementEnabled)
		{
			// GD.Print("movementEnabled");

			if (IsWindowFocused && !isPanning)
			{
//			if( Input.is_action_just_pressed("camera_forward")){
//				if( _translation_timer.is_stopped()){
//					_translation_timer.start()
//			if( Input.is_action_just_pressed("camera_back")){
//				if( _translation_timer.is_stopped()){
//					_translation_timer.start()
//			if( Input.is_action_just_pressed("camera_left")){
//				if( _translation_timer.is_stopped()){
//					_translation_timer.start()
//			if( Input.is_action_just_pressed("camera_right")){
//				if( _translation_timer.is_stopped()){
//					_translation_timer.start()
				if (Input.IsActionPressed("camera_forward"))
					newTranslation -= GlobalTransform.Basis.Z;

				if (Input.IsActionPressed("camera_back"))
					newTranslation += GlobalTransform.Basis.Z;

				if (Input.IsActionPressed("camera_left"))
					newTranslation -= GlobalTransform.Basis.X;

				if (Input.IsActionPressed("camera_right"))
					newTranslation += GlobalTransform.Basis.X;
//			if( Input.is_action_just_released("camera_forward")){
//				_translation_timer.stop()
//				movement_speed /= movement_speed_multiplier
//				movement_time /= movement_speed_multiplier
//			if( Input.is_action_just_released("camera_back")){
//				_translation_timer.stop()
//				movement_speed /= movement_speed_multiplier
//				movement_time /= movement_speed_multiplier
//			if( Input.is_action_just_released("camera_left")){
//				_translation_timer.stop()
//				movement_speed /= movement_speed_multiplier
//				movement_time /= movement_speed_multiplier
//			if( Input.is_action_just_released("camera_right")){
//				_translation_timer.stop()
//				movement_speed /= movement_speed_multiplier
//				movement_time /= movement_speed_multiplier

				var globalTransform = GlobalTransform;
				globalTransform.Origin = GlobalTransform.Origin.Lerp(newTranslation * movementSpeed, delta * movementTime);
				if (limitsEnabled)
				{
					globalTransform.Origin = ClampCamera(limitsRect, globalTransform.Origin);
				}

				GlobalTransform = globalTransform;
			}
		}

		if (rotationEnabled)
		{
			// GD.Print("rotationEnabled");
			if (Input.IsActionPressed("camera_rotate_x"))
				newRotation += Vector3.Up * Mathf.DegToRad(rotationSpeed) * (invertRotationKeys);
			if (Input.IsActionPressed("camera_rotate_-x"))
				newRotation += Vector3.Up * -Mathf.DegToRad(rotationSpeed) * (invertRotationKeys);
			newRotation.Y = Mathf.Wrap(newRotation.Y, Mathf.DegToRad(180), Mathf.DegToRad(-180));
			Rotation =  Quaternion.FromEuler(Rotation).Slerp(Quaternion.FromEuler(newRotation), delta * rotationTime).GetEuler();
		}

		if (pitchEnabled)
		{
			// GD.Print("PitchEnabled");
			if (Input.IsActionPressed("camera_tilt_y"))
			{
				newPitchRotation += Vector3.Right * Mathf.DegToRad(pitchSpeed) * invertPitch;
				pitch += Mathf.DegToRad(pitchSpeed) * invertPitch;
			}

			if (Input.IsActionPressed("camera_tilt_-y"))
			{
				newPitchRotation += Vector3.Right * -Mathf.DegToRad(pitchSpeed) * invertPitch;
				pitch += -Mathf.DegToRad(pitchSpeed) * invertPitch;
			}

			newPitchRotation.X = Mathf.Clamp(newPitchRotation.X, Mathf.DegToRad(pitchMinAngle), Mathf.DegToRad(pitchMaxAngle));
			pitch = Mathf.Clamp(pitch, Mathf.DegToRad(pitchMinAngle), Mathf.DegToRad(pitchMaxAngle));

			cameraTilt.Rotation = Quaternion.FromEuler(cameraTilt.Rotation).Slerp(Quaternion.FromEuler(newPitchRotation), delta * pitchTime).GetEuler();
		}

		if (zoomingEnabled)
		{
			// GD.Print("zoomingEnabled");
			if (Input.IsActionPressed("camera_zoom_in"))
			{
				// GD.Print("camera_zoom_in");
				newZoom += Vector3.Back * -zoomingSpeed * invertZooming;
				if (optionTargetZoom)
					newPitchRotation += Vector3.Right * Mathf.DegToRad(pitchSpeed) * zoomingSpeed / 2 * invertZooming;
			}

			if (Input.IsActionPressed("camera_zoom_out"))
			{
				// GD.Print("camera_zoom_out");
				newZoom += Vector3.Back * zoomingSpeed * invertZooming;
				if (optionTargetZoom)
					newPitchRotation += Vector3.Right * -Mathf.DegToRad(pitchSpeed) * zoomingSpeed / 2 * invertZooming;
			}

			if (optionTargetZoom)
				newPitchRotation.X = Mathf.Clamp(newPitchRotation.X, pitchMinAngle, pitch);

			Vector3? currentTranslation = optionZoomToCursor ? CastRayTo(mousePosition) : null;

			newZoom.Z = Mathf.Clamp(newZoom.Z, zoomingMinDistance, zoomingMaxDistance);

			cameraZoom.Position = cameraZoom.Position.Lerp(newZoom, delta * zoomingTime);
			if (currentTranslation.HasValue && currentTranslation != Vector3.Inf)
			{
				Vector3? newTranslation = CastRayTo(mousePosition);
				if (newTranslation.HasValue && newTranslation != Vector3.Inf)
				{
					this.newTranslation += (currentTranslation - newTranslation).Value;
				}
			}
		}
	}

	public bool IsWindowFocused { get; set; } = true;

	private static Vector3 ClampCamera(Rect2 rect, Vector3 position)
	{
		return new Vector3(Mathf.Clamp(position.X, rect.Position.X, rect.End.X), position.Y, Mathf.Clamp(position.Z, rect.Position.Y, rect.End.Y));
	}

	private Vector3? CastRayTo(Vector2 position)
	{
		var camera = GetViewport().GetCamera3D();
		var from = camera.ProjectRayOrigin(position);
		var to = from + camera.ProjectRayNormal(position) * camera.Far;
		var queryParameters = new PhysicsRayQueryParameters3D();
		queryParameters.From = from;
		queryParameters.To = to;
		if (GetWorld3D().DirectSpaceState.IntersectRay(queryParameters).TryGetValue("position", out var ray))
			return ray.AsVector3();
		return null;
	}

	// public void timeout()
	// {
	// 	
	// }
//public void _timeout(movement):
//	match movement:
//		"move":
//			movement_speed *= movement_speed_multiplier
//			movement_time *= movement_speed_multiplier
//			print(movement_speed)
//
//public void _setup_timer(timer: Timer, wait_time: float, type: String) -> void:
//	timer.Wait_time = wait_time
//	timer.one_shot = true
//	timer.connect("timeout", self, "_timeout", [type])

	// private void _notification(int what)
	// {
	//     switch (what)
	//     {
	//         case NOTIFICATION_WM_MOUSE_ENTER:
	//             _mouse_in = true;
	//         case NOTIFICATION_WM_MOUSE_EXIT:
	//             _mouse_in = false;
	//     }
	// }
}
