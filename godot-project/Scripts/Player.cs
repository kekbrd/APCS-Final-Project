using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class Player : CharacterBody2D
{
	private const float speed = 200;
	// Called when the node enters the scene tree for the first time.
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) // PhysicsProcess is used for movement, not Process.
	{
		Velocity = Vector2.Zero;
		Velocity = Input.GetVector("move_left", "move_right", "move_up", "move_down") * speed; // Got this from AI. Up is negative y, down is positive.
		MoveAndSlide();
	}
}
