using Godot;
using System;

public partial class BattleMenu : Control
{
	[Export] public GridContainer MainMenu;
	[Export] public GridContainer AttackMenu;
	[Export] public TextureButton[] MenuButtons = new TextureButton[4];
	[Export] public TextureButton[] AttackButtons = new TextureButton[5];

	public override void _Ready()
	{
		for (int i = 0; i < MenuButtons.Length; i++)
		{
			int index = i;
			MenuButtons[i].Pressed += () => OnMenuSelected(index);

			MenuButtons[i].FocusEntered += () => OnFocusEntered(MenuButtons, index);
			MenuButtons[i].FocusExited += () => OnFocusExited(MenuButtons, index);

			AttackButtons[i].FocusEntered += () => OnFocusEntered(AttackButtons, index);
			AttackButtons[i].FocusExited += () => OnFocusExited(AttackButtons, index);
		}
		//ShowMainMenu();
	}

	private void OnFocusExited(TextureButton[] button, int index)
	{
		button[index].GetNode<Label>("Nome").AddThemeColorOverride("font_color", new Color(0.51f, 0.51f, 0.463f));
	}

	private void OnFocusEntered(TextureButton[] button, int index)
	{
		button[index].GetNode<Label>("Nome").AddThemeColorOverride("font_color", new Color(1, 1, 1));
	}

	private void OnMenuSelected(int index)
	{
		switch (index)
		{
			case 0:
				OnAttackPressed();
				break;
			case 1:
				OnTeamPressed();
				break;
			case 2:
				OnBagPressed();
				break;
			case 3:
				OnRunPressed();
				break;
		}
	}

	public void ShowMainMenu()
	{
		MainMenu.Visible = true;
		AttackMenu.Visible = false;
		SetMainMenuFocus();
	}

	private void OnAttackPressed()
	{
		MainMenu.Visible = false;
		AttackMenu.Visible = true;

		SetAttackMenuFocus();
	}

	public void SetMainMenuFocus()
	{
		MenuButtons[0].GrabFocus(); // seleziona Attacca come primo focus
	}

	public void SetAttackMenuFocus()
	{
		AttackButtons[0].GrabFocus(); // seleziona la prima mossa
	}

	private void OnMoveSelected(int index)
	{
		//GD.Print($"Hai scelto la mossa: {currentMoves[index]}");
		// Inserisci qui la logica per usare la mossa
		ShowMainMenu();
	}

	private void OnTeamPressed()
	{
		GD.Print("Hai selezionato TEAM");
		// Apri schermata team
	}

	private void OnBagPressed()
	{
		GD.Print("Hai selezionato ZAINO");
		// Apri schermata oggetti
	}

	private void OnRunPressed()
	{
		GD.Print("Hai selezionato FUGGI");
		if (CheckRun())
		{
			//TODO devo gestire il QueueFree() quando torno a livello?
			SceneManager.ChangeLevel(LevelName.Back);
			SpaceManager.BlurTextureVisible(true);
		}
		else
		{
			//TODO visualizzare messaggio "Fuga non riuscita"
			GD.Print("Fuga non riuscita");
		}

	}

    private bool CheckRun()
    {
		//TODO Logica per fuggire dalla battaglia
		return true;
    }
}
