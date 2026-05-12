using Godot;
using System;
using System.Security;

public partial class Level : Node2D
{
	Player player;
	shyPlant p1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		p1 = GetNode<shyPlant>("Plant1"); // The class is whatever the script name is, not node type
		p1.setPlayer(player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
