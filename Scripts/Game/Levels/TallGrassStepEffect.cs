using Godot;
using System;

public partial class TallGrassStepEffect : AnimatedSprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Frame = 0;
	}

	public void OnAnimationFinished()
	{
		QueueFree();
	}
}
