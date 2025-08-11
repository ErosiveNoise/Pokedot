using Godot;
using System;
using System.Threading.Tasks;

public class ComplexMessage
{
    public string Name { get; set; }
    public string Message { get; set; }
}

public partial class DialogBox : CanvasLayer
{
	private RichTextLabel dialogLabel;
	private Label name;
	[Export] private AnimationPlayer _animPlayer;

	public override void _Ready()
	{
		dialogLabel = GetNode<RichTextLabel>("Control/Panel/DialogLabel");
		name = GetNode<Label>("Control/Panel/Name");
		//HideDialog();
		Hide();
	}

	public async Task ShowDialog(ComplexMessage text)
	{
		Show();
		dialogLabel.Text = "";
		_animPlayer.Play("showMenu");
		await StartTyping(text, 1);
	}

	public async Task HideDialogAsync()
	{
		_animPlayer.PlayBackwards("showMenu");
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		Hide();
	}

	public async Task StartTyping(ComplexMessage text, float waitTime)
	{
		name.Text = text.Name.ToUpper();
		await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
		//TODO forse conviene rimuovere process a PLAYER
		//GetTree().Paused = true;

		// FASE 1: COMPARSA LETTERA PER LETTERA
		for (int i = 0; i < text.Message.Length; i++)
		{
			dialogLabel.Text += text.Message[i];
			await ToSignal(GetTree().CreateTimer(0.02f), "timeout");
		}

		// attesa tra comparsa e scomparsa
		await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

		// FASE 2: SCOMPARSA LETTERA PER LETTERA (al contrario)
		// for (int i = 1; i <= text.Length; i++)
		// {
		// 	dialogLabel.Text = text.Substring(i);
		// 	await ToSignal(GetTree().CreateTimer(0.02f), "timeout");
		// }
	}
}
