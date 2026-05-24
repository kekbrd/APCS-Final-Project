using Godot;
using System;
using System.Threading;

public partial class shyPlant : CharacterBody2D, IHasCardinalDirection
{
    public const float shyPlantDetectionRadius = 149f;
    
    private char cardinalDirection;

    private const float walkSpeed = 20f;
    private const float runSpeed = 200f;
    private Vector2 secondaryFleeVelocity;

    private Boolean playerInRange = false;
    private Boolean isFleeing = false;
    private Boolean isSecondaryFleeing = false;
    private Boolean isAlert = false;
    private Boolean playerInOlfactoryRange = false;
    
    private Godot.Timer idleTimer;
    private Godot.Timer alertTimer;
    private float alertTimerLength = 1f;
    private float secondaryFleeTimerLength = 0.3f;
    
    private Random r = new Random();
    private Player player;
    private AnimatedSprite2D sprite;
    private Wind w;
    
    private Node2D nodeInOlfactoryRange;
    private float widthOfWind = 3f / 8f; // apparently everything defaults to int or double and you have to label all floats

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        if (isIdle() && playerInRange)
        {
            startFleeSequence();
        }
        if (playerInOlfactoryRange && nodeInOlfactoryRange != null && isIdle() && playerIsUpwind())
        {
            startFleeSequence();
        }
        MoveAndSlide();
    }

    // area detects collisionAreas only
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
                isAlert = true;
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
        if (body.IsInGroup("preySpecies"))
        {
            shyPlant plant = (shyPlant)body;
            if (plant.getIsFleeing() && isIdle())
            {
                isSecondaryFleeing = true;
                isAlert = true;
                secondaryFleeTimerLength = 0; // compensate for knowing only when it exits
                secondaryFleeVelocity = plant.Velocity.Normalized();
                startFleeSequence();
            }
        }
    }

    private void on_olfactory_detection_body_entered(Node2D body)
    {
        if (body is Player)
        {
            playerInOlfactoryRange = true;
            nodeInOlfactoryRange = body;
        }
    }

    private void on_olfactory_detection_body_exited(Node2D body)
    {
        if (body is Player)
        {
            playerInOlfactoryRange = false;
            nodeInOlfactoryRange = null;
        }
    }

    private async void idle_timer_timeout() // separate from main loop so that it can be interrupted
    {
        if (isIdle())
        {
            cardinalDirection = IdleMovement.setRandomDirection();
            Velocity = IdleMovement.startWalking(cardinalDirection, sprite, walkSpeed);
            await ToSignal(GetTree().CreateTimer(2), "timeout"); /* ToSignal converts to be awaitable, needs the timer object and the name of the 
            timer object's signal that it's done, name must be exact hahaa lost 2 hours on that */
            if (isIdle()) // avoid resetting velocity at inappropriate times
            {
                Velocity = Vector2.Zero;
                IdleMovement.setIdleAnimation(sprite, cardinalDirection);
                idleTimer.Start(r.Next(1, 5));
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
        AddToGroup("shyPlantGroup");
        sprite.Play("idle_down");
        idleTimer.Start(r.Next(1, 5));
        w = GetTree().GetFirstNodeInGroup("wind") as Wind;
        player = GetTree().GetFirstNodeInGroup("player") as Player; // gets reference of player from group tab
    }

    public Boolean isIdle()
    {
        return !isFleeing && !isAlert && !isSecondaryFleeing;
    }

    private bool playerIsUpwind() // angletopoint is not around radians, its the weird godot thing
    {
        float angleFromPlayer = nodeInOlfactoryRange.GlobalPosition.AngleToPoint(GlobalPosition);
        return angleFromPlayer > w.getWindDirection() - widthOfWind && angleFromPlayer < w.getWindDirection() + widthOfWind;
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

    private void startFleeSequence()
    {
        isAlert = true;
        Velocity = Vector2.Zero;
        if (isSecondaryFleeing)
        {
            alertTimer.Start(secondaryFleeTimerLength);
        }
        else
        {
            alertTimer.Start(alertTimerLength);
        }
    }

    public char getCardinalDirection()
    {
        if (isFleeing)
        {
            return 'Z';
        }
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
