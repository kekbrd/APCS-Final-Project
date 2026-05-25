using Game.UI;
using Godot;
using System;

public partial class Car : StaticBody2D
{
	private Player player;
	private AnimatedSprite2D sprite;
	private bool containsPlayer = false;
	private int numFailedEntry = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("Sprite");
		player = GetTree().GetFirstNodeInGroup("player") as Player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void on_area_2d_body_entered(Node2D body)
	{
		if (body is Player)
		{
			numFailedEntry++;
			if (player.getHasPhoto())
			{
				containsPlayer = true;
				player.setIsVisible(false);
				//win
			}
			else if (numFailedEntry == 1)
			{
				MessageManager.PlayText("The door is locked. Looks like the boss won't let you leave without a photo.");
			}
			else if (numFailedEntry == 2)
			{
				MessageManager.PlayText("It might not be a good idea to mess with the electronic locks.");
			}
			else if (numFailedEntry == 3)
			{
				MessageManager.PlayText("Seriously. Stop doing that.");
			}
			else if (numFailedEntry == 4)
			{
				sprite.Play("explode");
				sprite.Scale = new Vector2(3f, 3f);
			}
		}
	}

	private void on_sprite_animation_finished()
	{
		if (sprite.Animation == "explode")
		{
			QueueFree();
		}
	}
}
