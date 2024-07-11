using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 10.0f;
	public const float JumpVelocity = 8f;
	public const float Sensitivity = 1.5f;
	public int SelectedItem = 0;

	public Camera3D Camera;
	public RayCast3D RayCast;
	public ItemList Hotbar;
	public Timer ActionTimer;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = 16;

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Camera = GetNode<Camera3D>("Camera3D");
		RayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		Hotbar = GetNode<ItemList>("/root/World/UI/Hotbar");
		ActionTimer = GetNode<Timer>("ActionTimer");

		Hotbar.Select(0);
	}

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_foward", "move_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

    public override void _Input(InputEvent @event)
    {
        /*
		if (Input.IsActionJustPressed("lmb"))
		{
			if (RayCast.IsColliding())
			{
				if (RayCast.GetCollider().HasMethod("DestroyBlock"))
				{
					var gridMap = RayCast.GetCollider() as GridMap;
					gridMap.DestroyBlock(RayCast.GetCollisionPoint() - RayCast.GetCollisionNormal());
				}
			}
		}
		*/

        if (Input.IsActionJustPressed("lmb"))
        {
            if (RayCast.IsColliding())
            {
                if (RayCast.GetCollider().HasMethod("DestroyBlock"))
                {
                    var gridMap = RayCast.GetCollider() as GridMap;
					GD.Print(gridMap.GetBlock(RayCast.GetCollisionPoint() - RayCast.GetCollisionNormal()));
					gridMap.DestroyBlock(RayCast.GetCollisionPoint() - RayCast.GetCollisionNormal());
                }
            }
        }

        if (Input.IsActionJustPressed("rmb"))
        {
            if (RayCast.IsColliding())
            {
                if (RayCast.GetCollider().HasMethod("PlaceBlock"))
                {
                    var gridMap = RayCast.GetCollider() as GridMap;	
					gridMap.PlaceBlock(RayCast.GetCollisionPoint() + RayCast.GetCollisionNormal(), SelectedItem);
                }
            }
        }

		if (Input.IsActionJustPressed("one"))
		{
			SelectedItem = 0;
			Hotbar.Select(0);
		}
        if (Input.IsActionJustPressed("two"))
        {
            SelectedItem = 1;
            Hotbar.Select(1);
        }
        if (Input.IsActionJustPressed("three"))
        {
            SelectedItem = 2;
            Hotbar.Select(2);
        }
        if (Input.IsActionJustPressed("four"))
        {
            SelectedItem = 3;
            Hotbar.Select(3);
        }
        if (Input.IsActionJustPressed("five"))
        {
            SelectedItem = 4;
            Hotbar.Select(4);
        }

        if (Input.IsActionJustPressed("esc"))
        {
            if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventMouseMotion)
		{
			InputEventMouseMotion motion = (@event as InputEventMouseMotion);
			if (motion != null)
			{
                Rotation = new Vector3(Rotation.X, Rotation.Y - motion.Relative.X / 1000 * Sensitivity, Rotation.Z);
                Camera.Rotation = new Vector3(Mathf.Clamp(Camera.Rotation.X - motion.Relative.Y / 1000 * Sensitivity, -2, 2), Camera.Rotation.Y, Camera.Rotation.Z);
            }
		}

    }

	public float CalculateBreakTime(float BreakPace)
	{
		return (BreakPace * 1);
	}
}
