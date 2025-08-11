using Godot;
using System;

public partial class TransitionEffect : CanvasLayer
{
	private ShaderMaterial material;

	public override void _Ready()
	{
		material = GetNode<ColorRect>("TransitionRect").Material as ShaderMaterial;

		// Imposta la risoluzione attuale del viewport
		Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
		material.SetShaderParameter("screen_size", viewportSize);

		StartTransition();
	}


	private async void StartTransition()
    {
        // Chiude
        for (float p = 1.0f; p >= 0.0f; p -= 0.02f)
        {
            material.SetShaderParameter("progress", p);
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
        }

        await ToSignal(GetTree().CreateTimer(1.0f), "timeout");

        // Apre
        for (float p = 0.0f; p <= 1.0f; p += 0.02f)
        {
            material.SetShaderParameter("progress", p);
            await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
        }
    }
}
