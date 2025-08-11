using Godot;

public partial class Menu : CanvasLayer
{
	[Export] public AnimationPlayer _anim;
	[Export] public Control _control;
	[Export] public TextureButton[] menuButtons = new TextureButton[7];

	public enum ScreenLoaded { NOTHING, JUST_MENU, PARTY_SCREEN };
	public ScreenLoaded _screenLoaded = ScreenLoaded.NOTHING;

	public override void _Ready()
	{
		SignalManager.Instance.OnReturnToMenu += OnReturnToMenu;

		for (int i = 0; i < menuButtons.Length; i++)
		{
			int index = i;
			menuButtons[i].Pressed += () => OnMenuPressed(index);
		}
		menuButtons[0].GrabFocus();
	}

	private void OnMenuPressed(int selectedOption)
	{
		if (!SceneManager.Instance.IsPaused)
		{
			return;
		}
		//gestisco la posizione di selectedOption e l'apertura delle relative scene
		//TODO gestire enum al posto di int
		switch (selectedOption)
		{
			case 0:
				//POKEDEX
				return;
			case 1:
				//POKEMON TEAM
				_control.Hide();
				_anim.Play("ShiftMenu");
				SceneManager.Instance.TransitionToPartyScreen();
				_screenLoaded = ScreenLoaded.PARTY_SCREEN;
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
				CloseMenu();
				return;
		}
	}

	public override void _ExitTree()
	{
		SignalManager.Instance.OnReturnToMenu -= OnReturnToMenu;
	}

	private void OnReturnToMenu()
	{
		_screenLoaded = ScreenLoaded.JUST_MENU;
		_control.Show();
		//TODO tornare int per indice pulsante focus
		menuButtons[0].GrabFocus();
		_anim.PlayBackwards("ShiftMenu");
	}

	private void CloseMenu()
	{
		Node player = SpaceManager.GetPlayer();
		player.SetPhysicsProcess(true);
		_screenLoaded = ScreenLoaded.NOTHING;
		SpaceManager.BlurTextureVisible(true);
		SceneManager.Instance.IsPaused = false;

		_anim.PlayBackwards("ShowMenu");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			switch (_screenLoaded)
			{
				case ScreenLoaded.NOTHING:
					if (Input.IsActionJustPressed("ui_cancel"))//(keyEvent.Keycode == Key.Escape)
					{
						SceneManager.Instance.IsPaused = true;
						//TODO gestire errore apertura menu in battaglia
						Node player = SpaceManager.GetPlayer();
						player.SetPhysicsProcess(false);
						_screenLoaded = ScreenLoaded.JUST_MENU;
						SpaceManager.BlurTextureVisible(false);

						_anim.Play("ShowMenu");
						menuButtons[0].GrabFocus();
					}
					break;
				case ScreenLoaded.JUST_MENU:
					if (Input.IsActionJustPressed("ui_cancel"))
					{
						CloseMenu();
					}
					break;
				case ScreenLoaded.PARTY_SCREEN:
					if (Input.IsActionJustPressed("ui_cancel"))
					{
						SceneManager.Instance.TransitionExitPartyScreen();
						_anim.PlayBackwards("ShiftMenu");
						_screenLoaded = ScreenLoaded.JUST_MENU;
					}
					break;
			}
		}
	}
}
