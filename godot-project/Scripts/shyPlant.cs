using Godot;
using System;
using System.Threading;

public partial class shyPlant : CharacterBody2D
{
	public char cardinalDirection;
	private const float walkSpeed = 20;
	private const float runSpeed = 200;
	private Boolean playerInRange = false;
	private Boolean isFleeing = false;
    private Random r = new Random();
    Godot.Timer randMovementTimer;
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

	private async void on_timer_timeout()
	{
        if (!isFleeing)
        {
            setRandomDirection();
            setWalkVelocity();
            Velocity *= walkSpeed;
            await ToSignal(GetTree().CreateTimer(2), "timeout"); // ToSignal converts to be awaitable, needs the timer object and the name of the timer object's signal that it's done, name must be exact
            Velocity = Vector2.Zero;
            randMovementTimer.Start((float)r.Next(1, 7));
        }
	}

    public override void _Ready()
    {
		// C# doesn't let you write just method name
        randMovementTimer = GetNode<Godot.Timer>("Timer"); // thing in <> is the class type, thing in "" is the node name,
        // must be done after the plant is created
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) // Apparently if statements have to be in methods
	{
		MoveAndSlide();
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
}
