using Godot;
using System;

public partial class shyPlant : CharacterBody2D
{
	public char cardinalDirection;
	private const float walkSpeed = 40;
	private const float runSpeed = 200;
	private Boolean playerInRange = false;
	private void body_entered(Node2D body) // Method heater written by AI because even though I told it not to show me code it still did.
	{
		if (body is Player) // == and .equals don't work in C#
		{
			playerInRange = true;
		}
	}
	private void body_exited(Node2D body)
	{
		if (body is Player)
		{
			playerInRange = false;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) // Apparently if statements have to be in methods
	{
		Velocity = new Vector2(1, 1);
		if (playerInRange)
		{
			Velocity *= runSpeed;
		}
		MoveAndSlide();
	}
}
