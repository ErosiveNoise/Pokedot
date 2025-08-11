using Godot;
using System;

public partial class Level : Node2D
{
	[ExportCategory("Level Basic")]
	[Export] public LevelName LevelName;

	[ExportCategory("Camera Limits")]
	[Export] public int Top;
	[Export] public int Bottom;
	[Export] public int Left;
	[Export] public int Right;

	[ExportCategory("Player Var")]
	//[Export] public PackedScene _playerScene;
	[Export] public Marker2D _defaultSpawnPoint = null;
	[Export] private PackedScene _playerScene;
	[Export] private PackedScene _menuScene;

	public override void _Ready()
	{
	}
}
