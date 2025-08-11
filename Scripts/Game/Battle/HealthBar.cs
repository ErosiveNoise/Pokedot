using Godot;
using System;

public partial class HealthBar : ProgressBar
{

	[Export] private Timer _timer;
	[Export] private ProgressBar _damageBar;
	[Export] private Label _hpLabel;

	private int Health;
	private double colorRange;

	public override void _Ready()
	{
	}

	public void InitHealth(Pokemon _pokemon)
	{
		Health = _pokemon.CurrentHp;
		MaxValue = _pokemon.Hp;
		Value = _pokemon.CurrentHp;
		_damageBar.MaxValue = _pokemon.Hp;//?
		_damageBar.Value = _pokemon.CurrentHp;
		_hpLabel.Text = _pokemon.CurrentHp + " / " + _pokemon.Hp;
		CheckColorBar();
	}

	public void SetHealth(int newHealth)
	{
		int prevHealth = Health;
		Health = (int)Math.Min(MaxValue, newHealth);
		Value = Health;


		if (Health < prevHealth)
		{
			_timer.Start();
		}
		else
		{
			_damageBar.Value = Health;
		}
		CheckColorBar();

		_hpLabel.Text = Health + " / " + _damageBar.MaxValue;
	}

	private void CheckColorBar()
	{
		colorRange = Health * 100 / MaxValue;
		StyleBoxFlat styleBox = new StyleBoxFlat();

		if (100 >= colorRange && colorRange >= 60)
		{
			styleBox.BgColor = Color.Color8(67, 175, 141);
		}
		else if (60 > colorRange && colorRange >= 30)
		{
			styleBox.BgColor = Color.Color8(194, 141, 80);
		}
		else
		{
			styleBox.BgColor = Color.Color8(234, 110, 97);
		}
		AddThemeStyleboxOverride("fill", styleBox);
	}

	public void OnTimerTimeout()
	{
		_damageBar.Value = Health;
	}
}
