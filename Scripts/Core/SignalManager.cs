using Godot;
using System;

public partial class SignalManager : Node
{
	public static SignalManager Instance { get; private set; }

	[Signal] public delegate void OnReturnToMenuEventHandler();

	public override void _Ready()
	{
		Instance = this;
	}

	public static void EmitOnReturnToMenu()
	{
		Instance.EmitSignal(SignalName.OnReturnToMenu);
	}
}
