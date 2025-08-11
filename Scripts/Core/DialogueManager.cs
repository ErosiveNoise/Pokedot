using Godot;
using System;
using System.Threading.Tasks;

public partial class DialogueManager : Node
{
	public static DialogueManager Instance { get; private set; }
    
    private DialogBox dialogBox;
    private bool isActive = false;
	PackedScene dialogueScene = GD.Load<PackedScene>("Scenes/Core/DialogBox.tscn");
    
    public override void _Ready()
	{
		Instance = this;

		// Carica la scena del dialogo una sola volta
		CallDeferred(nameof(LoadDialogScene));
	}

	private void LoadDialogScene()
	{
		dialogBox = dialogueScene.Instantiate<DialogBox>();
		GetTree().Root.AddChild(dialogBox);
		//SpaceManager.GetCurrentScene().AddChild(dialogBox);
		dialogBox.Hide(); // Inizialmente nascosta
    }

	public async Task StartDialogue(ComplexMessage dialog, bool autoClose)
	{
		if (isActive) return;

		Node player = SpaceManager.GetPlayer();
		player.SetPhysicsProcess(false);

		isActive = true;
		await dialogBox.ShowDialog(dialog);
		EndDialogue();

		// Pausa il gioco
		//GetTree().Paused = true;

		if (autoClose) EndDialogue();
	}

	public void EndDialogue()
	{
		//GetTree().Paused = false;
		isActive = false;
		_ = dialogBox.HideDialogAsync();
		Node player = SpaceManager.GetPlayer();
		player.SetPhysicsProcess(true);
    }
}