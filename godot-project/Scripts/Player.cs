using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class Player : CharacterBody2D
{
	private const float Speed = 200.0f;
	// Called when the node enters the scene tree for the first time.
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 velocity = Vector2.Zero;
		velocity.X = Input.GetAxis("move_left", "move_right");
		velocity.Y = Input.GetAxis("move_up", "move_down");
		Velocity = velocity.Normalized() * Speed;
		GD.Print(Velocity);
		MoveAndSlide();
	}
}
