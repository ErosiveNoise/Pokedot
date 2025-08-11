using Godot;

public partial class PokemonSlot : Node2D
{

    [Export] public Sprite2D Background;
    [Export] public Sprite2D PokemonSprite;
    [Export] public Label PokemonName;
    [Export] public Label Level;
    [Export] public Label HealthLabel;
    [Export] public Label MaxHealthLabel;
    [Export] public HealthBar HealthBar;

    string imgPath = "Assets/Pokemon/Images/Png/Idle";

    public override void _Ready()
    {
    }

    public void UpdateSlot(Pokemon pokemon)
    {
        PokemonSprite.Texture = GD.Load<Texture2D>(imgPath + "-front/" + pokemon.Name + "/frame_00_delay-0.03s.png");
        PokemonName.Text = pokemon.Name;
        Level.Text = "Lv. " + pokemon.Level;
        HealthLabel.Text = $"{pokemon.CurrentHp}";
        MaxHealthLabel.Text = $"{pokemon.Hp} HP";
        HealthBar.InitHealth(pokemon);
    }

    public void ClearSlot()
    {
        PokemonSprite.Texture = null;
        PokemonName.Text = "--";
        Level.Text = "";
        HealthLabel.Text = "";
        MaxHealthLabel.Text = "";
    }
}
