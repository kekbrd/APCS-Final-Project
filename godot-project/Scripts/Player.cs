using Godot;
using System;
using System.Runtime.ConstrainedExecution;

public partial class Player : CharacterBody2D
{
	private const float speed = 50;
	public char cardinalDirection;
    private AnimatedSprite2D sprite;
    private Vector2 prevVelocity = Vector2.Zero; 
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		cardinalDirection = 'S';
        sprite = GetNode<AnimatedSprite2D>("Sprite");
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) // PhysicsProcess is used for movement, not Process.
	{
        Velocity = Vector2.Zero; // Velocity is built in for characterbody, it's a Vector2 which has X and Y values
        Velocity = Input.GetVector("move_left", "move_right", "move_up", "move_down") * speed; // Got this from AI. Up is negative y, down is positive.
        if (Velocity != prevVelocity)
        {
            updateCardinalDirection();
        }
        if (Velocity == Vector2.Zero)
        {
            setIdleAnimation();
        }
        else
        {
            setWalkAnimation();
        }
        prevVelocity = Velocity;
        MoveAndSlide();
	}

    private void updateCardinalDirection()
    {
        if (Velocity != Vector2.Zero)
        {
            if ((Velocity.Angle() < (-1 * Mathf.Pi / 4)) && (Velocity.Angle() > (-3 * Mathf.Pi / 4)))
            {
                cardinalDirection = 'N';
            }
            else if ((Velocity.Angle() <= (Mathf.Pi / 4)) && (Velocity.Angle() >= (-1 * Mathf.Pi / 4)))
            {
                cardinalDirection = 'E';
            }
            else if ((Velocity.Angle() > (Mathf.Pi / 4)) && (Velocity.Angle() < (3 * Mathf.Pi / 4)))
            {
                cardinalDirection = 'S';
            }
            else
            {
                cardinalDirection = 'W';
            }
        }
    }

    private void setWalkAnimation()
    {
        if ((Velocity.Angle() < (-1 * Mathf.Pi / 4)) && (Velocity.Angle() > (-3 * Mathf.Pi / 4)))
        {
            sprite.Play("walk_up");
        }
        else if ((Velocity.Angle() <= (Mathf.Pi / 4)) && (Velocity.Angle() >= (-1 * Mathf.Pi / 4)))
        {
            sprite.Play("walk_right");
        }
        else if ((Velocity.Angle() > (Mathf.Pi / 4)) && (Velocity.Angle() < (3 * Mathf.Pi / 4)))
        {
            sprite.Play("walk_down");
        }
        else
        {
            sprite.Play("walk_left");
        }
    }

    private void setIdleAnimation()
    {
        if (cardinalDirection == 'N')
        {
            sprite.Play("idle_up");
        }
        else if (cardinalDirection == 'E')
        {
            sprite.Play("idle_right");
        }
        else if (cardinalDirection == 'S')
        {
            sprite.Play("idle_down");
        }
        else
        {
            sprite.Play("idle_left");
        }
    }
}
