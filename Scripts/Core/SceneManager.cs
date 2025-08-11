using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class SceneManager : Node
{
	[Export] public ColorRect FadeRect;
	[Export] public AnimationPlayer animFader;
	[Export] private PackedScene _playerScene;

	public static SceneManager Instance { get; private set; }
	public string DestinationTrigger;
	public string SpawnPoint;
	public bool IsPaused;

	public static Level CurrentLevel;
	public LevelName CurrentLevelName;
	public Array<Level> AllLevels { get; set; } = new Array<Level>();
	Vector2 newPosition = new Vector2();
	bool spawn = false;
	Battle currentBattle;

	//party screen
	PokemonPartyScreen partyScene;

	//team pokemon del player
	public List<Pokemon> Team = new List<Pokemon>();

	public override void _Ready()
	{
		Instance = this;
		IsPaused = false;

		Input.MouseMode = Input.MouseModeEnum.Captured;

		SpaceManager.LoadPokemonFile();

		LoadTeam();

		//TODO da rimuovere, solo per test
		if (Team.Count == 0)
		{
			AddToTeam(SpaceManager.pokedex["4"]);
			AddToTeam(SpaceManager.pokedex["6"]);
			AddToTeam(SpaceManager.pokedex["18"]);
			SaveTeam();
		}
	}

	public override void _ExitTree()
	{
		SaveGame();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse || @event is InputEventMouseButton || @event is InputEventMouseMotion)
		{
			GetViewport().SetInputAsHandled();
		}
	}

	private void SaveGame()
	{
		SaveTeam();
		// Esempio semplice con file JSON
		Godot.Collections.Dictionary<string, Variant> saveData = new Godot.Collections.Dictionary<string, Variant>();
		saveData["livello"] = 5;
		saveData["hp"] = 100;

		string path = ProjectSettings.GlobalizePath("user://save.json");
		string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
		File.WriteAllText(path, json);
	}

	//TODO
	// private void LoadAttackType()
	// {
	//     Dictionary<TipoMossa, PackedScene> effetti = new() {
	// 	{ TipoMossa.Fuoco, GD.Load<PackedScene>("res://Effects/Fuoco.tscn") },
	// 	{ TipoMossa.Fulmine, GD.Load<PackedScene>("res://Effects/Fulmine.tscn") }
	// 	};
	// }

	//salva team su json
	public void SaveTeam()
	{
		string path = ProjectSettings.GlobalizePath("user://team.json");
		string json = JsonConvert.SerializeObject(Team, Formatting.Indented);
		File.WriteAllText(path, json);
		GD.Print("Team salvato!");
	}

	public void LoadTeam()
	{
		string path = ProjectSettings.GlobalizePath("user://team.json");
		if (!File.Exists(path))
		{
			GD.Print("Nessun salvataggio trovato.");
			return;
		}

		string json = File.ReadAllText(path);
		Team = JsonConvert.DeserializeObject<List<Pokemon>>(json);
		GD.Print("Team caricato!");
	}

	public void AddToTeam(Pokemon pokemon)
	{
		Team.Add(pokemon);
	}

	public void RemoveFromTeam(Pokemon pokemon)
	{
		Team.Remove(pokemon);
	}

	public static async void ChangeLevel(LevelName level = LevelName.Level, string spawnPoint = null, bool battle = false)
	{
		await Instance.ToSignal(Instance.GetTree(), "process_frame");
		Instance.CurrentLevelName = level;
		Instance.SpawnPoint = spawnPoint;
		Instance.animFader.Play("FadeIn");
		if (battle)
		{
			Instance.GetBattle(Instance.CurrentLevelName);
		}
		else
		{
			if (Instance.CurrentLevelName == LevelName.Back)
			{
				Instance.GetLevelFromBattle(Instance.CurrentLevelName);
			}
			else
			{
				Instance.GetLevel(Instance.CurrentLevelName);
			}
		}
		if (!battle) Instance.Spawn(Instance.SpawnPoint);
		Instance.animFader.Play("FadeOut");
	}

	public void Spawn(string trigger)
	{
		if (CurrentLevelName != LevelName.Back)
		{
			if (trigger != null)
			{
				string spawnPath = "/SceneTriggers/" + trigger;
				SceneTrigger spawn = SpaceManager.GetCurrentScene().GetNode(CurrentLevelName + spawnPath) as SceneTrigger;
				newPosition = spawn.Position + spawn.Spawn.Position;
			}
			else
			{
				newPosition = CurrentLevel._defaultSpawnPoint.Position;
			}

			Player player = _playerScene.Instantiate<Player>();
			player.Position = newPosition;
			SpaceManager.GetCurrentScene().GetChild(0).AddChild(player);
		}
	}

	public void GetLevel(LevelName levelName)
	{
		if (CurrentLevel != null)
		{
			Player pPlayer = SpaceManager.GetPlayer() as Player;
			pPlayer.QueueFree();
			SpaceManager.GetCurrentScene().RemoveChild(CurrentLevel);
		}

		CurrentLevel = AllLevels.FirstOrDefault(level => level.LevelName == levelName);

		if (CurrentLevel != null)
		{
			SpaceManager.GetCurrentScene().AddChild(CurrentLevel);
			CurrentLevel.GetParent().MoveChild(CurrentLevel, 0);
			spawn = false;
		}
		else
		{
			CurrentLevel = GD.Load<PackedScene>("Scenes/Levels/" + levelName + ".tscn").Instantiate<Level>();
			AllLevels.Add(CurrentLevel);
			SpaceManager.GetCurrentScene().AddChild(CurrentLevel);
			CurrentLevel.GetParent().MoveChild(CurrentLevel, 0);
			spawn = true;
		}
	}

	public void GetLevelFromBattle(LevelName levelName)
	{
		if (currentBattle != null)
		{
			SpaceManager.GetCurrentScene().RemoveChild(currentBattle);
		}

		if (CurrentLevel != null)
		{
			SpaceManager.GetCurrentScene().AddChild(CurrentLevel);
			CurrentLevel.GetParent().MoveChild(CurrentLevel, 0);
			spawn = false;
		}
		// else
		// {
		// in teoria questo ramo false non verrà mai usato
		// CurrentLevel = GD.Load<PackedScene>("Scenes/Levels/" + levelName + ".tscn").Instantiate<Level>();
		// AllLevels.Add(CurrentLevel);
		// SpaceManager.GetCurrentScene().AddChild(CurrentLevel);
		// CurrentLevel.GetParent().MoveChild(CurrentLevel, 0);
		// spawn = true;
		//}
	}

	private void GetBattle(LevelName levelName)
	{
		//TODO inserire qui transizione
		if (CurrentLevel != null) SpaceManager.GetCurrentScene().RemoveChild(CurrentLevel);
		currentBattle = GD.Load<PackedScene>("Scenes/Battle/" + levelName + ".tscn").Instantiate<Battle>();
		SpaceManager.GetCurrentScene().AddChild(currentBattle);
		spawn = true;
	}

	public void TransitionToPartyScreen()
	{
		PackedScene PartyScene = GD.Load<PackedScene>("Scenes/Game/PokemonPartyScreen.tscn");
		partyScene = PartyScene.Instantiate<PokemonPartyScreen>();
		SpaceManager.GetCurrentScene().AddChild(partyScene);
		//GetTree().Root.AddChild(partyScene);
		partyScene._anim.Play("ShowTeam");
	}

	public void TransitionExitPartyScreen()
	{
		SpaceManager.GetCurrentScene().RemoveChild(partyScene);
		//GetTree().Root.RemoveChild(partyScene);
	}
}
