using Godot;
using System;

public partial class TallGrass : Node2D
{

	[Export] private AnimationPlayer _animation;
	[Export] private PackedScene _stepEffect;

	Random rnd = new Random();
	LevelName TargetLevelName = LevelName.Battle;

	private int _collisionCount = 0;

	public override void _Ready()
	{
	}

	private void OnPlayerEntered(Player player)
	{
		if (player.Name == "Player")
		{
			_animation.Play("Stepped");
			_collisionCount++;

			TallGrassStepEffect grassEffect = _stepEffect.Instantiate<TallGrassStepEffect>();
			grassEffect.Position = Position;
			SpaceManager.GetCurrentScene().AddChild(grassEffect);

			//probabilità di trovare pokemon
			if (rnd.Next(20) == 3 && _collisionCount == 1)
			{
				SceneManager.ChangeLevel(level: TargetLevelName, battle: true);
			}
		}
	}

	private void OnPlayerExited(Player player)
	{
		if (player.Name == "Player")
        {
            _collisionCount--;
        }
	}
}
