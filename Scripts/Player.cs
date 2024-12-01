using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 10.0f;
	public const float JumpVelocity = 8f;
	public const float Sensitivity = 1.5f;
	public int SelectedItem = 0;

	public Block CurrentBlock = null;
	Vector3I CurrentBlockPosition;
	float MiningSpeed = 2f;
	float MiningProgress = 0f;
	float BlockHealth = 10f;
	float MiningTimer = 0f;

	public Camera3D Camera;
	public RayCast3D RayCast;
	public ItemList Hotbar;
	public Timer ActionTimer;
	public ProgressBar ProgBar;
	public Node3D Blocks;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = 16;

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Camera = GetNode<Camera3D>("Camera3D");
		RayCast = GetNode<RayCast3D>("Camera3D/RayCast3D");
		Hotbar = GetNode<ItemList>("/root/World/UI/Hotbar");
		ActionTimer = GetNode<Timer>("ActionTimer");
		ProgBar = GetNode<ProgressBar>("/root/World/UI/ProgressBar");
		Blocks = GetNode<Node3D>("/root/World/Blocks");


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

    public override void _Process(double delta)
    {
		if (Input.IsActionJustPressed("lmb") || (Input.IsActionPressed("lmb")) && CurrentBlock == null)
        {
            if (RayCast.IsColliding())
            {
                if (RayCast.GetCollider().HasMethod("DestroyBlock"))
                {
                    Vector3 blockPosition = RayCast.GetCollisionPoint() - RayCast.GetCollisionNormal();
                    var gridMap = RayCast.GetCollider() as GridMap;
					int blockIndex = gridMap.GetBlock(blockPosition);

                    if (blockIndex == -1)
						return;

					if (CurrentBlock == null || CurrentBlock != Blocks.GetChild<Block>(blockIndex))
					{
						CurrentBlock = Blocks.GetChild<Block>(blockIndex);
                        ProgBar.Visible = true;
						MiningProgress = 0f;
						MiningTimer = 0f;
						BlockHealth = 10f;
						MiningSpeed = 20f;
                    }
                }
            }
        }

		if (Input.IsActionPressed("lmb") && CurrentBlock != null)
		{
			MiningTimer += (float)delta * MiningSpeed;
			ProgBar.Value = MiningTimer;

			if (MiningTimer >= BlockHealth)
			{
				if (RayCast.IsColliding())
				{
					if (RayCast.GetCollider().HasMethod("DestroyBlock"))
					{
						Vector3 blockPosition = RayCast.GetCollisionPoint() - RayCast.GetCollisionNormal();
						var gridMap = RayCast.GetCollider() as GridMap;

							gridMap.DestroyBlock(blockPosition);
							ProgBar.Value = 0;
							ProgBar.Visible = false;
							CurrentBlock = null;
							//InputDelay(delta);
                    }
				}
			}
		}

		if (Input.IsActionJustReleased("lmb"))
		{
			CurrentBlock = null;
            MiningProgress = 0f;
			MiningTimer = 0f;
			ProgBar.Value = 0;
            ProgBar.Visible = false;
        }

        if (Input.IsActionJustPressed("rmb"))
        {
            if (RayCast.IsColliding())
            {
                if (RayCast.GetCollider().HasMethod("PlaceBlock"))
                {
                    Vector3 blockPosition = RayCast.GetCollisionPoint() + RayCast.GetCollisionNormal();
                    var gridMap = RayCast.GetCollider() as GridMap;
                    gridMap.PlaceBlock(blockPosition, SelectedItem);
                }
            }
        }

    }

    public override void _Input(InputEvent @event)
    {
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
                Camera.Rotation = new Vector3(Mathf.Clamp(Camera.Rotation.X - motion.Relative.Y / 1000 * Sensitivity, -1.5f, 1.5f), Camera.Rotation.Y, Camera.Rotation.Z);
            }
		}

    }

	public float CalculateBreakTime(float BreakPace)
	{
		return (BreakPace * 1);
	}

	public void MineBlockStart(GridMap gridMap, Vector3 WorldCoordinate)
	{
		int blockIndex = gridMap.GetBlock(WorldCoordinate);
		Block InteractBlock = Blocks.GetChild<Block>(blockIndex);

        ActionTimer.WaitTime = CalculateBreakTime(InteractBlock.BlockStat.BreakPace);
		ActionTimer.Start();
	}

	public void InputDelay(double delta)
	{
		float delay = 6f;
		float delayStart = 0;
		while (delay > 0)
		{
			delayStart += (float)delta * 2f;

			if (delayStart == delay)
				break;
		}
	}
}
