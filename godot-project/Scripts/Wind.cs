using Godot;
using System;

public partial class Wind : AnimatedSprite2D
{
	private float windDirection;
	private Random r = new Random();
	public override void _Ready()
	{
		windDirection = (float)r.NextDouble() * 2 * Mathf.Pi - Mathf.Pi;
		Rotate(windDirection); // is clockwise
		if (windDirection > Mathf.Pi / 2 || windDirection < -1 * Mathf.Pi / 2)
		{
			FlipV = true;
		}
	}

	public float getWindDirection()
	{
		return windDirection;
	}
}
