using System;
using Godot;

namespace FlappyBird;

public partial class PipeSpawner : Node2D
{
    [Export] public PackedScene PipeScene { get; set; }
    [Export] public float SpawnInterval = 2.5f;
    [Export] public float PipeSpeed = 200f;
    [Export] public float GapSize = 150f;
    [Export] public float MinHeight = 100f;
    [Export] public float MaxHeight = 300f;
    
    private Timer _spawnTimer;
    private bool _gameActive = false;
    private readonly Random _random = new Random();
    
    public override void _Ready()
    {
        // Ensure SpawnInterval is valid
        if (SpawnInterval <= 0)
            SpawnInterval = 2.5f;
            
        // Create and setup spawn timer
        _spawnTimer = GetNode<Timer>("_spawnTimer");
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Timeout += SpawnPipe;
        _spawnTimer.Autostart = false;
        
        GD.Print("PipeSpawner ready");
    }

    public void StartSpawning()
    {
        _gameActive = true;
        _spawnTimer.Start();
        
        GD.Print("PipeSpawner started");
    }

    private void StopSpawning()
    {
        _gameActive = false;
        _spawnTimer.Stop();
        
        // Remove all existing pipes
        GetTree().CallGroup("pipes", "queue_free");
    }
    
    private void SpawnPipe()
    {
        GD.Print("Pipe spawning");
        if (!_gameActive || PipeScene == null)
            return;
            
        // Create pipe pair
        var pipePair = new Node2D();
        pipePair.AddToGroup("pipes");
        GetParent().AddChild(pipePair);
        
        
        // Set spawn position (off-screen to the right)
        var spawnX = GetViewportRect().Size.X + 50;
        var screenHeight = GetViewportRect().Size.Y;
        
        // Random gap position
        var gapCenter = _random.NextSingle() * (screenHeight - MaxHeight - GapSize) + MinHeight + GapSize/2 -400;
        
        // Create top pipe
        if (PipeScene.Instantiate() is StaticBody2D topPipe)
        {
            topPipe.Position = new Vector2(spawnX, gapCenter - GapSize / 2);
            topPipe.Scale = new Vector2(1, -1); // Flip vertically for top pipe
            pipePair.AddChild(topPipe);
        }

        // Create bottom pipe
        if (PipeScene.Instantiate() is StaticBody2D bottomPipe)
        {
            bottomPipe.Position = new Vector2(spawnX, gapCenter + GapSize / 2 + 400);
            pipePair.AddChild(bottomPipe);
        }
    }
}