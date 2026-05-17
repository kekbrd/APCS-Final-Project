using Godot;
using System;
using System.Threading;

public partial class shyPlant : CharacterBody2D
{
	private char cardinalDirection;
	private const float walkSpeed = 20;
	private const float runSpeed = 200;
	private Boolean playerInRange = false;
	private Boolean isFleeing = false;
    private Boolean isSecondaryFleeing = false;
    private Vector2 secondaryFleeVelocity;
    private Boolean isAlert = false;
    private Random r = new Random();
    private Godot.Timer idleTimer;
    private Godot.Timer alertTimer;
    private float alertTimerLength = 1;
    private Player player;
    private AnimatedSprite2D sprite;
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
	{
        if (isIdle() && playerInRange)
        {
            startFleeSequence();
        }
		MoveAndSlide();
	}
    
    // area detects collisionAreas
    private void body_entered(Node2D body) // Method header written by AI because even though I told it not to show me code it still did.
	{
		if (body is Player) // == and .equals don't work in C#
		{
			playerInRange = true;
		}
        if (body.IsInGroup("preySpecies"))
        {
            shyPlant plant = (shyPlant)body;
            if (plant.getIsFleeing() && isIdle())
            {
                isSecondaryFleeing = true;
                secondaryFleeVelocity = plant.Velocity.Normalized();
                startFleeSequence();
            }
        }
	}
	private void body_exited(Node2D body)
	{
		if (body is Player)
		{
			playerInRange = false;
		}
        if (body is shyPlant)
        {
            shyPlant plant = (shyPlant)body;
            if (plant.getIsFleeing() && isIdle())
            {
                isSecondaryFleeing = true;
                secondaryFleeVelocity = plant.Velocity.Normalized();
                startFleeSequence();
            }
        }
	}

	private async void idle_timer_timeout() // separate from main loop so that it can be interrupted
	{
        if (isIdle())
        {
            setRandomDirection();
            startWalking();
            await ToSignal(GetTree().CreateTimer(2), "timeout"); /* ToSignal converts to be awaitable, needs the timer object and the name of the 
            timer object's signal that it's done, name must be exact hahaa lost 2 hours on that */
            if (isIdle()) // avoid resetting velocity at inappropriate times
            {
                Velocity = Vector2.Zero;
                setIdleAnimation();
                idleTimer.Start((float)r.Next(1, 5));
            }
        }
	}

    private void alert_timer_timeout()
    {
        isFleeing = true;
        isAlert = false;
        if (player != null && !isSecondaryFleeing) // idea of null check from AI
        {
            Velocity = (GlobalPosition - player.GlobalPosition).Normalized(); // The AI forgot my directions to not show code when I asked it what the method was for normalizing, 
            // I would've done the same thing myself but I'll cite it anyways because I saw it.
            Velocity *= runSpeed;
            setRunAnimation();
        }
        else if (player != null && isSecondaryFleeing)
        {
            Velocity = secondaryFleeVelocity;
            Velocity *= runSpeed;
            setRunAnimation();
        }
    }

    public override void _Ready()
    {
		// C# doesn't let you write just method name
        idleTimer = GetNode<Godot.Timer>("IdleTimer"); // thing in <> is the class type, thing in "" is the node name,
                                                       // must be done after the plant is created
        alertTimer = GetNode<Godot.Timer>("AlertTimer");
        sprite = GetNode<AnimatedSprite2D>("Sprite");
        AddToGroup("preySpecies");
        sprite.Play("idle_down");
        player = GetTree().GetFirstNodeInGroup("player") as Player; // gets reference of player from group tab
    }

    public Boolean isIdle()
    {
        return !isFleeing && !isAlert && !isSecondaryFleeing;
    }

    private void setRandomDirection()
    {
        int i = r.Next(0, 4); // inclusive and exclusive
        if (i == 0)
        {
            cardinalDirection = 'N';
        }
        else if (i == 1)
        {
            cardinalDirection = 'E';
        }
        else if (i == 2)
        {
            cardinalDirection = 'S';
        }
        else
        {
            cardinalDirection = 'W';
        }
    }

    private void startWalking()
    {
        setWalkVelocity();
        Velocity *= walkSpeed;
        setWalkAnimation();
    }

	private void setWalkVelocity()
	{
        if (cardinalDirection == 'N')
        {
            Velocity = new Vector2(0, -1);
        }
        else if (cardinalDirection == 'E')
        {
            Velocity = new Vector2(1, 0);
        }
        else if (cardinalDirection == 'S')
        {
            Velocity = new Vector2(0, 1);
        }
        else
        {
            Velocity = new Vector2(-1, 0);
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

    private void startFleeSequence()
    {
        isAlert = true;
        Velocity = Vector2.Zero;
        alertTimer.Start(alertTimerLength);
    }

    public char getCardinalDirection()
    {
        return cardinalDirection;
    }

    public bool getIsFleeing()
    {
        return isFleeing;
    }

    public bool getIsAlert()
    {
        return isAlert;
    }
}
