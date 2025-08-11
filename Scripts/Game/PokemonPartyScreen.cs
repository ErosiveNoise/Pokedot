using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

//TODO rifare grafica
public partial class PokemonPartyScreen : Node2D
{
	[Export] private Sprite2D _CancelSprite;
	[Export] public PokemonSlot[] PokemonSlots = new PokemonSlot[6];
	[Export] public AnimationPlayer _anim;

	enum Options { FIRST_SLOT, SECOND_SLOT, THIRD_SLOT, FOURTH_SLOT, FIFTH_SLOT, SIXTH_SLOT, CANCEL };
	int selectedOption = (int)Options.FIRST_SLOT;

	Dictionary<Options, Sprite2D> optionDic = new();

	public override void _Ready()
	{
		//popolo da file
		var team = SceneManager.Instance.Team;

		for (int i = 0; i < PokemonSlots.Count(); i++)
		{
			if (i < team.Count)
				PokemonSlots[i].UpdateSlot(team[i]);
			else
				PokemonSlots[i].ClearSlot();
			optionDic.Add((Options)i, PokemonSlots[i].Background);
		}

		optionDic.Add(Options.CANCEL, _CancelSprite);
		SetActiveOption();
	}

	public override void _ExitTree()
	{
		SignalManager.EmitOnReturnToMenu();
	}

	public void UnsetActiveOption()
	{
		optionDic[(Options)selectedOption].Frame = 0;
	}

	public void SetActiveOption()
	{
		optionDic[(Options)selectedOption].Frame = 1;
	}

	public override void _Input(InputEvent @event)
	{
		// if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		// {
		if (!SceneManager.Instance.IsPaused)
		{
			return;
		}

		if (Input.IsActionJustPressed("ui_down"))
		{
			UnsetActiveOption();
			selectedOption = (selectedOption + 1) % 7;
			SetActiveOption();
		}
		else if (Input.IsActionJustPressed("ui_up"))
		{
			UnsetActiveOption();
			if (selectedOption == 0)
			{
				selectedOption = (int)Options.CANCEL;
			}
			else
			{
				selectedOption -= 1;
			}
			SetActiveOption();
		}
		else if (Input.IsActionJustPressed("ui_left"))
		{
			UnsetActiveOption();
			selectedOption = 0;
			SetActiveOption();
		}
		else if (Input.IsActionJustPressed("ui_right") && selectedOption == (int)Options.FIRST_SLOT)
		{
			UnsetActiveOption();
			selectedOption = 1;
			SetActiveOption();
		}
		else if (Input.IsActionJustPressed("ui_accept"))
		{
			//TODO
			switch (selectedOption)
			{
				case 0:
					//POKEDEX
					return;
				case 1:
					//POKEMON TEAM
					return;
				case 2:
					//BAG
					return;
				case 3:
					//Player
					return;
				case 4:
					//SAVE
					return;
				case 5:
					//OPTION
					return;
				case 6:
					//EXIT
					//SpaceManager.GetCurrentScene().RemoveChild(this);
					//SignalManager.EmitOnReturnToMenu();
					//_anim.PlayBackwards("ShowTeam");
					QueueFree();
					//SceneManager.Instance.TransitionExitPartyScreen();
					//menu.Visible = true;
					//menu._screenLoaded = Menu.ScreenLoaded.JUST_MENU;
					return;
			}
			//}
		}
	}
}
