using System;
using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export] private AnimatedSprite2D _playerSprite;	

	const float speed = 100f;
	int run = 1;
	Vector2 characterDirection;
	string currentDir = "none";

	public override void _Ready()
	{
		_playerSprite.Play("front_idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		Move(delta);
	}

	public void Move(double delta)
	{
		if (Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right") || Input.IsActionPressed("ui_up") || Input.IsActionPressed("ui_down"))
		{
			characterDirection.X = Input.GetAxis("ui_left", "ui_right");
			characterDirection.Y = Input.GetAxis("ui_up", "ui_down");
			characterDirection = characterDirection.Normalized();

			//verifico in che direzione mi sto muovendo per gestire animazioni
			if (characterDirection.X < 0 && characterDirection.Y == 0)//verso SX
			{
				currentDir = "Left";
				PlayAnimation(currentDir, 1);
			}
			else if (characterDirection.X > 0 && characterDirection.Y == 0)//verso DX
			{
				currentDir = "Right";
				PlayAnimation(currentDir, 1);
			}
			else if (characterDirection.Y > 0)//verso DOWN
			{
				currentDir = "Down";
				PlayAnimation(currentDir, 1);
			}
			else if (characterDirection.Y < 0)//verso UP
			{
				currentDir = "Up";
				PlayAnimation(currentDir, 1);
			}

			//TODO manca * delta
			if (Input.IsKeyPressed(Key.Shift))
			{
				run = 2;
				_playerSprite.SpeedScale = 3;
			}
			else
			{
				run = 1;
				_playerSprite.SpeedScale = 1;
			}
			Velocity = characterDirection * speed * run;
		}
		else
		{
			PlayAnimation(currentDir, 0);
			Velocity = new Vector2(0, 0);
			run = 1;
			_playerSprite.SpeedScale = 1;
		}
		MoveAndSlide();
	}

	public void PlayAnimation(string direction, int movement)
	{
		if (direction == "Right")
		{
			_playerSprite.FlipH = false;
			if (movement == 1)
			{
				_playerSprite.Play("side_walk");
			}
			else if (movement == 0)
			{
				_playerSprite.Play("side_idle");
			}
		}
		else if (direction == "Left")
		{
			_playerSprite.FlipH = true;
			if (movement == 1)
			{
				_playerSprite.Play("side_walk");
			}
			else if (movement == 0)
			{
				_playerSprite.Play("side_idle");
			}
		}
		else if (direction == "Up")
		{
			if (movement == 1)
			{
				_playerSprite.Play("back_walk");
			}
			else if (movement == 0)
			{
				_playerSprite.Play("back_idle");
			}
		}
		else if (direction == "Down")
		{
			if (movement == 1)
			{
				_playerSprite.Play("front_walk");
			}
			else if (movement == 0)
			{
				_playerSprite.Play("front_idle");
			}
		}
	}
}