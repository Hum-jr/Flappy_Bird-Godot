using Godot;
using FlappyBird;

public partial class GameManager : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var pipeSpawner = GetNode<PipeSpawner>("PipeSpawner");
		pipeSpawner.StartSpawning();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
