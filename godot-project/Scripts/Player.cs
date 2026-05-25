using Godot;
using System;
using System.Runtime.ConstrainedExecution;
using Game.UI;
using System.Text.RegularExpressions;

public partial class Player : CharacterBody2D
{
	private const float speed = 200f; // should be 50 normally, changed for testing
	private char cardinalDirection;
    private AnimatedSprite2D sprite;
    private Vector2 prevVelocity = Vector2.Zero; 
    private Boolean hasPhoto = false;
    private Boolean isAlive = true;
    private Boolean isVisible = true;
    private String targetSpeciesGroup;
    private float photoRange;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		cardinalDirection = 'S';
        sprite = GetNode<AnimatedSprite2D>("Sprite");
        photoRange = shyPlant.shyPlantDetectionRadius + 40f;
    }
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionReleased("take_photo"))
        {
            Node2D closest = closestNode2DInGroup(targetSpeciesGroup);

            if (!isAlive || !isVisible)
            {
                // nothing happens
            }
            else if (hasPhoto)
            {
                MessageManager.PlayText("You've already taken a good picture. Return to the car before you get mauled, please.");
            }
            else if (GlobalPosition.DistanceTo(closest.GlobalPosition) > photoRange + 100f)
            {
                MessageManager.PlayText("You're too far away from the target. The boss has pretty bad vision, but I don't think you can pass off that tree as the target.");
            }
            else if (GlobalPosition.DistanceTo(closest.GlobalPosition) > photoRange)
            {
                MessageManager.PlayText("Get a little closer, we need more detail.");
            }
            else if (targetIsFleeing(closest))
            {
                MessageManager.PlayText("The photo is too blurry.");
            }
            else if (correctAngleForPhoto(closest))
            {
                hasPhoto = true;
                MessageManager.PlayText("Nice! Got the photo.");
            }
            else if (!correctAngleForPhoto(closest))
            {
                MessageManager.PlayText("Come on, pick a more flattering angle.");
            }
            else
            {
                GD.Print("Taking photo has resulted in unexpected error.");
            }
        }
    }

    public override void _PhysicsProcess(double delta) // PhysicsProcess is used for movement, not Process.
	{
        Velocity = Vector2.Zero; // Velocity is built in for characterbody, it's a Vector2 which has X and Y values
        if (!isVisible)
        {
            sprite.Stop();
        }
        if (isAlive && isVisible)
        {
            Velocity = Input.GetVector("move_left", "move_right", "move_up", "move_down") * speed; // Got this line from AI. Up is negative y, down is positive.
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
        else if (!isAlive)
        {
            sprite.Play("die");
            sprite.Scale = new Vector2(0.5f, 0.5f);
        }
	}

    private bool targetIsFleeing(Node2D n)
    {
        IHasCardinalDirection target = (IHasCardinalDirection)n;
        return target.getCardinalDirection() == 'Z';
    }

    private bool correctAngleForPhoto(Node2D n) // any angle other than the quadrant behind target is good
    {
        IHasCardinalDirection target = (IHasCardinalDirection)n;
        char direction = target.getCardinalDirection();

        Vector2 diff = GlobalPosition - n.GlobalPosition; // this group of 2 lines from AI, gets angle relative to target
        float angle = Mathf.Atan2(diff.Y, diff.X);

        if (direction == 'E') 
        {
            return angle > (-3 * Mathf.Pi / 4) && angle < (3 * Mathf.Pi / 4);
        }
        else if (direction == 'S')
        {
            return angle > (-1 * Mathf.Pi / 4) || angle < (-3 * Mathf.Pi / 4);
        }
        else if (direction == 'W')
        {
            return angle < (-1 * Mathf.Pi / 4) || angle > Mathf.Pi / 4;
        }
        else if (direction == 'N')
        {
            return angle < Mathf.Pi / 4 || angle > 3 * Mathf.Pi / 4;
        }
        else
        {
            return false;
        }
    }

    private Node2D closestNode2DInGroup(String group)
    {
        Node2D result = (Node2D)GetTree().GetFirstNodeInGroup(group); // Nodes don't have global position, but Node2D does
        foreach (Node raw in GetTree().GetNodesInGroup(group)) // c# for each loops look like this apparently
        {
            Node2D n = (Node2D)raw;
            if (GlobalPosition.DistanceTo(n.GlobalPosition) < GlobalPosition.DistanceTo(result.GlobalPosition))
            {
                result = n;
            }
        }
        return result;
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

    public bool getHasPhoto()
    {
       return hasPhoto;
    }

    public bool getIsAlive()
    {
       return isAlive;
    }

    public void setIsAlive(bool b)
    {
        isAlive = b;
    }

    public void setTargetSpecies(String g)
    {
        targetSpeciesGroup = g;
    }

    public void setIsVisible(bool b)
    {
        isVisible = b;
    }

    public bool getIsVisible()
    {
        return isVisible;
    }
}
