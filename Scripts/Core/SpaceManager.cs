using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class SpaceManager : Node
{
	public static SpaceManager Instance { get; private set; }
	[Export] SubViewport CurrentScene;
	[Export] public TextureRect BlurTexture;
	
	public PackedScene Menu = GD.Load<PackedScene>("Scenes/Game/Menu.tscn");
	public PokemonDownloader PokedexLoader = new PokemonDownloader();

	const string POKEMON_FILE = "Scripts/Game/Battle/pokedex.json";
	public static Dictionary<string, Pokemon> pokedex = new Dictionary<string, Pokemon>();

	public override void _Ready()
	{
		Instance = this;
		LoadPokedexLoader();
		//LoadPokemonFile();
		SceneManager.ChangeLevel();
		LoadMenu();
		//CallDeferred(nameof(LoadMenuScene));
	}

    private void LoadPokedexLoader()
    {
		//TODO gestire INTRO in attesa del caricamento Pokedex
        using var file = FileAccess.Open(POKEMON_FILE, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			PokedexLoader.Main();
		}
    }

	private void LoadMenu()
	{
		Menu menu = Menu.Instantiate<Menu>();
		Instance.CurrentScene.AddChild(menu);
	}

	// private void LoadMenuScene()
	// {
	// 	Menu menu = Menu.Instantiate<Menu>();
	// 	GetTree().Root.AddChild(menu);
    // }

	public static void LoadPokemonFile()
	{
		using var file = FileAccess.Open(POKEMON_FILE, FileAccess.ModeFlags.Read);
		if (file != null)
		{
			try
			{
				string json = file.GetAsText();

				pokedex = JsonConvert.DeserializeObject<Dictionary<string, Pokemon>>(json);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Errore nella lettura del JSON: {ex.Message}");
			}
		}
	}

	public static SubViewport GetCurrentScene()
	{
		return Instance.CurrentScene;
	}

	public static Node GetPlayer()
	{
		return Instance.CurrentScene.GetChild(0).GetNode("Player");
	}
	
	public static void BlurTextureVisible(bool isVisible)
	{
		Instance.BlurTexture.Visible = isVisible;
	}
}
