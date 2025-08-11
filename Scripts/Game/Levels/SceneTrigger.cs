using Godot;
using System;
using System.Data;
using System.Collections.Generic;

public partial class SceneTrigger : Area2D
{
	[ExportCategory("Target Scene")]
	[Export] public LevelName TargetLevelName;
	[Export] public string DestinationTrigger;
	[Export] public string SpawnDirection = "up";

	[Export] public Marker2D Spawn;

	[ExportCategory("Current Scene Vars")]
	[Export] public int CurrentLevelTrigger = 0;

	[Export] public Vector2 EntryDirection;

	[Export] public CollisionShape2D CollisionShape2D;

	public bool Locked = false;

	public override void _Ready()
	{
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body.Name != "Player")
		{
			return;
		}
		else
		{
			if (Locked)
			{
				GD.Print("Uh oh!  The door is locked ...");
			}
			else
			{
				SceneManager.ChangeLevel(TargetLevelName, DestinationTrigger);
			}
		}
	}
}
