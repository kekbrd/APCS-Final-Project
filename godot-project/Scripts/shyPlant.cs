using Godot;
using System;
using System.Threading;

public partial class shyPlant : CharacterBody2D
{
	public char cardinalDirection;
	private const float walkSpeed = 40;
	private const float runSpeed = 200;
	private Boolean playerInRange = false;
	private Boolean isFleeing = false;
	private Boolean isWalking = false;
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

	private void on_timer_timeout()
	{
        setWalkVelocity();
        GetTree().CreateTimer(2);
        Random r = new Random();
        Timer.Start(r.Next(1, 7));
	}

    public override void _Ready()
    {
		setRandomDirection(); // C# doesn't let you write just method name
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta) // Apparently if statements have to be in methods
	{
		if (!playerInRange)
		{
			Velocity *= runSpeed;
		}
		MoveAndSlide();
	}

    private void setRandomDirection()
    {
        Random r = new Random();
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
