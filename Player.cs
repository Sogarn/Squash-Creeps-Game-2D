using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	
	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	public Vector2 ScreenSize; // Size of the game window.

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right")){
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left")){
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down")){
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up")){
			velocity.Y -= 1;
		}

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0){
			velocity = velocity.Normalized() * Speed;
			animatedSprite.Play();
		}
		else{
			animatedSprite.Stop();
		}

		Position += velocity * (float)delta;

		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		if (velocity.X != 0){
			// Set walking animation to left-right and flip horizontal if going backwards
			animatedSprite.Animation = "walk";
			animatedSprite.FlipV = false;
			animatedSprite.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0){
			// Set walking animation to up-down and flip vertical if going up
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.Y > 0;
		}
	}

	// When detecing collision
	private void OnBodyEntered(Node2D body)
	{
		Hide(); // Player disappears after being hit
		EmitSignal(SignalName.Hit);
		// Wait to disable collision until it is safe to do so
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// On node start
	public void Start(Vector2 position)
	{
		Position = position;
		// Show player
		Show();
		// Enable collision
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}
