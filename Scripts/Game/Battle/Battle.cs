using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

/*
TODO
Turni determinati da Speed
Calcolo danni in base a resistenza e debolezza
Sistema di eventi e animazioni (AnimationPlayer)
*/

public partial class Battle : Control
{
	[Export] private AnimatedSprite2D _enemyPokemonSprite;
	[Export] private Sprite2D _enemyEffectSprite;
	[Export] private AnimatedSprite2D _myPokemonSprite;
	[Export] private HealthBar _enemyPokemonHealthBar;
	[Export] private HealthBar _myPokemonHealthBar;
	[Export] private Node2D _enemyInfoPokemon;
	[Export] private Node2D _myInfoPokemon;
	[Export] private BattleMenu _battleMenu;
	[Export] private Label _attackLabel;
	[Export] private Label _finalLabel;
	[Export] private Label _introLabel;
	[Export] private Viewport _viewport;

	Pokemon enemyPokemon = new Pokemon();
	Pokemon myPokemon = new Pokemon();
	Vector2 spriteSize = new Vector2(1, 1);

	int[] _myMossePP = new int[4];
	int[] _myMossePower = new int[4];
	string[] _myMosseName = new string[4];

	int[] _enemyMossePower = new int[4];
	string[] _enemyMosseName = new string[4];

	string imgPath = "Assets/Pokemon/Images/Png/Idle";
	private BattleState _state;

	float messageTime = 1.0f;

	public override async void _Ready()
	{
		_attackLabel.Text = "";
		_finalLabel.Text = "";
		_introLabel.Text = "";

		// Collega i bottoni delle mosse
		for (int i = 0; i < _battleMenu.AttackButtons.Length; i++)
		{
			int index = i; // Necessario per catturare correttamente l'indice
			_battleMenu.AttackButtons[i].Pressed += async () => await OnAttackSelected(index);
		}

		//TODO enemyPokemon potrà essere passato come parametro in caso di scontro con avversario o casuale ma tra un pool definito in base alla zona della mappa/livello
		enemyPokemon = SpaceManager.pokedex[GetRandom(SpaceManager.pokedex.Count).ToString()];
		myPokemon = SceneManager.Instance.Team[0];

		GUIUpdate();
		SpaceManager.BlurTextureVisible(false);
		await StartBattle();
	}

	private async Task StartBattle()
	{
		_state = BattleState.Start;
		//TODO eventuale animazioni
		//TODO sistemare scritta su pokemon
		await StartTyping($"{enemyPokemon.Name} selvatico appare", _introLabel, 1.5f);
		PlayerTurn();
	}

	private void PlayerTurn()
	{
		_state = BattleState.PlayerTurn;
		_battleMenu.Visible = true;
		_battleMenu.ShowMainMenu();
	}

	private async Task OnAttackSelected(int index)
	{
		if (_state != BattleState.PlayerTurn)
			return;

		if (index == 4)
		{
			//return to menu
			_battleMenu.MainMenu.Visible = true;
			_battleMenu.AttackMenu.Visible = false;
			_battleMenu.MenuButtons[0].GrabFocus();
		}
		else
		{
			if (_myMosseName[index].ToString() != "—" && _myMossePP[index] > 0)
			{
				await StartTyping($"Hai usato {_myMosseName[index].ToUpper()}!", _attackLabel, messageTime);
				enemyPokemon.CurrentHp -= _myMossePower[index];
				//TODO correggere
				myPokemon.Attacks[index].CurrentPP -= 1;
				UpdateBattleMenu();
				//TODO parametrizzare nome effetto mossa
				await EffettoMossa("electric_sparks_2", _enemyEffectSprite);
				if (enemyPokemon.CurrentHp <= 0)
				{
					_enemyPokemonHealthBar.SetHealth(0);
					PokemonDie(_enemyPokemonSprite);
					await Final("vittoria");
				}
				else
				{
					_enemyPokemonHealthBar.SetHealth(enemyPokemon.CurrentHp);
					_battleMenu.Visible = false;
					EnemyTurn();
				}
			}
		}
	}

	// private void AttackEnemy()
	// {
	// 	_state = BattleState.Busy;
	// 	//TODO animazione attacco
	// 	//await ToSignal(GetTree().CreateTimer(2), "timeout");

	// 	EnemyTurn();
	// }

	private async void EnemyTurn()
	{
		_state = BattleState.EnemyTurn;
		//TODO animazione attacco
		await OnEnemyAttackSelected(GetRandom(4));
		//await ToSignal(GetTree().CreateTimer(2), "timeout");

		PlayerTurn();
	}

	private async Task OnEnemyAttackSelected(int index)
	{
		if (_enemyMosseName[index].ToString() != "—")
		{
			//
		}
		else if (_enemyMosseName[index - 1]?.ToString() != "—")
		{
			index--;
		}
		else if (index + 1 < 4 && _enemyMosseName[index + 1]?.ToString() != "—")
		{
			index++;
		}
		else
		{
			index = 0;
		}

		await StartTyping($"Il nemico usa {_enemyMosseName[index].ToUpper()}!", _attackLabel, messageTime);

		if (myPokemon.CurrentHp - _enemyMossePower[index] <= 0)
		{
			myPokemon.CurrentHp = 0;
			_myPokemonHealthBar.SetHealth(0);
			PokemonDie(_myPokemonSprite);
			await Final("sconfitta");
		}
		else
		{
			myPokemon.CurrentHp -= _enemyMossePower[index];
			_myPokemonHealthBar.SetHealth(myPokemon.CurrentHp);
		}
	}

	public async Task EffettoMossa(string nomeMossa, Sprite2D pokemonSprite)
	{
		// Rimuovi vecchio effetto (se esiste)
		var vecchio = _viewport.GetNodeOrNull<Node3D>("EffettoMossa3D");
		if (vecchio != null)
			vecchio.QueueFree();

		// Carica il nuovo effetto
		//TODO sistemare selezione cartella effetto
		var scenaEffetto = GD.Load<PackedScene>($"PolyBlocks/EffectBlocks/assets/energy/{nomeMossa}.tscn");
		var nuovoEffetto = scenaEffetto.Instantiate<Node3D>();
		nuovoEffetto.Name = "EffettoMossa3D";
		_viewport.AddChild(nuovoEffetto);

		pokemonSprite.Texture = _viewport.GetTexture();
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		pokemonSprite.Texture = null;
	}

	private void PokemonDie(AnimatedSprite2D pokemon)
	{
		AnimationPlayer animationPlayer = pokemon.GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("EnemyDie");
	}

	private void GUIUpdate()
	{
		_enemyInfoPokemon.GetNode<Label>("Nome").Text = enemyPokemon.Name;
		_enemyInfoPokemon.GetNode<Label>("LevelNr").Text = enemyPokemon.Level.ToString();

		_myInfoPokemon.GetNode<Label>("Nome").Text = myPokemon.Name;
		_myInfoPokemon.GetNode<Label>("LevelNr").Text = myPokemon.Level.ToString();

		for (int i = 0; i < 4; i++)
		{
			_myMossePower[i] = myPokemon.Attacks[i].Power;
			_myMossePP[i] = myPokemon.Attacks[i].CurrentPP;
			_myMosseName[i] = myPokemon.Attacks[i].Name;
			_enemyMossePower[i] = enemyPokemon.Attacks[i].Power;
			_enemyMosseName[i] = enemyPokemon.Attacks[i].Name;
		}

		UpdateBattleMenu();

		SpriteFrames enemySpriteFrames = CreateSpriteFramesFromFolder(imgPath + "-front/" + enemyPokemon.Name);
		AnimationSprite(enemySpriteFrames, _enemyPokemonSprite, new Vector2(0.5f, 0.5f));

		SpriteFrames mySpriteFrames = CreateSpriteFramesFromFolder(imgPath + "-back/" + myPokemon.Name);
		AnimationSprite(mySpriteFrames, _myPokemonSprite, new Vector2(1.5f, 1.5f));

		_enemyPokemonHealthBar.InitHealth(enemyPokemon);
		_myPokemonHealthBar.InitHealth(myPokemon);
	}

    private void UpdateBattleMenu()
    {
		for (int i = 0; i < 4; i++)
		{
			_battleMenu.AttackButtons[i].GetNode<Label>("Nome").Text = myPokemon.Attacks[i].Name;
			_battleMenu.AttackButtons[i].GetNode<Label>("PP").Text = myPokemon.Attacks[i].CurrentPP + "/" + myPokemon.Attacks[i].PP;
		}
    }

    private void AnimationSprite(SpriteFrames newspriteFrames, AnimatedSprite2D PokemonSprite, Vector2 defaultScale)
	{
		if (newspriteFrames != null)
		{
			if (PokemonSprite != null)
			{
				PokemonSprite.SpriteFrames = newspriteFrames;
				PokemonSprite.Animation = "default";
				Vector2 scale = new Vector2(1f, 1f);
				//TODO non vengono corretamente renderizzati i NEMICI:
				// doduo
				// machop (nell'animazione in cui muore di ingrandisce)
				// Charizard
				// Nidorina
				if (spriteSize.X > 310)
				{
					scale = new Vector2(1f, 1f) * defaultScale;
				}
				else
				{
					scale = new Vector2(0.5f, 0.5f) * defaultScale;
				}
				PokemonSprite.Scale = scale;
				PokemonSprite.SpeedScale = 6;
				PokemonSprite.Play();
				GD.Print("SpriteFrames caricati e assegnati all'AnimatedSprite2D.");
			}
			else
			{
				GD.PrintErr("Nessun AnimatedSprite2D trovato nella scena per assegnare i SpriteFrames.");
			}

			// Puoi anche salvare i SpriteFrames come risorsa per riutilizzarli
			// ResourceSaver.Save(newSpriteFrames, "res://GeneratedSpriteFrames/PlayerAnimation.tres");
			// GD.Print("SpriteFrames salvati come PlayerAnimation.tres");
		}
	}

	public SpriteFrames CreateSpriteFramesFromFolder(string folderPath)
	{
		SpriteFrames spriteFrames = new SpriteFrames();
		string animationName = "default";

		//TODO - sarebbe da controllare l'esistenza di una SpriteFrames per questo pokemon?

		using var dir = DirAccess.Open(folderPath);
		if (dir == null)
		{
			GD.PrintErr($"Impossibile aprire la directory: {folderPath}");
			return null;
		}

		// Recupera tutti i nomi dei file PNG nella cartella
		string[] files = dir.GetFiles();
		Array.Sort(files); // Ordina i file per garantire l'ordine corretto dell'animazione

		foreach (string file in files)
		{
			if (file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
			{
				string fullPath = Path.Combine(folderPath, file); // Godot Path.Combine funziona con res://

				// Carica la Texture2D
				Texture2D texture = GD.Load<Texture2D>(fullPath);
				if (texture != null)
				{
					// Aggiungi la texture come fotogramma all'animazione
					spriteFrames.AddFrame(animationName, texture);
					spriteSize = texture.GetSize();
				}
				else
				{
					GD.PrintErr($"Impossibile caricare la texture: {fullPath}");
				}
			}
		}

		if (spriteFrames.GetFrameCount(animationName) == 0)
		{
			GD.PrintErr($"Nessun fotogramma PNG trovato nella cartella: {folderPath}");
			return null;
		}

		return spriteFrames;
	}

	public int GetRandom(int max)
	{
		Random rnd = new Random();
		return rnd.Next(max - 1) + 1;
	}

	private async Task StartTyping(string text, Label label, float waitTime)
	{
		// FASE 1: COMPARSA LETTERA PER LETTERA
		for (int i = 0; i < text.Length; i++)
		{
			label.Text += text[i];
			await ToSignal(GetTree().CreateTimer(0.02f), "timeout");
		}

		// attesa tra comparsa e scomparsa
		await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

		// FASE 2: SCOMPARSA LETTERA PER LETTERA (al contrario)
		for (int i = 1; i <= text.Length; i++)
		{
			label.Text = text.Substring(i);
			await ToSignal(GetTree().CreateTimer(0.02f), "timeout");
		}
	}

	private async Task Final(string esito)
	{
		await StartTyping(esito.ToUpper(), _finalLabel, 1.5f);
		//TODO gestire conseguenze sconfitta e vittoria
		SceneManager.ChangeLevel(LevelName.Back);
		SpaceManager.BlurTextureVisible(true);
	}
}
