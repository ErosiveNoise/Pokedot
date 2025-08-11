using Godot;
using System;
using System.Threading.Tasks;

public partial class CuraCollision : Area2D
{
	private bool _entered = false;
	[Export] private Label _label;
	//[Export] public PackedScene _menu;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
		TreeExiting += OnTreeExiting;
		_label.Text = "";
	}

	private void OnTreeExiting()
	{
		_label.Text = "";
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.Name == "Player")
		{
			_entered = false;
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Player")
		{
			_entered = true;
			GD.Print("entrato");
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (SceneManager.Instance.IsPaused)
		{
			return;
		}

		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (_entered)
			{
				ComplexMessage message = new ComplexMessage
				{
					Name = "Pokecenter",
					Message = "I tuoi pokemon ora sono di nuovo in salute."
				};
				_ = DialogueManager.Instance.StartDialogue(message, true);
				foreach (Pokemon p in SceneManager.Instance.Team)
				{
					p.CurrentHp = p.Hp;
					foreach (Attack a in p.Attacks)
					{
						a.CurrentPP = a.PP;
					}
				}
			}
		}
	}

	// private async Task StartTyping(string text, Label label, float waitTime)
	// {
	// 	// FASE 1: COMPARSA LETTERA PER LETTERA
	// 	for (int i = 0; i < text.Length; i++)
	// 	{
	// 		label.Text += text[i];
	// 		await ToSignal(GetTree().CreateTimer(0.02f), "timeout");
	// 	}

	// 	// attesa tra comparsa e scomparsa
	// 	await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

	// 	// FASE 2: SCOMPARSA LETTERA PER LETTERA (al contrario)
	// 	for (int i = 1; i <= text.Length; i++)
	// 	{
	// 		label.Text = text.Substring(i);
	// 		await ToSignal(GetTree().CreateTimer(0.02f), "timeout");
	// 	}
	// }
}