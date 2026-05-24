using Game.UI;
using Godot;
using System;
using System.Security;

public partial class Level : Node2D
{
	Player player;
	shyPlant p1;
	// don't really need this but oh well
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		p1 = GetNode<shyPlant>("Plant1"); // The class is whatever the script name is, not node type
		MessageManager.PlayText("You've been hired to take a photo of some blue walking plants. Follow the tracks, pay attention to wind direction, take the picture, then get to the car without dying.");
		player.setTargetSpecies("shyPlantGroup");
	}
}
