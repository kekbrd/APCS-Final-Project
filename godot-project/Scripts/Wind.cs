using Godot;
using System;

public partial class Wind : AnimatedSprite2D
{
	private float windDirection;
	private Random r = new Random();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AddToGroup("wind");
		windDirection = (float)r.NextDouble() * 2 * Mathf.Pi;
		Rotate(windDirection); // is clockwise
		if (windDirection > Mathf.Pi/2 && windDirection < 3 * Mathf.Pi / 2)
		{
			FlipV = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public float getWindDirection()
	{
		return windDirection;
	}
}
