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
		MessageManager.PlayText("Hey!");
		player.setTargetSpecies("shyPlantGroup");
	}
}
