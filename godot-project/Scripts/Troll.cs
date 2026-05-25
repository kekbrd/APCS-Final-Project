using Godot;
using System;

public partial class Troll : CharacterBody2D
{
	private char cardinalDirection;

    private const float walkSpeed = 30f;
    private const float runSpeed = 150f;

	private bool isChasing = false;
	private bool isAwake = false;

	private Timer idleTimer;
	private Random r = new Random();
    private Player player;
    private AnimatedSprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		idleTimer = GetNode<Godot.Timer>("IdleTimer");
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		player = GetTree().GetFirstNodeInGroup("player") as Player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isAwake && player.getHasPhoto())
		{
			isAwake = true;
			idleTimer.Start();
		}

		if (isChasing && player.getIsAlive())
		{
			Velocity = (player.GlobalPosition - GlobalPosition).Normalized();
			Velocity *= runSpeed;
			setRunAnimation();
		}

		if (!isAwake)
		{
			Velocity = Vector2.Zero;
		}

		MoveAndSlide();
		if (player.getHasPhoto() && GetSlideCollisionCount() > 0)
		{
			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				if (GetSlideCollision(i).GetCollider() is Player)
				{
					player.setIsAlive(false);
					isChasing = false;
					idleTimer.Start(0.01);
				}
			}
		}
	}

	private void on_detection_area_body_entered(Node2D body)
	{
		if (isAwake && body is Player)
		{
			isChasing = true;
		}
	}

	private void on_detection_area_body_exited(Node2D body)
	{
		if (isAwake && body is Player)
		{
			isChasing = true;
		}
	}

	private async void on_idle_timer_timeout()
	{
		if (isAwake && !isChasing)
		{
			cardinalDirection = IdleMovement.setRandomDirection();
			Velocity = IdleMovement.startWalking(cardinalDirection, sprite, walkSpeed);
		
			await ToSignal(GetTree().CreateTimer(3), "timeout"); /* ToSignal converts to be awaitable, needs the timer object and the name of the
            timer object's signal that it's done, name must be exact*/
            if (isAwake && !isChasing) // avoid resetting velocity at inappropriate times
            {
                Velocity = Vector2.Zero;
                IdleMovement.setIdleAnimation(sprite, cardinalDirection);
                idleTimer.Start(r.Next(3, 5));
		    }
		}
	}

	private void setRunAnimation() // Angles have positive clockwise, from 0 to pi, left is pi or -pi
   {
       if ((Velocity.Angle() < (-1 * Mathf.Pi / 4)) && (Velocity.Angle() > (-3 * Mathf.Pi / 4)))
       {
           sprite.Play("run_up");
       }
       else if ((Velocity.Angle() <= (Mathf.Pi / 4)) && (Velocity.Angle() >= (-1 * Mathf.Pi / 4)))
       {
           sprite.Play("run_right");
       }
       else if ((Velocity.Angle() > (Mathf.Pi / 4)) && (Velocity.Angle() < (3 * Mathf.Pi / 4)))
       {
           sprite.Play("run_down");
       }
       else
       {
           sprite.Play("run_left");
       }
   }

}
